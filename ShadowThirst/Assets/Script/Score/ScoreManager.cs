using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }     // Singleton instance for global, controlled access

    [Header("BatPoints Settings")]
    [SerializeField] private float bloodBatPoints = 2f;           // Score gained from blood vial
    [SerializeField] private float toxicDamage = 1f;              // Score lost from poison vial
    [SerializeField] private float enemyDamage = 1f;              // Score penalty from enemy attacks
    [SerializeField] private float durationBonusBatPoint = 1f;    // Time-based score gain

    [Header("Score Intervals")]
    [SerializeField] private float phaseOneScoreInterval = 5f;
    [SerializeField] private float phaseTwoScoreInterval = 2f;
    [SerializeField] private float phaseThreeScoreInterval = 1f;
    [SerializeField] private float phaseFourScoreInterval = 0.5f;

    private float batPoints = 0f;                    // Current score
    private float scoreTimer = 0f;                   // Timer for time-based score
    private ShadowSpiritController shadowSpirit;     // Player reference

    public float BatPoints => batPoints;             // Read-only score access
    public float EnemyDamage => enemyDamage;         // Read-only enemy damage
    public float ToxicDamage => toxicDamage;         // Read-only toxic damage


    private void Awake()
    {
        // Enforce singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);       // Persist across scenes
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        shadowSpirit = ShadowSpiritController.Instance;

        // Add null check:
        if (shadowSpirit == null)
        {
            Debug.Log("ShadowSpiritController instance not found!");
        }

        batPoints = 0f;
        scoreTimer = 0f;

    }

    // Update is called once per frame
    void Update()
    {
        if (shadowSpirit == null || shadowSpirit.IsDead) return;     // Stop scoring when player is dead

        BatPointsHandler();
    }

    private void BatPointsHandler()
    {
        float elapsedTime = Time.time - shadowSpirit.StartTime;
        scoreTimer += Time.deltaTime;

        // Select interval based on phase
        float interval = elapsedTime <= shadowSpirit.PhaseOneEndTime ? phaseOneScoreInterval :
                         elapsedTime <= shadowSpirit.PhaseTwoEndTime ? phaseTwoScoreInterval :
                         elapsedTime <= shadowSpirit.PhaseThreeEndTime ? phaseThreeScoreInterval :
                         phaseFourScoreInterval;


        // Apply score when interval reached
        if (scoreTimer >= interval)
        {
            scoreTimer = 0f;
            batPoints += durationBonusBatPoint;
        }
    }

    // Called by blood pickup
    public void AddBatPoints()
    {
        batPoints += bloodBatPoints;
    }

    // Subtract score by variable amount (enemy / poison / future hazards)
    public void SubtractBatPoints(float damage)
    {
        batPoints -= damage;
        batPoints = Mathf.Max(batPoints, 0f);    // Prevent negative score
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
