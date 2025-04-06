using System;
using _Game;
using UnityEngine;
using UnityEngine.Audio;

public class SFX : MonoBehaviour
{
    [SerializeField] public AudioClip collideEffect;
    [SerializeField] public AudioClip attackEffect;
    [SerializeField] public AudioClip sonarEffect;
    private AudioSource _audioSource;


    private void Awake()
    {
        _audioSource = gameObject.GetComponent<AudioSource>();
        G.sfx = this;
    }

    public void PlayEffect(AudioClip audio, float volume = 1f)
    {
        Debug.Log("Play Effect " + audio.name );
        _audioSource.PlayOneShot(audio, volume);
    }
    
    
}
