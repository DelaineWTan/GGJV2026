using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private TextMeshProUGUI restartText;
    [SerializeField] private TextMeshProUGUI quitText;
    
    [Header("Messages")]
    [SerializeField] private string winMessage = "YOU WIN!";
    [SerializeField] private string loseMessage = "GAME OVER";
    
    [Header("Colors")]
    [SerializeField] private Color selectedColor = Color.yellow;
    [SerializeField] private Color unselectedColor = Color.white;
    
    private int _selectedIndex = 0; // 0 = Restart, 1 = Quit
    private bool _isActive = false;
    private PlayerController _playerController;

    private void Start()
    {
        panel.SetActive(false);
        _playerController = FindFirstObjectByType<PlayerController>();
    }

    private void Update()
    {
        if (!_isActive) return;
        
        // Navigate menu
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            _selectedIndex = 0;
            UpdateSelection();
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            _selectedIndex = 1;
            UpdateSelection();
        }
        
        // Confirm selection
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            if (_selectedIndex == 0)
                RestartScene();
            else
                QuitGame();
        }
    }

    public void ShowWin()
    {
        messageText.text = winMessage;
        panel.SetActive(true);
        _isActive = true;
        _selectedIndex = 0;
        UpdateSelection();
        Time.timeScale = 0;
        _playerController.enabled = false;
    }

    public void ShowLose()
    {
        messageText.text = loseMessage;
        panel.SetActive(true);
        _isActive = true;
        _selectedIndex = 0;
        UpdateSelection();
        Time.timeScale = 0;
        _playerController.enabled = false;
    }

    private void UpdateSelection()
    {
        // Highlight selected option
        restartText.color = _selectedIndex == 0 ? selectedColor : unselectedColor;
        quitText.color = _selectedIndex == 1 ? selectedColor : unselectedColor;
    }

    private void RestartScene()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void QuitGame()
    {
        Time.timeScale = 1;
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
