using UnityEngine;
using System.Linq;
using UnityEngine.Audio;

public class ProximityAudioManager : MonoBehaviour
{
    [SerializeField] private float maxDistance = 10f;
    [SerializeField] private AudioClip collectibleSound;
    [SerializeField] private AudioClip enemySound;
    [SerializeField] private AudioMixerGroup sfxMixerGroup;
    
    private Transform _player;
    private AudioSource _collectibleSource;
    private AudioSource _enemySource;
    
    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        
        // Create two AudioSources
        _collectibleSource = gameObject.AddComponent<AudioSource>();
        _collectibleSource.clip = collectibleSound;
        _collectibleSource.loop = true;
        _collectibleSource.outputAudioMixerGroup = sfxMixerGroup;
        
        _enemySource = gameObject.AddComponent<AudioSource>();
        _enemySource.clip = enemySound;
        _enemySource.loop = true;
        _enemySource.outputAudioMixerGroup = sfxMixerGroup;
    }
    
    private void Update()
    {
        // Play closest collectible sound
        var collectibles = FindObjectsByType<Collectible>(FindObjectsSortMode.None);
        if (collectibles.Length > 0)
        {
            var closest = collectibles.OrderBy(c => 
                Vector2.Distance(_player.position, c.transform.position)).First();
            var distance = Vector2.Distance(_player.position, closest.transform.position);
            
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
        }
        else if (_enemySource.isPlaying)
        {
            _enemySource.Stop();
        }
    }
}
