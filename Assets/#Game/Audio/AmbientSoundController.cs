using System.Collections;
using UnityEngine;

public class AmbientSoundController : MonoBehaviour
{
    public AudioSource ambientSource;

    void Start()
    {
        if (ambientSource != null && !ambientSource.isPlaying)
            ambientSource.Play();
    }

    public void FadeOut(float duration)
    {
        StartCoroutine(FadeAudio(duration, 0f));
    }

    public void FadeIn(float duration, float targetVolume)
    {
        StartCoroutine(FadeAudio(duration, targetVolume));
    }

    IEnumerator FadeAudio(float duration, float targetVolume)
    {
        float currentTime = 0;
        float start = ambientSource.volume;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            ambientSource.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
            yield return null;
        }
        if (targetVolume == 0)
            ambientSource.Stop();
    }
}

