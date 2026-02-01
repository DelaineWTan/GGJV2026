using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private int totalCollectibles;
    private int collectedCount = 0;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        totalCollectibles = FindObjectsByType<Collectible>(FindObjectsSortMode.None).Length;
        Debug.Log("Total collectibles: " + totalCollectibles);
    }

    public void CollectItem()
    {
        collectedCount++;
        Debug.Log($"Collected: {collectedCount}/{totalCollectibles}");
    }

    public bool AllCollected()
    {
        return collectedCount >= totalCollectibles;
    }
}
