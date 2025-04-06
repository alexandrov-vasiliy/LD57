using System;
using System.Collections;
using _Game;
using _Game.Marker;
using _Game.Utils;
using UnityEngine;

public class Main : MonoBehaviour
{
    [SerializeField] private MarkerComponent _marker;
    [SerializeField] private MonitorOffEffectController _dataMonitor;
    [SerializeField] private CRTGlitchEffect _dataMonitorGlitch;

    [SerializeField] private MonitorOffEffectController _sonarMonitor;
    [SerializeField] private CRTGlitchEffect _sonarMonitorGlitch;
    [SerializeField] private GameObject _Game;
    void Awake()
    {
        G.MarkerPool = new ObjectPool<MarkerComponent>(_marker, 200, transform);
        _dataMonitor.OnMonitor();
        _sonarMonitor.OnMonitor();
    }

    private void OnEnable()
    {
        Health.OnDeath += HandleDeath;
    }

    private void OnDisable()
    {
        Health.OnDeath -= HandleDeath;
    }

    private void HandleDeath()
    {
        StartCoroutine(DeathRoutine());
    }

    private IEnumerator DeathRoutine()
    {
        
        yield return new WaitForSeconds(1f);
        _sonarMonitorGlitch.glitchIntensity = 1;
        _sonarMonitorGlitch.noiseIntensity = 1;
        _dataMonitorGlitch.glitchIntensity = 1;
        _dataMonitorGlitch.noiseIntensity = 1;
        /*_sonarMonitor.OffMonitor();
        _dataMonitor.OffMonitor();*/
        _Game.SetActive(false);
    }

    void Update()
    {
    }
}