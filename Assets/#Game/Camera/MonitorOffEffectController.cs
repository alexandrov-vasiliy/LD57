using UnityEngine;
using System.Collections;
using NaughtyAttributes;

public class MonitorOffEffectController : MonoBehaviour
{
    [Header("Материал с MonitorOffEffect шейдером")]
    public Material monitorMaterial;
    
    [Header("Длительность эффекта в секундах")]
    public float duration = 1.5f;

    [Button]
    public void OffMonitor()
    {
        StartCoroutine(AnimateOff());
    }

    [Button]
    public void OnMonitor()
    {
        StartCoroutine(AnimateOn());
    }

    IEnumerator AnimateOn()
    {
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            // Интерполируем значение _Off от 1 до 0 для эффекта включения
            float onValue = Mathf.Clamp01(1 - (timer / duration));
            monitorMaterial.SetFloat("_Off", onValue);
            yield return null;
        }
    }

    IEnumerator AnimateOff()
    {
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            // Интерполируем значение _Off от 0 до 1 для эффекта выключения
            float offValue = Mathf.Clamp01(timer / duration);
            monitorMaterial.SetFloat("_Off", offValue);
            yield return null;
        }
    }
}
