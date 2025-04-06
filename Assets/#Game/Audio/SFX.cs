using System;
using _Game;
using UnityEngine;
using UnityEngine.Audio;

public class SFX : MonoBehaviour
{
    [SerializeField] public AudioClip collideEffect;
    [SerializeField] public AudioClip attackEffect;
    [SerializeField] public AudioClip sonarEffect;


    private void Awake()
    {
        G.sfx = this;
    }

    public void PlayEffect(AudioClip audio, float volume = 0.5f)
    {
        gameObject.GetComponent<AudioSource>().PlayOneShot(audio, volume);
    }
    
    
}
