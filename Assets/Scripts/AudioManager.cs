using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;   // Singleton reference
    public Sound[] sounds;                 // Assign in Inspector

    void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep across scenes
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Initialize all sounds
        foreach (Sound s in sounds)
        {
            s.audioSource = gameObject.AddComponent<AudioSource>();
            s.audioSource.clip = s.audioClip;
            s.audioSource.volume = s.volume;
        }
    }

    public static void PlaySfx(string name)
    {
        if (Instance == null)
        {
            Debug.LogWarning("AudioManager not initialized yet!");
            return;
        }

        Sound s = Array.Find(Instance.sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning($"Sound '{name}' not found!");
            return;
        }

        s.audioSource.Play();
    }

    public static void StopSfx(string name)
    {
        Sound s = Array.Find(Instance.sounds, sound => sound.name == name);
        if (s != null)
            s.audioSource.Stop();
    }

    public static void PlayMusic(string name)
    {
        Sound s = Array.Find(Instance.sounds, sound => sound.name == name);
        if (s != null)
        {
            s.audioSource.loop = true;
            s.audioSource.Play();
        }
    }
}
