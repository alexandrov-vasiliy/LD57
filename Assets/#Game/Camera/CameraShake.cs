using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;

    [SerializeField] private float shakeDuration = 0.5f;
    [SerializeField] private float shakeMagnitude = 0.1f;

    private Vector3 initialPosition;
    private float currentShakeDuration = 0f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void OnEnable()
    {
        initialPosition = transform.localPosition;
    }

    private void Update()
    {
        if (currentShakeDuration > 0)
        {
            Vector3 shakeOffset = Random.insideUnitSphere * shakeMagnitude;
            transform.localPosition = initialPosition + shakeOffset;
            currentShakeDuration -= Time.deltaTime;
        }
        else
        {
            transform.localPosition = initialPosition;
        }
    }

    public void Shake(float duration, float magnitude)
    {
        shakeDuration = duration;
        shakeMagnitude = magnitude;
        currentShakeDuration = shakeDuration;
    }

    public void Shake() => Shake(shakeDuration, shakeMagnitude);
}