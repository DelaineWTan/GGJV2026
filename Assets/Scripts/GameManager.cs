using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI collectibleText;
    [SerializeField] private TextMeshProUGUI instructionText;
    
    private int _totalCollectibles;
    private int _collectedCount = 0;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        // Set global light intensity
        Light2D globalLight = FindFirstObjectByType<Light2D>();
        if (globalLight != null && globalLight.lightType == Light2D.LightType.Global)
        {
            globalLight.intensity = 0.01f;
        }
        
        _totalCollectibles = FindObjectsByType<Collectible>(FindObjectsSortMode.None).Length;
        Debug.Log("Total collectibles: " + _totalCollectibles);
        
        UpdateCollectibleUI();
        UpdateInstructionText();
    }

    public void CollectItem()
    {
        _collectedCount++;
        Debug.Log($"Collected: {_collectedCount}/{_totalCollectibles}");
        UpdateCollectibleUI();
        // Check if all collected
        if (AllCollected())
        {
            UpdateInstructionText();
        }
    }

    private void UpdateCollectibleUI()
    {
        if (collectibleText != null)
        {
            collectibleText.text = $"{_collectedCount} / {_totalCollectibles}";
        }
    }

    private void UpdateInstructionText()
    {
        if (instructionText != null)
        {
            if (AllCollected())
            {
                instructionText.text = "You found them all! Get to the exit!";
            }
            else
            {
                instructionText.text = "You cannot leave until you find all collectibles!";
            }
        }
    }

    public bool AllCollected()
    {
        return _collectedCount >= _totalCollectibles;
    }
}
