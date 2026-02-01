using UnityEngine;

public class ProximityAudio : MonoBehaviour
{
    [SerializeField] private AudioClip audioClip;
    [SerializeField] private float maxDistance = 10f;
    [SerializeField] private float maxVolume = 1f;
    
    private AudioSource _audioSource;
    private Transform _player;
    
    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        
        _audioSource = gameObject.AddComponent<AudioSource>();
        _audioSource.clip = audioClip;
        _audioSource.loop = true;
        _audioSource.spatialBlend = 0; // 2D sound
        _audioSource.Play();
    }
    
    private void Update()
    {
        var distance = Vector2.Distance(transform.position, _player.position);
        
        // Volume fades from maxVolume (at 0 distance) to 0 (at maxDistance)
        float volume = Mathf.Clamp01(1 - (distance / maxDistance)) * maxVolume;
        _audioSource.volume = volume;
    }
}