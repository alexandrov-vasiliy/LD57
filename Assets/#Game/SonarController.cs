// SonarController.cs
using UnityEngine;
using System.Collections.Generic;

public class SonarController : MonoBehaviour
{
    [SerializeField] private GameObject hitMarkerPrefab;  // Префаб визуального маркера попадания
    [SerializeField] private Material sonarMaterial;
    [SerializeField] private float rotationSpeed = 1.0f;      // Скорость вращения луча
    [SerializeField] private float lineWidth = 0.05f;         // Толщина линии-сканера
    [SerializeField] private float scanDistance = 50f;        // Дальность сканирования
    [SerializeField] private LayerMask scanLayer;             // Слой объектов, по которым сканировать

    private float time;
    private List<Vector3> hitPoints = new();

    void Update()
    {
        time += Time.deltaTime;

        // лучевой угол (вращение в плоскости XY)
        float angle = time * rotationSpeed;
        Vector3 direction = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f); // вращение в плоскости XY

        // передаём параметры в шейдер
        sonarMaterial.SetFloat("_TimeValue", time);

        sonarMaterial.SetFloat("_RotationSpeed", rotationSpeed);
        sonarMaterial.SetFloat("_LineWidth", lineWidth);
        sonarMaterial.SetVector("_Center", transform.position);

        // Визуализация луча
        Debug.DrawRay(transform.position, direction * scanDistance, Color.green);

        // сканируем по направлению
        if (Physics.Raycast(transform.position, direction, out RaycastHit hit, scanDistance, scanLayer))
        {
            Debug.Log($"Сонар столкнулся с: {hit.collider.name} в точке {hit.point}");
            RegisterHit(hit.point);
            if (hitMarkerPrefab != null)
            {
                Destroy(Instantiate(hitMarkerPrefab, hit.point, Quaternion.identity), 2f); // вспышка остаётся 2 секунды
            }
        }

        // передаём слепки в шейдер
        sonarMaterial.SetFloat("_HitCount", hitPoints.Count);
        if (hitPoints.Count > 0)
        {
            Vector4[] arr = new Vector4[50];
            for (int i = 0; i < hitPoints.Count && i < 50; i++)
                arr[i] = hitPoints[i];
            sonarMaterial.SetVectorArray("_HitPoints", arr);
        }
    }

    public void RegisterHit(Vector3 point)
    {
        if (hitPoints.Count >= 50)
            hitPoints.RemoveAt(0);
        hitPoints.Add(point);
    }
}
