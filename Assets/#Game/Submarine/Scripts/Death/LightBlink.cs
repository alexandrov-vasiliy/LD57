using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class LightBlink : MonoBehaviour
{
    [SerializeField] private Light spotLight;
    [SerializeField] private Light pointLight;
    public float blinkDuration = 0.1f;
    public int blinkCountAttack = 3;
    public int blinkCountCollision = 1;


    public void HandleAttack()
    {
        StartCoroutine(Blink(blinkCountAttack));
    }

    public void HandleCollision()
    {
        StartCoroutine(Blink(blinkCountCollision));
    }

    private IEnumerator Blink(int count)
    {
        for (int i = 0; i < count; i++)
        {
            SetLights(false);
            yield return new WaitForSeconds(blinkDuration);
            SetLights(true);
            yield return new WaitForSeconds(blinkDuration);
        }
    }

    private void SetLights(bool state)
    {
        if (spotLight != null) spotLight.enabled = state;
        if (pointLight != null) pointLight.enabled = state;
    }


    private void OnEnable()
    {
        Attack.OnAttack += HandleAttack;
        CollisionChecker.OnCollision += HandleCollision;
    }

    private void OnDisable()
    {
        Attack.OnAttack -= HandleAttack;
        CollisionChecker.OnCollision  -= HandleCollision;
    }
}