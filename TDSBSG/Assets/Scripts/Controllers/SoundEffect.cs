using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffect : MonoBehaviour
{
    Toolbox toolbox;
    EventManager em;

    public string _name;
    public AudioClip _clip;
    public AudioSource _source;
    [Range(0f, 1f)]
    public float _volume = 1f;
    [Range(0.1f, 3f)]
    public float _pitch = 1f;
    public bool _loop = false;

    private void OnEnable()
    {
        toolbox = FindObjectOfType<Toolbox>();
        em = toolbox.GetComponent<EventManager>();

        em.BroadcastRegisterSoundEffect(this);
    }


}
