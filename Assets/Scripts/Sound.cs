using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip audioClip;

    [Range(0, 1)]
    public float volume;

    [HideInInspector]
    public AudioSource audioSource;
}
