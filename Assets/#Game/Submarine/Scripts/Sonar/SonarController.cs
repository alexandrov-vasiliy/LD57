// SonarController.cs

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using _Game;
using _Game.Marker;
using NaughtyAttributes;

public enum EMarkerType
{
    ENEMY,
    OBSTACLE
}

public class SonarController : MonoBehaviour
{
    [Serializable]
    public class MarkerMapping
    {
        [Tag] public string Tag;
        public EMarkerType MarkerType;
    }

    [Header("Sonar Settings")] [SerializeField]
    private List<MarkerMapping> markerMappings = new List<MarkerMapping>();
    
    [SerializeField] private Material sonarMaterial;
    [SerializeField] private Color lineColor = Color.white;
    [SerializeField] private float rotationSpeed = 1.0f; // Скорость вращения луча
    [SerializeField] private float lineWidth = 0.05f; // Толщина линии-сканера
    [SerializeField] private float scanDistance = 50f; // Дальность сканирования
    [SerializeField] private LayerMask scanLayer; // Слой объектов для сканирования

    [Header("Markers Settings")] [SerializeField]
    private Color enemyColor;

    [SerializeField] private Color obstacleColor;
    [SerializeField] private float markerLifeTime = 3f;

    private float time;
    public bool isSonarActive { private set; get; }

    private void Start()
    {
        ActivateSonar();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ToggleSonar();
        }

        if (!isSonarActive)
        {
            DeactivateSonarMaterial();
            return;
        }


        SonarEvents.SonarActivated(transform, isSonarActive);
        UpdateSonarMaterialProperties();

        time += Time.deltaTime;
        Vector3 direction = CalculateDirection();

        Debug.DrawRay(transform.position, direction * scanDistance, Color.green);
        ProcessScan(direction);
    }


    /// <summary>
    /// Обновляет параметры материала сонарного сканера.
    /// </summary>
    private void UpdateSonarMaterialProperties()
    {
        sonarMaterial.SetFloat("_LineWidth", lineWidth);
        sonarMaterial.SetFloat("_TimeValue", time);
        sonarMaterial.SetColor("_Color", lineColor);
        sonarMaterial.SetFloat("_RotationSpeed", rotationSpeed);
        sonarMaterial.SetVector("_Center", transform.position);
    }

    /// <summary>
    /// Выключает визуализацию сонарного сканера.
    /// </summary>
    private void DeactivateSonarMaterial()
    {
        sonarMaterial.SetFloat("_LineWidth", 0f);
    }

    /// <summary>
    /// Вычисляет направление луча на основе времени и скорости вращения.
    /// </summary>
    /// <returns>Вектор направления</returns>
    private Vector3 CalculateDirection()
    {
        float angle = time * rotationSpeed;
        return new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f);
    }

    /// <summary>
    /// Выполняет сканирование в указанном направлении.
    /// </summary>
    /// <param name="direction">Направление сканирования</param>
    private void ProcessScan(Vector3 direction)
    {
        if (Physics.Raycast(transform.position, direction, out RaycastHit hit, scanDistance, scanLayer))
        {
            HandleMarkerHit(hit);
        }
    }

    /// <summary>
    /// Обрабатывает попадание луча в объект.
    /// </summary>
    /// <param name="hit">Результат столкновения</param>
    private void HandleMarkerHit(RaycastHit hit)
    {
        EMarkerType markerType = GetMarkerForTag(hit.collider.tag);
        var marker = G.MarkerPool.GetObject();
        marker.transform.position = hit.point;

        StartCoroutine(ExecuteAfterDelay(() => G.MarkerPool.ReturnObject(marker), markerLifeTime));
        SetMarkerColor(marker, markerType);
    }

    /// <summary>
    /// Устанавливает цвет маркера в зависимости от его типа.
    /// </summary>
    /// <param name="marker">Объект маркера</param>
    /// <param name="markerType">Тип маркера</param>
    private void SetMarkerColor(MarkerComponent marker, EMarkerType markerType)
    {
        switch (markerType)
        {
            case EMarkerType.ENEMY:
                marker.SetColor(enemyColor);
                G.sfx.PlayEffect(G.sfx.sonarEffect);
                break;
            case EMarkerType.OBSTACLE:
                marker.SetColor(obstacleColor);
                break;
            default:
                throw new AggregateException("Необработанный тип маркера");
                break;
        }
    }

    /// <summary>
    /// Возвращает тип маркера для заданного тега.
    /// </summary>
    /// <param name="tag">Тег объекта</param>
    /// <returns>Тип маркера</returns>
    private EMarkerType GetMarkerForTag(string tag)
    {
        foreach (var mapping in markerMappings)
        {
            if (mapping.Tag == tag)
                return mapping.MarkerType;
        }

        return EMarkerType.OBSTACLE;
    }

    /// <summary>
    /// Включает работу сонара.
    /// </summary>
    public void ActivateSonar()
    {
        isSonarActive = true;

        time = 0f;
    }

    /// <summary>
    /// Переключает состояние сонара.
    /// </summary>
    public void ToggleSonar()
    {
        isSonarActive = !isSonarActive;
        
        time = 0f;
    }



    /// <summary>
    /// Выполняет заданное действие с задержкой.
    /// </summary>
    /// <param name="action">Действие для выполнения</param>
    /// <param name="seconds">Задержка в секундах</param>
    /// <returns>Корутина</returns>
    private IEnumerator ExecuteAfterDelay(Action action, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        action?.Invoke();
    }


}