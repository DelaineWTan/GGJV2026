using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

public class SensorySystem : MonoBehaviour
{
    public enum SenseState { Negative, Neutral, Positive }
    public enum SenseType { See, Hear, Speak }
    
    [Header("Dice SFX")]
    [SerializeField] private AudioClip goodDieRollSfx;
    [SerializeField] private AudioClip badDieRollSfx;
    
    [Header("Dice States - Track separately for UI")]
    public SenseType goodDieState = SenseType.See;
    public SenseType badDieState = SenseType.See;
    
    [Header("Effective States - Computed from both dice")]
    public SenseState seeState = SenseState.Neutral;
    public SenseState hearState = SenseState.Neutral;
    public SenseState speakState = SenseState.Neutral;
    
    [Header("Bad Die")]
    [SerializeField] private float badDieRollInterval = 10f;
    private float _lastBadRoll;
    
    [Header("See System")]
    [SerializeField] private Light2D playerSpotlight;
    
    [Header("Vision Settings - Neutral")]
    [SerializeField] private float neutralInnerAngle = 60f;
    [SerializeField] private float neutralOuterAngle = 90f;
    [SerializeField] private float neutralOuterRadius = 10f;
    [SerializeField] private float neutralIntensity = 0.5f;

    [Header("Vision Settings - Positive")]
    [SerializeField] private float positiveInnerAngle = 90f;
    [SerializeField] private float positiveOuterAngle = 150f;
    [SerializeField] private float positiveOuterRadius = 20f;
    [SerializeField] private float positiveIntensity = 1f;

    [Header("Hear System")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private float negativeVolume = -80f;
    [SerializeField] private float neutralVolume = -10f;
    [SerializeField] private float positiveVolume = 0f;

    [Header("Speak System")]
    [SerializeField] private Transform player;

    [Header("UI Events")]
    public Action<SenseType, SenseType> OnDiceChanged; // (goodDie, badDie)

    private void Update()
    {
        if (Time.time >= _lastBadRoll + badDieRollInterval)
        {
            RollBadDie();
            _lastBadRoll = Time.time;
        }
    }
    
    // Player picks up fruit - rolls good die
    public void RollGoodDie()
    {
        // Random good die roll
        int roll = Random.Range(0, 3);
        goodDieState = (SenseType)roll;
        
        AudioUtils.PlayOneShotSFX(goodDieRollSfx);
        
        RecalculateStates();
        OnDiceChanged?.Invoke(goodDieState, badDieState);
    }

    private void RollBadDie()
    {
        // Random bad die roll
        int roll = Random.Range(0, 3);
        badDieState = (SenseType)roll;
        
        AudioUtils.PlayOneShotSFX(badDieRollSfx);
        
        RecalculateStates();
        OnDiceChanged?.Invoke(goodDieState, badDieState);
    }

    private void RecalculateStates()
    {
        // Reset all to neutral
        seeState = SenseState.Neutral;
        hearState = SenseState.Neutral;
        speakState = SenseState.Neutral;
        
        // Apply good die (positive effect)
        switch (goodDieState)
        {
            case SenseType.See:
                seeState = SenseState.Positive;
                break;
            case SenseType.Hear:
                hearState = SenseState.Positive;
                break;
            case SenseType.Speak:
                speakState = SenseState.Positive;
                break;
        }
        
        // Apply bad die (negative effect) - overrides if same sense
        switch (badDieState)
        {
            case SenseType.See:
                seeState = SenseState.Negative;
                break;
            case SenseType.Hear:
                hearState = SenseState.Negative;
                break;
            case SenseType.Speak:
                speakState = SenseState.Negative;
                break;
        }
        
        ApplySensoryStates();
    }

    private void ApplySensoryStates()
    {
        ApplySeeState();
        ApplyHearState();
        ApplySpeakState();
    }
    
    private void ApplySeeState()
    {
        if (!playerSpotlight) return;
    
        switch (seeState)
        {
            case SenseState.Negative:
                playerSpotlight.enabled = false; // Completely blind
                break;
            
            case SenseState.Neutral:
                playerSpotlight.enabled = true;
                playerSpotlight.pointLightInnerAngle = neutralInnerAngle;
                playerSpotlight.pointLightOuterAngle = neutralOuterAngle;
                playerSpotlight.pointLightOuterRadius = neutralOuterRadius;
                playerSpotlight.intensity = neutralIntensity;
                break;
            
            case SenseState.Positive:
                playerSpotlight.enabled = true;
                playerSpotlight.pointLightInnerAngle = positiveInnerAngle;
                playerSpotlight.pointLightOuterAngle = positiveOuterAngle;
                playerSpotlight.pointLightOuterRadius = positiveOuterRadius;
                playerSpotlight.intensity = positiveIntensity;
                break;
        }
    }
    
    private void ApplyHearState()
    {
        if (!audioMixer) return;
        
        float volume = hearState switch
        {
            SenseState.Negative => negativeVolume,
            SenseState.Neutral => neutralVolume,
            SenseState.Positive => positiveVolume,
            _ => neutralVolume
        };
        
        audioMixer.SetFloat("MasterVolume", volume);
    }
    
    private void ApplySpeakState()
    {
        var enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
    
        foreach (var enemy in enemies)
        {
            switch (speakState)
            {
                case SenseState.Negative:
                    enemy.SetBehavior(Enemy.BehaviorState.Chase);
                    break;
                case SenseState.Neutral:
                    enemy.SetBehavior(Enemy.BehaviorState.Neutral);
                    break;
                case SenseState.Positive:
                    enemy.SetBehavior(Enemy.BehaviorState.Repel);
                    break;
            }
        }
    }
}
