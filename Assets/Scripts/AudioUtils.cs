using UnityEngine;

public static class AudioUtils
{
    public static void PlayOneShotSFX(AudioClip clip, float volume = 0.5f)
    {
        if (!clip) return;
        
        // Create temporary GameObject with AudioSource
        GameObject sfxObject = new GameObject("OneShotSFX");
        AudioSource audioSource = sfxObject.AddComponent<AudioSource>();
        
        // Configure the AudioSource
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 0f; // 2D sound (not positional)
        
        // Play and destroy after clip finishes
        audioSource.Play();
        Object.Destroy(sfxObject, clip.length);
    }
}
