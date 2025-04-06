using UnityEngine;
using System.Collections;
using NaughtyAttributes;

[ExecuteInEditMode]
public class CRTGlitchEffect : MonoBehaviour
{
    [Header("Настройки эффекта")]
    public Shader glitchShader;
    [Range(0, 1)]
    public float glitchIntensity = 0.5f;   
    [Range(0, 1)]
    public float noiseIntensity = 0.5f;   
    [Range(0, 1)]
    public float fishEye = 0.5f;

    private Material glitchMaterial;

    void Start() {
        if (glitchShader == null) {
            Debug.LogError("Не назначен шейдер для эффекта!");
            enabled = false;
            return;
        }
        else
        {
            enabled = true;
        }
        glitchMaterial = new Material(glitchShader);
    }

    // Метод OnRenderImage применяется к изображению камеры после рендеринга
    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (glitchMaterial != null)
        {
            glitchMaterial.SetFloat("_GlitchIntensity", glitchIntensity);
            glitchMaterial.SetFloat("_NoiseIntensity", noiseIntensity);
            glitchMaterial.SetFloat("_FisheyeIntensity", fishEye);
            Graphics.Blit(src, dest, glitchMaterial);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }

    // Публичный метод для активации эффекта с заданной интенсивностью и длительностью
    [Button]
    public void ActivateGlitch(float intensity, float duration)
    {
        StartCoroutine(GlitchCoroutine(intensity, duration));
    }

    // Корутина для временного изменения интенсивности эффекта
    private IEnumerator GlitchCoroutine(float intensity, float duration)
    {
        float originalIntensity = glitchIntensity;
        glitchIntensity = intensity;
        yield return new WaitForSeconds(duration);
        glitchIntensity = originalIntensity;
    }
}