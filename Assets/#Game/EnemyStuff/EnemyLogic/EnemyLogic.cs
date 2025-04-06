using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyLogic : MonoBehaviour
{
    [Header("Quad Plane Settings")]
    [SerializeField] private Transform quadPlane;

    [Header("Movement Settings")]
    [SerializeField] private float RangeRadius;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float stopDistance = 0.05f;

    public bool hasAttacked = false;

    private Vector3 _targetPos;
    private Vector3 _planeNormal;
    private Vector3 _planeOrigin;
    private Vector3 _axis1; // ось X квадрата (ширина)
    private Vector3 _axis2; // ось Y квадрата (высота)
    private float _halfWidth;
    private float _halfHeight;

    private bool hasReachedTarget = true;

    private void Start()
    {
        if (quadPlane == null)
        {

            enabled = false;
            return;
        }

        _planeOrigin = quadPlane.position;
        _planeNormal = quadPlane.forward.normalized;

        // Оси квадрата
        _axis1 = quadPlane.right.normalized;
        _axis2 = quadPlane.up.normalized;

        // Размеры квадрата (Unity Quad = 1x1, поэтому localScale = абсолютный размер)
        _halfWidth = quadPlane.localScale.x * 0.5f;
        _halfHeight = quadPlane.localScale.y * 0.5f;
    }

    private void OnEnable()
    {
        SonarEvents.OnSonarActivated += CheckDistance;
    }

    private void OnDisable()
    {
        SonarEvents.OnSonarActivated -= CheckDistance;
    }

    private void CheckDistance(Transform ship, bool isSonarActive)
    {
        ;

        if ((Vector3.Distance(ship.position, transform.position) < RangeRadius) && !hasAttacked)
        {
            _targetPos = ship.position;
        }
    }

    private void Update()
    {
        if (hasReachedTarget)
        {
            ChangePos();
        }

        transform.position = Vector3.MoveTowards(transform.position, _targetPos, moveSpeed * Time.deltaTime);

        float distance = Vector3.Distance(transform.position, _targetPos);
        hasReachedTarget = distance < stopDistance;

        if (hasReachedTarget)
            hasAttacked = false;
    }

    public void ChangePos()
    {
        float offset1 = Random.Range(-_halfWidth, _halfWidth);
        float offset2 = Random.Range(-_halfHeight, _halfHeight);

        _targetPos = _planeOrigin + (_axis1 * offset1) + (_axis2 * offset2);
    }

    private Vector3 ProjectPointOnPlane(Vector3 point)
    {
        Vector3 fromOriginToPoint = point - _planeOrigin;
        float distance = Vector3.Dot(fromOriginToPoint, _planeNormal);
        return point - _planeNormal * distance;
    }

    private Vector3 ClampToQuad(Vector3 point)
    {
        Vector3 local = point - _planeOrigin;

        float x = Vector3.Dot(local, _axis1);
        float y = Vector3.Dot(local, _axis2);

        x = Mathf.Clamp(x, -_halfWidth, _halfWidth);
        y = Mathf.Clamp(y, -_halfHeight, _halfHeight);

        return _planeOrigin + (_axis1 * x) + (_axis2 * y);
    }
}

