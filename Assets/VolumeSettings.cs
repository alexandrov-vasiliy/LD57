using System;
using _Game.InteractiveObjects;
using UnityEngine;

public class VolumeSettings : MonoBehaviour
{
    public LeverPointerControl VolumeLever;

    public AudioSource sfxSource;
    public AudioSource ambientSource;

    private void Update()
    {
        float volume = Math.Clamp(VolumeLever.leverValue, 0, 1);

        sfxSource.volume = volume;
        ambientSource.volume = volume;
    }
}
