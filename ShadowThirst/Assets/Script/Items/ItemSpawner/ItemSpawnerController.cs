using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ItemSpawnerController : MonoBehaviour
{
    // Enum defining the different types of items that can be spawned
    private enum Items {BloodVial, ShieldVial, PoisonVial, SpeedBoostVail }

    public Tilemap tilemap;                          // Reference to the tilemap used for valid spawn positions
    public GameObject[] objectPrefabs;               // Array of prefabs matching the Items enum order
    public float bloodVialsProbability = 0.4f;       // Chance to spawn a blood vial (healing)
    public float shieldVialProbability = 0.2f;       // Chance to spawn a shield vial (protection)
    public float poisoneVialProbabilty = 0.3f;       // Chance to spawn a poison vial (damage)
    public float speedBoostVialProbability = 0.1f;   // Chance to spawn a speed boost vial (movement)

    public int maxObjects = 20;            // Maximum number of items allowed in the scene
    public float vialsLife = 10f;          // How long items remain before auto-destruction
    public float spawnIntervals = 1f;      // Time between spawn attempts

    private List<Vector3> validSpawnPositions = new List<Vector3>();       // All possible spawn locations on tilemap
    private List<GameObject> spawnObjects = new List<GameObject>();        // Currently active spawned items
    private bool isSpawning = false;                                       // Prevents multiple spawning coroutines from running simultaneously


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GatherValidPositions();
        StartCoroutine(SpawnObjectIfNeeded());
    }


    // Update is called once per frame
    void Update()
    {
        if (!isSpawning && ActiveObjectCount() < maxObjects)
        {
            StartCoroutine(SpawnObjectIfNeeded());
        }
    }


    // Scans the tilemap to collect all positions where items can be spawned
    private void GatherValidPositions()
    {
        validSpawnPositions.Clear();
        BoundsInt boundsInt = tilemap.cellBounds;
        TileBase[] allTiles =  tilemap.GetTilesBlock(boundsInt);
        Vector3 start = tilemap.CellToWorld(new Vector3Int(boundsInt.xMin, boundsInt.yMin, 0));

        for (int x = 0; x <  boundsInt.size.x; x++)
        {
            for (int y = 0; y < boundsInt.size.y; y++)
            {
                TileBase tile = allTiles[x + y * boundsInt.size.x];
                if (tile != null)
                {
                    Vector3 place = start + new Vector3(x + 0.5f, y, 0f);
                    validSpawnPositions.Add(place);
                }
            }
        }
    }


    // Attempts to spawn a single item at a valid position
    private void SpawnObject()
    {
        if (validSpawnPositions.Count == 0) return;

        Vector3 spawnPosition = Vector3.zero;
        bool validPositionFound = false;

        // Search for a position that doesn't have adjacent items
        while (!validPositionFound && validSpawnPositions.Count > 0)
        {
            int randomIndex = Random.Range(0, validSpawnPositions.Count);
            Vector3 potentialPosition = validSpawnPositions[randomIndex];
            Vector3 leftPosition = potentialPosition + Vector3.left;
            Vector3 rightPosition = potentialPosition + Vector3.right;
            Vector3 downPosition = potentialPosition + Vector3.down;
            Vector3 upPosition = potentialPosition + Vector3.up;

            // Check all four adjacent positions for existing items
            if (!PositionHasObject(leftPosition) && !PositionHasObject(rightPosition) && !PositionHasObject(upPosition) && !PositionHasObject(downPosition))
            {
                spawnPosition = potentialPosition;
                validPositionFound = true;
            }

            validSpawnPositions.RemoveAt(randomIndex);
        }

        if (validPositionFound)
        {
            Items itemType = RandomObjectType();
            GameObject gameObject = Instantiate(objectPrefabs[(int) itemType], spawnPosition, Quaternion.identity);
            spawnObjects.Add(gameObject);

            StartCoroutine(DestroyAfterDelay(gameObject, vialsLife));
        }
    }


    // Checks if a position already contains a spawned object
    private bool PositionHasObject(Vector3 positionToCheck)
    {
        return spawnObjects.Any(checkObj => checkObj && Vector3.Distance(checkObj.transform.position, positionToCheck) < 1.0f);
    }


    // Randomly selects an item type based on configured probabilities
    private Items RandomObjectType()
    {
        float randomChoice = Random.value;

        if (randomChoice <= bloodVialsProbability)
        {
            return Items.BloodVial;
        }
        else if (randomChoice <= (bloodVialsProbability + poisoneVialProbabilty))
        {
            return Items.PoisonVial;
        }
        else if (randomChoice <= (bloodVialsProbability + poisoneVialProbabilty + speedBoostVialProbability))
        {
            return Items.SpeedBoostVail;
        }
        else
        {
            return Items.ShieldVial;
        }
    }


    // Coroutine to automatically destroy an item after its lifetime expires
    private IEnumerator DestroyAfterDelay(GameObject gameObject, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (gameObject != null)
        {
            spawnObjects.Remove(gameObject);
            validSpawnPositions.Add(gameObject.transform.position);
            Destroy(gameObject);
        }
    }


    // Returns the current count of active items, cleaning up null references
    private int ActiveObjectCount()
    {
        spawnObjects.RemoveAll(item => item == null);
        return spawnObjects.Count;
    }


    // Main spawning coroutine that continues until maximum object count is reached
    private IEnumerator SpawnObjectIfNeeded()
    {
        isSpawning = true;
        while(ActiveObjectCount() < maxObjects)
        {
            SpawnObject();
            yield return new WaitForSeconds(spawnIntervals);
        }
        isSpawning = false;
    }
}
