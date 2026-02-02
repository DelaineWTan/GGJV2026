using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class CollectibleSpawner : MonoBehaviour
{
    [Header("Random Spawn Settings")]
    [SerializeField] private int numberOfCollectibles = 3;
    
    private void Start()
    {
        RandomizeCollectibles();
    }
    
    private void RandomizeCollectibles()
    {
        // Get all child collectibles
        List<GameObject> allCollectibles = new List<GameObject>();
        
        for (int i = 0; i < transform.childCount; i++)
        {
            allCollectibles.Add(transform.GetChild(i).gameObject);
        }
        
        // Disable all first
        foreach (GameObject collectible in allCollectibles)
        {
            collectible.SetActive(false);
        }
        
        // Shuffle and enable random ones
        List<GameObject> shuffled = allCollectibles.OrderBy(x => Random.value).ToList();
        
        int count = Mathf.Min(numberOfCollectibles, shuffled.Count);
        for (int i = 0; i < count; i++)
        {
            shuffled[i].SetActive(true);
        }
        
        Debug.Log($"Enabled {count} out of {allCollectibles.Count} collectibles");
    }
}
