using UnityEngine;
using UnityEngine.Serialization;

public class ExitDoor : MonoBehaviour
{
    [Header("Visual Components")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Collider2D exitCollider;
    
    [Header("Win SFX")]
    [SerializeField] private AudioClip winSound;
    
    [FormerlySerializedAs("gameOverPanel")]
    [Header("Game Over Panel")]
    [SerializeField] private GameOverUI gameOverUI;
    
    private bool isActive = false;

    private void Start()
    {
        // Start invisible and disabled
        spriteRenderer.enabled = false;
        exitCollider.enabled = false;
    }

    private void Update()
    {
        // Check if all collectibles are collected
        if (!isActive && GameManager.Instance.AllCollected())
        {
            EnableExit();
        }
    }

    private void EnableExit()
    {
        isActive = true;
        spriteRenderer.enabled = true;
        exitCollider.enabled = true;
        
        Debug.Log("Exit is now open!");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && isActive)
        {
            WinGame();
        }
    }

    private void WinGame()
    {
        AudioUtils.PlayOneShotSFX(winSound);
        Debug.Log("YOU WIN!");
        // Find and show win panel
        gameOverUI.ShowWin();
    }
}