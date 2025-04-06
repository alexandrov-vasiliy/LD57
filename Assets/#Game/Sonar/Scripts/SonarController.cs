// SonarController.cs

using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using _Game;

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
        public string tag;
        public EMarkerType markerType;
    }

    [Header("Sonar Settings")]
    
    [SerializeField] private List<MarkerMapping> markerMappings = new();
    [SerializeField] private Material sonarMaterial;
    [SerializeField] private Color lineColor = Color.white;
    [SerializeField] private float rotationSpeed = 1.0f; // Скорость вращения луча
    [SerializeField] private float lineWidth = 0.05f; // Толщина линии-сканера
    [SerializeField] private float scanDistance = 50f; // Дальность сканирования
    [SerializeField] private LayerMask scanLayer; // Слой объектов, по которым сканировать

    [Header("Markers Settings")] 
    [SerializeField] private Color _enemyColor;
    [SerializeField] private Color _obstacleColor;
    
    private float time;
    private List<Vector3> hitPoints = new();
    private bool isSonarActive = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isSonarActive = !isSonarActive;
            time = 0f;
        }

        if (isSonarActive)
        {
            SonarEvents.SonarActivated(gameObject.transform, isSonarActive);
            sonarMaterial.SetFloat("_LineWidth", lineWidth);
        }
        else
        {
            sonarMaterial.SetFloat("_LineWidth", 0f);
            return;
        }

        time += Time.deltaTime;

        // лучевой угол (вращение в плоскости XY)
        float angle = time * rotationSpeed;
        Vector3 direction = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f); // вращение в плоскости XY

        // передаём параметры в шейдер
        sonarMaterial.SetFloat("_TimeValue", time);
        sonarMaterial.SetColor("_Color", lineColor);
        sonarMaterial.SetFloat("_RotationSpeed", rotationSpeed);
        sonarMaterial.SetFloat("_LineWidth", lineWidth);
        sonarMaterial.SetVector("_Center", transform.position);

        // Визуализация луча
        Debug.DrawRay(transform.position, direction * scanDistance, Color.green);

        // сканируем по направлению
        if (Physics.Raycast(transform.position, direction, out RaycastHit hit, scanDistance, scanLayer))
        {
            //Debug.Log($"Сонар столкнулся с: {hit.collider.tag} в точке {hit.point}");
            RegisterHit(hit.point);

            EMarkerType markerType = GetMarkerForTag(hit.collider.tag);
            var marker = G.MarkerPool.GetObject();
            marker.transform.position = hit.point;
            
            StartCoroutine(DelayedExecution(() => G.MarkerPool.ReturnObject(marker), 2));

            switch (markerType)
            {
                case EMarkerType.ENEMY:
                    marker.SetColor(_enemyColor);
                    break;
                case EMarkerType.OBSTACLE:
                    marker.SetColor(_obstacleColor);
                   break;
                
                default:
                    Debug.LogError("Add New MarkerType Handler");
                    break;
            }
            
            

            
        }

        // передаём слепки в шейдер
        /*sonarMaterial.SetFloat("_HitCount", hitPoints.Count);
        if (hitPoints.Count > 0)
        {
            Vector4[] arr = new Vector4[50];
            for (int i = 0; i < hitPoints.Count && i < 50; i++)
                arr[i] = hitPoints[i];
            sonarMaterial.SetVectorArray("_HitPoints", arr);
        }*/
    }

    private EMarkerType GetMarkerForTag(string tag)
    {
        foreach (var mapping in markerMappings)
        {
            if (mapping.tag == tag)
                return mapping.markerType;
        }

        return EMarkerType.OBSTACLE;
    }

    public void RegisterHit(Vector3 point)
    {
        if (hitPoints.Count >= 50)
            hitPoints.RemoveAt(0);
        hitPoints.Add(point);
    }

    public IEnumerator DelayedExecution(Action fn, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        
        fn.Invoke();
    }
    
}