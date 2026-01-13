using System;
using System.Collections;
using UnityEngine;

public class LightSystemController : MonoBehaviour
{
    [Header("Behavior Toggles")]
    [SerializeField] private bool enablePatrol = true;
    [SerializeField] private bool enableRotation = true;

    [Header("Patrol Settings")]
    [SerializeField] private Vector3 pointA;
    [SerializeField] private Vector3 pointB;
    [SerializeField] private float patrolSpeed = 1.0f;
    [SerializeField] private float patrolDelay = 1.0f;

    private Vector3 patrolTarget;
    private bool isWaitingPatrol;

    [Header("Rotation Bounds")]
    [SerializeField] private float angleA = 0.0f;
    [SerializeField] private float angleB = 45f;

    [Header("Timing")]
    [SerializeField] private float rotationDuration = 1.0f;
    [SerializeField] private float rotationDelay = 0.5f;

    private float rotationTarget;
    private Quaternion startRotation;
    private Quaternion endRotation;
    private float rotationTimer;

    private bool isRotating;
    private bool isWaiting;
    private bool isInCycle;
    private bool targetIsB;
    private bool wasRotationEnabled;

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
    }

    // Update is called once per frame
    void Update()
    {
        //Handle patrol movement if enabled
        if (enablePatrol)
        {
            HandlePatrol();
        }

        //Handle rotation state changes
        if (enableRotation && !wasRotationEnabled)
        {
            //Rotation was just enabled, initialize it
            InitializeRotation();
        }
        else if (!enableRotation && wasRotationEnabled)
        {
            //Rotation was just disabled, clean up rotation state
            isRotating = false;
            isWaiting = false;
            isInCycle = false;
            StopAllCoroutines();
        }

        //Update rotation if active
        if (enableRotation && isRotating)
        {
            HandleRotation();
        }

        //Track previous rotation state for next frame
        wasRotationEnabled = enableRotation;
    }


    // -------------------- PATROL --------------------
   private void HandlePatrol()
    {
        //Skip movement if we're waiting at a patrol point
        if (isWaitingPatrol) return;

        //Move towards the current patrol target
        transform.position = Vector3.MoveTowards(transform.position, patrolTarget, patrolSpeed * Time.deltaTime);

        //If we've reached the target (within tolerance), start the delay
        if (Vector3.Distance(transform.position, patrolTarget) < 0.01f)
        {
            StartCoroutine(PatrolDelay());
        }
    }

    private IEnumerator PatrolDelay()
    {
        isWaitingPatrol = true;
        yield return new WaitForSeconds(patrolDelay);

        //Switch to the other patrol point after waiting
        patrolTarget =  patrolTarget == pointA ? pointB : pointA;
        isWaitingPatrol = false;
    }


    // -------------------- ROTATION --------------------

    private void InitializeRotation()
    {
        //Determine which rotation target (A or B) is closest to current rotation
        float current = transform.rotation.eulerAngles.z;
        float distToA = MathF.Abs(Mathf.DeltaAngle(current, angleA));
        float distToB = MathF.Abs(Mathf.DeltaAngle(current, angleB));

        targetIsB = distToB < distToA;
        rotationTarget = targetIsB ? angleB : angleA;

        isInCycle = false;
        StartRotation();
    }

    private void StartRotation()
    {
        startRotation = transform.rotation;
        endRotation = Quaternion.Euler(0f, 0f, rotationTarget);

        //If we're already at the target angle, just wait instead of rotating
        if (Quaternion.Angle(startRotation, endRotation) < 0.1f)
        {
            isWaiting = true;
            StartCoroutine(RotationDelay());
            return;
        }

        rotationTimer = 0f;
        isRotating = true;
    }

    private  void HandleRotation()
    {
        //Update rotation interpolation
        rotationTimer += Time.deltaTime;
        float t = Mathf.Clamp01(rotationTimer / rotationDuration);

        transform.rotation = Quaternion.Slerp(startRotation, endRotation, t);

        //If rotation is complete, start waiting before next rotation
        if (t >= 1f && !isWaiting)
        {
            isWaiting = true;
            StartCoroutine(RotationDelay());
        }
    }

    private IEnumerator RotationDelay()
    {
        isRotating = false;
        yield return new WaitForSeconds(rotationDelay);

        //Mark that we've started the full A->B->A cycle
        if (!isInCycle)
        {
            isInCycle = true;
        }

        //Switch to the other rotation target
        targetIsB = !targetIsB;
        rotationTarget =  targetIsB ? angleB : angleA;

        isWaiting = false;
        StartRotation(); //Start rotating to the new target
    }
}
