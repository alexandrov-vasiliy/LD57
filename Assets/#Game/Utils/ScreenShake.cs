using NaughtyAttributes;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    public float screenShakeSetting = 8f; 

    private float shakeAmount;
    private float originalShakeAmount;
    private Vector3 originalPosition;
    private Quaternion originalRotation;

    void Start()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        CalculateShakeAmount();
    }

    void Update()
    {
        if (shakeAmount > 0)
        {
            float dt = Time.deltaTime;
            shakeAmount = Mathf.Max(0, shakeAmount - (50 * dt * (shakeAmount > 0.05f ? 1 : 0)));

            // Apply shake effect
            float realTime = Time.realtimeSinceStartup; 
            transform.rotation = originalRotation * Quaternion.Euler(0f, 0f, (0.001f * Mathf.Sin(0.3f * realTime) + 0.002f * shakeAmount * Mathf.Sin(39.913f * realTime)) * shakeAmount);
            transform.position = originalPosition + new Vector3(
                (shakeAmount) * (0.015f * Mathf.Sin(0.913f * realTime) + 0.01f * shakeAmount * Mathf.Sin(19.913f * realTime)),
                (shakeAmount) * (0.015f * Mathf.Sin(0.952f * realTime) + 0.01f * shakeAmount * Mathf.Sin(21.913f * realTime)),
                0);

            // Reset position and rotation if shakeAmount is too low, to avoid drift from original position/rotation
            if (shakeAmount <= 0.05f)
            {
                transform.position = originalPosition;
                transform.rotation = originalRotation;
            }
        }
    }

    public void UpdateScreenShakeSetting(float newSetting)
    {
        screenShakeSetting = newSetting;
        CalculateShakeAmount();
    }

    [Button]
    void CalculateShakeAmount()
    {
        originalShakeAmount = screenShakeSetting * 3f;
        shakeAmount = originalShakeAmount < 0.05f ? 0 : originalShakeAmount;
    }
}