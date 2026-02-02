using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [System.Serializable]
    public class SpawnPair
    {
        public Transform pointA;
        public Transform pointB;
    }
    
    [Header("Spawn Pairs")]
    [SerializeField] private SpawnPair[] spawnPairs;
    [SerializeField] private Transform player;
    [SerializeField] private Transform exit;

    private void Start()
    {
        RandomizeSpawns();
    }

    private void RandomizeSpawns()
    {
        if (spawnPairs.Length == 0)
        {
            Debug.LogWarning("No spawn pairs defined!");
            return;
        }
        
        // Pick random pair
        int randomPairIndex = Random.Range(0, spawnPairs.Length);
        SpawnPair selectedPair = spawnPairs[randomPairIndex];
        
        // Randomly decide which point is player vs exit
        bool playerAtA = Random.value > 0.5f;
        
        if (playerAtA)
        {
            if (player != null)
                player.position = selectedPair.pointA.position;
            if (exit != null)
                exit.position = selectedPair.pointB.position;
            
            Debug.Log($"Pair {randomPairIndex}: Player at A, Exit at B");
        }
        else
        {
            if (player != null)
                player.position = selectedPair.pointB.position;
            if (exit != null)
                exit.position = selectedPair.pointA.position;
            
            Debug.Log($"Pair {randomPairIndex}: Player at B, Exit at A");
        }
    }
}
