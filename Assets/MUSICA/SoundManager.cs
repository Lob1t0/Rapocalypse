using UnityEngine;

public static class SoundManager
{
    public static void PlaySound2D(AudioClip clip, float volume = 1f)
    {
        if (clip == null || Camera.main == null)
            return;

        AudioSource audio = Camera.main.gameObject.AddComponent<AudioSource>();
        audio.clip = clip;
        audio.volume = volume;
        audio.spatialBlend = 0f; // 2D
        audio.playOnAwake = false;
        audio.loop = false;

        audio.Play();
        Object.Destroy(audio, clip.length);
    }
}

