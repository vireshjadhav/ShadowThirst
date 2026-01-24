using System;
using System.Collections;
using UnityEngine;

public class LightSystemController : MonoBehaviour
{
    private enum LightState { On, Off, Waiting }

    // Represents the current hazard state of the light
    // Waiting is reserved for future extensions (difficulty scaling, scripted events)
    [Header("Behavior Toggles")]
    [SerializeField] private bool enablePatrol = true;         // Enables movement between points
    [SerializeField] private bool enableRotation = true;       // Enables rotation behavior
    [SerializeField] private bool enableOnOffToggle = true;    // Enables timed ON/OFF hazard state

    // -------------------- PATROL --------------------
    [Header("Patrol Settings")]
    [SerializeField] private Vector3 pointA;
    [SerializeField] private Vector3 pointB;
    [SerializeField] private float patrolSpeed = 1.0f;
    [SerializeField] private float patrolDelay = 1.0f;         // Delay when reaching a patrol point

    private Vector3 patrolTarget;          // Current target position
    private bool isWaitingPatrol;          // Prevents movement during delay
    private Coroutine patrolCoroutine;     // Tracks patrol delay coroutine

    // -------------------- ROTATION --------------------
    [Header("Rotation Bounds")]
    [SerializeField] private float angleA = 0.0f;
    [SerializeField] private float angleB = 45f;

    [Header("Timing")]
    [SerializeField] private float rotationDuration = 1.0f;    // Time to rotate between angles
    [SerializeField] private float rotationDelay = 0.5f;       // Pause after reaching an angle

    private float rotationTarget;
    private Quaternion startRotation;
    private Quaternion endRotation;
    private float rotationTimer;

    private bool isRotating;
    private bool isWaitingRotation;
    private bool targetIsB;
    private bool wasRotationEnabled;
    private Coroutine rotationCoroutine;

    // -------------------- ON / OFF TOGGLE --------------------
    [Header("On Off Toggles")]
    [SerializeField] private LightHazardController hazardController;
    [SerializeField] private float minTimeToToggle = 4.0f;
    [SerializeField] private float maxTimeToToggle = 15f;

    private LightState currentState = LightState.On;     // Current hazard state
    private Coroutine toggleCoroutine;                   // Tracks ON/OFF toggle coroutine

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Initialize patrol system
        patrolTarget = pointB;

        //Initialize rotation system if enabled
        if (enableRotation)
        {
            InitializeRotation();
        }

        wasRotationEnabled = enableRotation;

        // Sync initial state with hazard controller
        if (hazardController != null)
        {
            currentState = hazardController.GetHazardActive() ? LightState.On : LightState.Off;
        }

        // Start ON/OFF toggle cycle if enabled
        if (enableOnOffToggle)
        {
            StartToggleCycle();
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Handle patrol movement if enabled
        if (enablePatrol)
        {
            HandlePatrol();
        }

        // Handle rotation state changes
        if (enableRotation && !wasRotationEnabled)
        {
            // Rotation was just enabled, initialize it
            InitializeRotation();
        }
        else if (!enableRotation && wasRotationEnabled)
        {

            StopRotation();
        }

        // Update rotation if active
        if (enableRotation && isRotating)
        {
            HandleRotation();
        }

        // Track previous rotation state for next frame
        wasRotationEnabled = enableRotation;
    }

    private void OnDisable()
    {
        // Cleanly stop all owned coroutines when disabled
        StopToggleCycle();
        StopRotation();
        StopPatrol();
    }

    // -------------------- PATROL --------------------
    private void HandlePatrol()
    {
        // Skip movement if we're waiting at a patrol point
        if (isWaitingPatrol) return;

        // Move towards the current patrol target
        transform.position = Vector3.MoveTowards(transform.position, patrolTarget, patrolSpeed * Time.deltaTime);

        // If we've reached the target (within tolerance), start the delay
        if (Vector3.Distance(transform.position, patrolTarget) < 0.01f)
        {
            patrolCoroutine = StartCoroutine(PatrolDelay());
        }
    }

    private IEnumerator PatrolDelay()
    {
        isWaitingPatrol = true;
        yield return new WaitForSeconds(patrolDelay);

        // Switch to the other patrol point after waiting
        patrolTarget = patrolTarget == pointA ? pointB : pointA;
        isWaitingPatrol = false;
    }

    private void StopPatrol()
    {
        if (patrolCoroutine != null)
        {
            StopCoroutine(patrolCoroutine);
            patrolCoroutine = null;
        }
        isWaitingPatrol = false;
    }

    // -------------------- ROTATION --------------------

    private void InitializeRotation()
    {
        // Determine which rotation target (A or B) is closest to current rotation
        float current = transform.rotation.eulerAngles.z;
        float distToA = Mathf.Abs(Mathf.DeltaAngle(current, angleA));
        float distToB = Mathf.Abs(Mathf.DeltaAngle(current, angleB));

        targetIsB = distToB < distToA;
        rotationTarget = targetIsB ? angleB : angleA;

        StartRotation();
    }

    private void StartRotation()
    {
        startRotation = transform.rotation;
        endRotation = Quaternion.Euler(0f, 0f, rotationTarget);

        // If we're already at the target angle, just wait instead of rotating
        if (Quaternion.Angle(startRotation, endRotation) < 0.1f)
        {
            isWaitingRotation = true;
            rotationCoroutine = StartCoroutine(RotationDelay());
            return;
        }

        rotationTimer = 0f;
        isRotating = true;
        isWaitingRotation = false;
    }

    private void HandleRotation()
    {
        // Update rotation interpolation
        rotationTimer += Time.deltaTime;
        float t = Mathf.Clamp01(rotationTimer / rotationDuration);

        transform.rotation = Quaternion.Slerp(startRotation, endRotation, t);

        // If rotation is complete, start waiting before next rotation
        if (t >= 1f && !isWaitingRotation)
        {
            rotationCoroutine = StartCoroutine(RotationDelay());
        }
    }

    private IEnumerator RotationDelay()
    {
        isRotating = false;
        isWaitingRotation = true;

        yield return new WaitForSeconds(rotationDelay);

        //Switch to the other rotation target
        targetIsB = !targetIsB;
        rotationTarget =  targetIsB ? angleB : angleA;

        isWaitingRotation = false;
        rotationCoroutine = null;

        StartRotation(); // Start rotating to the new target
    }

    // Stops rotation behavior and safely terminates any active rotation coroutine
    private void StopRotation()
    {
        if (rotationCoroutine != null)
        {
            StopCoroutine(rotationCoroutine);
            rotationCoroutine = null;
        }

        isRotating = false;
        isWaitingRotation = false;
    }

    // -------------------- ON / OFF TOGGLE --------------------

    // Enables or disables automatic ON/OFF toggling of the light hazard
    public void SetToggleEnabled(bool enabled)
    {
        enableOnOffToggle = enabled;

        if (enabled)
        {
            StartToggleCycle();
        }
        else
        {
            StopToggleCycle();
        }
    }

    // Starts the ON/OFF toggle coroutine, ensuring only one instance runs
    private void StartToggleCycle()
    {
        StopToggleCycle();

        toggleCoroutine = StartCoroutine(LightToggleCycle());
    }

    // Stops the ON/OFF toggle coroutine without changing the current light state
    private void StopToggleCycle()
    {
        if (toggleCoroutine != null)
        {
            StopCoroutine(toggleCoroutine);
            toggleCoroutine = null;
        }
    }

    // Periodically toggles the light hazard between ON and OFF using random intervals
    private IEnumerator LightToggleCycle()
    {
        while (enableOnOffToggle)
        {
            yield return new WaitForSeconds(GetRandomTimeToToggle());

            if (currentState == LightState.On)
            {
                TurnLightOff();
            }
            else
            {
                TurnLightOn();

            }
        }
    }

    // Activates the light hazard by enabling collision and visibility
    private void TurnLightOn()
    {
        if (hazardController != null)
        {
            hazardController.SetHazardActive(true);
        }
        currentState = LightState.On;
    }

    // Deactivates the light hazard by disabling collision and visibility
    private void TurnLightOff()
    {
        if (hazardController != null)
        {
            hazardController.SetHazardActive(false);
        }
        currentState = LightState.Off;
    }

    // Returns a randomized delay duration for the next toggle cycle
    private float GetRandomTimeToToggle()
    {
        return UnityEngine.Random.Range(minTimeToToggle, maxTimeToToggle);
    }

    // Forces the light ON and pauses automatic toggling (for scripted or difficulty events)
    public void ForcedLightOn()
    {
        StopToggleCycle();
        TurnLightOn();
    }

    // Forces the light OFF and pauses automatic toggling (for safe zones or relief moments)
    public void ForcedLightOff()
    {
        StopToggleCycle();
        TurnLightOff();
    }
}
