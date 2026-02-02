using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.Audio;

public class ProximityAudioManager : MonoBehaviour
{
    [SerializeField] private float maxDistance = 10f;
    [SerializeField] private AudioClip collectibleSound;
    [SerializeField] private AudioClip enemySound;
    [SerializeField] private AudioMixerGroup sfxMixerGroup;
    [SerializeField] private Transform exit;
    
    [Header("Enemy Alert")]
    [SerializeField] private AudioClip enemyAlertSound;
    [SerializeField] private float alertDistance = 3f;
    
    private Transform _player;
    private AudioSource _collectibleSource;
    private AudioSource _enemySource;
    private AudioSource _enemyAlertSource;
    
    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        
        // Create AudioSources
        _collectibleSource = gameObject.AddComponent<AudioSource>();
        _collectibleSource.clip = collectibleSound;
        _collectibleSource.loop = true;
        _collectibleSource.outputAudioMixerGroup = sfxMixerGroup;
        
        _enemySource = gameObject.AddComponent<AudioSource>();
        _enemySource.clip = enemySound;
        _enemySource.loop = true;
        _enemySource.outputAudioMixerGroup = sfxMixerGroup;
        
        _enemyAlertSource = gameObject.AddComponent<AudioSource>();
        _enemyAlertSource.clip = enemyAlertSound;
        _enemyAlertSource.loop = true;
        _enemyAlertSource.outputAudioMixerGroup = sfxMixerGroup;
    }
    
    private void Update()
    {
        // Find closest collectible OR exit (if available)
        var collectibles = FindObjectsByType<Collectible>(FindObjectsSortMode.None);
        
        // Build list of positions (collectibles + exit if all collected)
        List<Transform> targets = new List<Transform>();
        foreach (var c in collectibles)
        {
            targets.Add(c.transform);
        }
        
        // Add exit to targets ONLY if all collectibles collected
        if (exit && GameManager.Instance && GameManager.Instance.AllCollected())
        {
            targets.Add(exit);
        }
        
        if (targets.Count > 0)
        {
            var closest = targets.OrderBy(t => 
                Vector2.Distance(_player.position, t.position)).First();
            var distance = Vector2.Distance(_player.position, closest.position);
            
            if (distance <= maxDistance)
            {
                var volume = 1f - (distance / maxDistance);
                _collectibleSource.volume = volume;
                if (!_collectibleSource.isPlaying) _collectibleSource.Play();
            }
            else if (_collectibleSource.isPlaying)
            {
                _collectibleSource.Stop();
            }
        }
        else if (_collectibleSource.isPlaying)
        {
            _collectibleSource.Stop();
        }
        
        // Play closest enemy sound
        var enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        if (enemies.Length > 0)
        {
            var closest = enemies.OrderBy(e => 
                Vector2.Distance(_player.position, e.transform.position)).First();
            var distance = Vector2.Distance(_player.position, closest.transform.position);
            
            if (distance <= maxDistance)
            {
                var volume = 1f - (distance / maxDistance);
                _enemySource.volume = volume;
                if (!_enemySource.isPlaying) _enemySource.Play();
            }
            else if (_enemySource.isPlaying)
            {
                _enemySource.Stop();
            }
            
            // Play alert sound if enemy is very close
            if (distance <= alertDistance)
            {
                var alertVolume = 1f - (distance / alertDistance);
                _enemyAlertSource.volume = alertVolume;
                if (!_enemyAlertSource.isPlaying) _enemyAlertSource.Play();
            }
            else if (_enemyAlertSource.isPlaying)
            {
                _enemyAlertSource.Stop();
            }
        }
        else
        {
            if (_enemySource.isPlaying) _enemySource.Stop();
            if (_enemyAlertSource.isPlaying) _enemyAlertSource.Stop();
        }
    }
}
