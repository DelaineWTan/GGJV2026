using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

public class SensorySystem : MonoBehaviour
{
    public enum SenseState { Negative, Neutral, Positive }
    public enum SenseType { See, Hear, Speak }
    
    [Header("Dice SFX")]
    [SerializeField] private AudioClip diceRollingSfx;
    [SerializeField] private AudioClip playerFallSfx; 
    [SerializeField] private AudioClip goodDieRollSfx;
    [SerializeField] private AudioClip badDieRollSfx;
    
    [Header("Dice States - Track separately for UI")]
    public SenseType goodDieState = SenseType.See;
    public SenseType badDieState = SenseType.See;
    
    [Header("Dice Animation Visuals")]
    [SerializeField] private DiceUI diceUI;
    [SerializeField] private float diceFlipSpeed = 0.2f;
    
    [Header("Effective States - Computed from both dice")]
    public SenseState seeState = SenseState.Neutral;
    public SenseState hearState = SenseState.Neutral;
    public SenseState speakState = SenseState.Neutral;
    
    [Header("Bad Die")]
    [SerializeField] private float badDieRollInterval = 10f;
    private float _lastBadRoll;
    
    [Header("Dice Animation")]
    [SerializeField] private float diceRollDelay = 2f;
    private bool _isRolling = false;
    
    [Header("Player Spin Effect")]
    [SerializeField] private float spinSpeed = 720f; // Degrees per second during fall

    
    [Header("See System")]
    [SerializeField] private Light2D playerSpotlight;
    
    [Header("Vision Settings - Neutral")]
    [SerializeField] private float neutralInnerAngle = 60f;
    [SerializeField] private float neutralOuterAngle = 90f;
    [SerializeField] private float neutralOuterRadius = 12f;
    [SerializeField] private float neutralIntensity = 0.6f;

    [Header("Vision Settings - Positive")]
    [SerializeField] private float positiveInnerAngle = 90f;
    [SerializeField] private float positiveOuterAngle = 150f;
    [SerializeField] private float positiveOuterRadius = 20f;
    [SerializeField] private float positiveIntensity = 1f;

    [Header("Vision Settings - Negative")]
    [SerializeField] private float negativeInnerAngle = 30f;
    [SerializeField] private float negativeOuterAngle = 45f;
    [SerializeField] private float negativeOuterRadius = 8f;
    [SerializeField] private float negativeIntensity = 0.3f;

    [Header("Hear System")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private float negativeVolume = -80f;
    [SerializeField] private float neutralVolume = -6f;
    [SerializeField] private float positiveVolume = 0f;

    [Header("Speak System")]
    [SerializeField] private Transform player;
    [SerializeField] private AudioClip speakPositiveLoopSfx;
    
    private AudioSource _speakPositiveSource;

    [Header("UI Events")]
    public Action<SenseType, SenseType> OnDiceChanged;
    
    private PlayerController _playerController;

    private void Start()
    {
        _playerController = FindFirstObjectByType<PlayerController>();
        
        // Initialize AudioUtils with SFX mixer group
        if (audioMixer != null)
        {
            AudioMixerGroup[] sfxGroups = audioMixer.FindMatchingGroups("SFX");
            if (sfxGroups.Length > 0)
                AudioUtils.Initialize(sfxGroups[0]);
        }
        
        RollBadDie(false);
        RollGoodDie(false);
    }

    private void Update()
    {
        if (Time.time >= _lastBadRoll + badDieRollInterval && !_isRolling)
        {
            StartCoroutine(RollBadDieDelayed());
            _lastBadRoll = Time.time;
        }
    }
    
    public void RollGoodDie(bool playSfx = true)
    {
        if (playSfx && !_isRolling)
        {
            StartCoroutine(RollGoodDieDelayed());
        }
        else
        {
            ApplyGoodDieRoll();
        }
    }

    private void RollBadDie(bool playSfx = true)
    {
        if (playSfx && !_isRolling)
        {
            StartCoroutine(RollBadDieDelayed());
        }
        else
        {
            ApplyBadDieRoll();
        }
    }

    private IEnumerator RollGoodDieDelayed()
    {
        _isRolling = true;

        AudioUtils.PlayOneShotSFX(diceRollingSfx);

        FreezeEnemies(true);
        FreezePlayer(true);

        // Stop player momentum
        var playerRb = _playerController.GetComponent<Rigidbody2D>();
        if (playerRb) playerRb.linearVelocity = Vector2.zero;

        // Play fall sound
        if (playerFallSfx)
            AudioUtils.PlayOneShotSFX(playerFallSfx);
    
        // Start spinning
        StartCoroutine(SpinPlayer());

        // Animate good die cycling
        StartCoroutine(AnimateDiceRoll(true));

        yield return new WaitForSeconds(diceRollDelay);

        ApplyGoodDieRoll();
        AudioUtils.PlayOneShotSFX(goodDieRollSfx);

        FreezeEnemies(false);
        FreezePlayer(false);

        _isRolling = false;
    }

    private IEnumerator RollBadDieDelayed()
    {
        _isRolling = true;

        AudioUtils.PlayOneShotSFX(diceRollingSfx);

        FreezeEnemies(true);
        FreezePlayer(true);

        // Stop player momentum
        var playerRb = _playerController.GetComponent<Rigidbody2D>();
        if (playerRb) playerRb.linearVelocity = Vector2.zero;

        // Play fall sound
        if (playerFallSfx)
            AudioUtils.PlayOneShotSFX(playerFallSfx);
    
        // Start spinning
        StartCoroutine(SpinPlayer());

        // Animate bad die cycling
        StartCoroutine(AnimateDiceRoll(false));

        yield return new WaitForSeconds(diceRollDelay);

        ApplyBadDieRoll();
        AudioUtils.PlayOneShotSFX(badDieRollSfx);

        FreezeEnemies(false);
        FreezePlayer(false);

        _isRolling = false;
    }

    private IEnumerator AnimateDiceRoll(bool isGoodDie)
    {
        float elapsed = 0f;
        int currentIndex = 0;
    
        while (elapsed < diceRollDelay)
        {
            // Cycle through see, hear, speak
            SenseType currentType = (SenseType)(currentIndex % 3);
        
            // Tell DiceUI to update (it will pick the right sprite)
            if (isGoodDie)
                diceUI.UpdateDiceDisplay(currentType, badDieState);
            else
                diceUI.UpdateDiceDisplay(goodDieState, currentType);
        
            currentIndex++;
            elapsed += diceFlipSpeed;
        
            yield return new WaitForSeconds(diceFlipSpeed);
        }
    }
    
    private void FreezePlayer(bool freeze)
    {
        if (_playerController)
        {
            _playerController.enabled = !freeze;
        
            // Set to idle animation when frozen
            var animator = _playerController.GetComponent<Animator>();
            if (animator)
            {
                animator.SetBool("IsMoving", !freeze);
            }
        }
    }

    private void ApplyGoodDieRoll()
    {
        int roll = Random.Range(0, 3);
        goodDieState = (SenseType)roll;
        
        RecalculateStates();
        OnDiceChanged?.Invoke(goodDieState, badDieState);
    }

    private void ApplyBadDieRoll()
    {
        int roll = Random.Range(0, 3);
        badDieState = (SenseType)roll;
        
        RecalculateStates();
        OnDiceChanged?.Invoke(goodDieState, badDieState);
    }

    private void FreezeEnemies(bool freeze)
    {
        var enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        
        foreach (var enemy in enemies)
        {
            enemy.SetFrozen(freeze);
        }
    }
    
    private IEnumerator SpinPlayer()
    {
        if (!player) yield break;
    
        float startRotation = player.eulerAngles.z;
        float elapsed = 0f;
    
        // Randomly choose clockwise or counter-clockwise
        float direction = Random.value > 0.5f ? 1f : -1f;
    
        // Calculate total rotations to end at 0 (upright)
        // We want at least 2 full spins (720 degrees)
        float totalRotation = 720f * direction; // 2 full rotations in random direction
    
        while (elapsed < diceRollDelay)
        {
            float progress = elapsed / diceRollDelay;
            float currentRotation = startRotation + (totalRotation * progress);
        
            player.rotation = Quaternion.Euler(0, 0, currentRotation);
        
            elapsed += Time.deltaTime;
            yield return null;
        }
    
        // End exactly upright
        player.rotation = Quaternion.Euler(0, 0, 0);
    }

    private void RecalculateStates()
    {
        seeState = SenseState.Neutral;
        hearState = SenseState.Neutral;
        speakState = SenseState.Neutral;
        
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
                playerSpotlight.enabled = true;
                playerSpotlight.pointLightInnerAngle = negativeInnerAngle;
                playerSpotlight.pointLightOuterAngle = negativeOuterAngle;
                playerSpotlight.pointLightOuterRadius = negativeOuterRadius;
                playerSpotlight.intensity = negativeIntensity;
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
        
        bool success = audioMixer.SetFloat("Master", volume);
        Debug.Log($"Setting MasterVolume to {volume}dB. Success: {success}");
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

        // Control speak positive loop sound on player
        if (speakState == SenseState.Positive)
        {
            // Add audio source if not present
            if (!_speakPositiveSource && player && speakPositiveLoopSfx)
            {
                _speakPositiveSource = player.gameObject.AddComponent<AudioSource>();
                _speakPositiveSource.clip = speakPositiveLoopSfx;
                _speakPositiveSource.loop = true;
            
                // Route through mixer - SFX or Music group
                if (audioMixer)
                {
                    AudioMixerGroup[] sfxGroups = audioMixer.FindMatchingGroups("SFX");
                    if (sfxGroups.Length > 0)
                        _speakPositiveSource.outputAudioMixerGroup = sfxGroups[0];
                }
            
                _speakPositiveSource.Play();
            }
        }
        else
        {
            // Remove audio source when not positive
            if (_speakPositiveSource)
            {
                Destroy(_speakPositiveSource);
                _speakPositiveSource = null;
            }
        }
    }

    public bool IsRolling()
    {
        return _isRolling;
    }
}
