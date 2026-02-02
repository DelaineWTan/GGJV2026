using UnityEngine;
using UnityEngine.Rendering.Universal;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI collectibleText;
    [SerializeField] private TextMeshProUGUI instructionText;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private GameOverUI gameOverUI;
    
    [Header("Audio")]
    [SerializeField] private AudioClip collectiblePickupSfx;
    [SerializeField] private AudioClip allCollectedSfx;
    [SerializeField] private AudioClip damageSfx;
    [SerializeField] private AudioClip deathSfx;
    
    [Header("Player Health")]
    [SerializeField] private int maxHealth = 3;
    private int _currentHealth;
    
    private int _totalCollectibles;
    private int _collectedCount = 0;
    private bool _hasPlayedCompletionSound = false;
    private bool _isGameOver = false;

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
            globalLight.intensity = 0.005f;
        }
        
        _totalCollectibles = FindObjectsByType<Collectible>(FindObjectsSortMode.None).Length;
        Debug.Log("Total collectibles: " + _totalCollectibles);
        
        _currentHealth = maxHealth;
        
        UpdateCollectibleUI();
        UpdateInstructionText();
        UpdateHealthUI();
    }

    public void CollectItem()
    {
        if (_isGameOver) return;
        
        _collectedCount++;
        Debug.Log($"Collected: {_collectedCount}/{_totalCollectibles}");
        UpdateCollectibleUI();
        
        // Play pickup sound
        if (collectiblePickupSfx != null)
            AudioUtils.PlayOneShotSFX(collectiblePickupSfx);
        
        // Check if all collected
        if (AllCollected() && !_hasPlayedCompletionSound)
        {
            UpdateInstructionText();
            PlayCompletionSound();
            _hasPlayedCompletionSound = true;
        }
    }

    public void TakeDamage(int amount)
    {
        if (_isGameOver) return;
        
        _currentHealth -= amount;
        _currentHealth = Mathf.Max(_currentHealth, 0);
        
        UpdateHealthUI();
        
        if (_currentHealth > 0)
        {
            // Still alive - play damage sound
            if (damageSfx != null)
                AudioUtils.PlayOneShotSFX(damageSfx);
        }
        else
        {
            // Dead - trigger game over
            PlayerDied();
        }
    }

    private void PlayerDied()
    {
        _isGameOver = true;
        
        if (deathSfx != null)
            AudioUtils.PlayOneShotSFX(deathSfx);
        
        if (gameOverUI != null)
            gameOverUI.ShowLose();
        
        Debug.Log("Player died!");
    }

    private void UpdateCollectibleUI()
    {
        if (collectibleText != null)
        {
            collectibleText.text = $"{_collectedCount} / {_totalCollectibles}";
        }
    }

    private void UpdateHealthUI()
    {
        if (healthText != null)
        {
            healthText.text = $"{_currentHealth} / {maxHealth}";
        }
    }

    private void UpdateInstructionText()
    {
        if (instructionText != null)
        {
            if (AllCollected())
            {
                instructionText.text = "You found all the bananas! Get to the exit!";
            }
            else
            {
                instructionText.text = "You cannot leave until you find all bananas!";
            }
        }
    }

    private void PlayCompletionSound()
    {
        if (allCollectedSfx != null)
        {
            AudioUtils.PlayOneShotSFX(allCollectedSfx);
        }
    }

    public bool AllCollected()
    {
        return _collectedCount >= _totalCollectibles;
    }

    public bool IsGameOver()
    {
        return _isGameOver;
    }
}
