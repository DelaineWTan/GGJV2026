using UnityEngine;
using UnityEngine.UI;

public class DiceUI : MonoBehaviour
{
    [Header("Good Die Sprites (3 faces)")]
    [SerializeField] private Sprite goodSeeSprite;
    [SerializeField] private Sprite goodHearSprite;
    [SerializeField] private Sprite goodSpeakSprite;
    
    [Header("Bad Die Sprites (3 faces)")]
    [SerializeField] private Sprite badSeeSprite;
    [SerializeField] private Sprite badHearSprite;
    [SerializeField] private Sprite badSpeakSprite;
    
    [Header("UI Images")]
    [SerializeField] private Image goodDieImage;
    [SerializeField] private Image badDieImage;
    
    private SensorySystem _sensorySystem;

    private void Start()
    {
        _sensorySystem = GameManager.Instance.GetComponent<SensorySystem>();
        
        // Subscribe to dice changes
        _sensorySystem.OnDiceChanged += UpdateDiceDisplay;
        
        // Initialize display
        UpdateDiceDisplay(_sensorySystem.goodDieState, _sensorySystem.badDieState);
    }

    private void UpdateDiceDisplay(SensorySystem.SenseType goodDie, SensorySystem.SenseType badDie)
    {
        // Update good die image
        goodDieImage.sprite = goodDie switch
        {
            SensorySystem.SenseType.See => goodSeeSprite,
            SensorySystem.SenseType.Hear => goodHearSprite,
            SensorySystem.SenseType.Speak => goodSpeakSprite,
            _ => goodSeeSprite
        };
        
        // Update bad die image
        badDieImage.sprite = badDie switch
        {
            SensorySystem.SenseType.See => badSeeSprite,
            SensorySystem.SenseType.Hear => badHearSprite,
            SensorySystem.SenseType.Speak => badSpeakSprite,
            _ => badSeeSprite
        };
    }

    private void OnDestroy()
    {
        if (_sensorySystem != null)
            _sensorySystem.OnDiceChanged -= UpdateDiceDisplay;
    }
}