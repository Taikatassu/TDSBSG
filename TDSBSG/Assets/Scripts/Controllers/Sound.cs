using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Sound
{
    public string _name;
    public AudioClip _clip;
    public AudioSource _source;
    [Range(0f, 1f)]
    public float _volume = 1f;
    [Range(0.1f, 3f)]
    public float _pitch = 1f;
    public bool _loop = false;

}
