using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyLogic : MonoBehaviour
{

    [SerializeField] private float RangeRadius;
    [SerializeField] private float lerpSpeed = 2f;
    [SerializeField] private float stopDistance = 0.05f;
    
    [SerializeField] private float minXoffset;
    [SerializeField] private float maxXoffset;
    
    [SerializeField] private float minYoffset;
    [SerializeField] private float maxYoffset;

    public bool hasAttacked = false;
    
    private Vector3 _targetPos;
    private Transform _ship;
    private bool _isSonarActive;
    
    private bool hasReachedTarget = true;
    
    
    void OnEnable()
    {
        SonarEvents.OnSonarActivated += CheckDistance;
    }


    private void CheckDistance(Transform ship, bool isSonarActive)
    {
        if ((Vector3.Distance(ship.position, gameObject.transform.position) < RangeRadius)&&!hasAttacked)
        {
            _isSonarActive = isSonarActive;
            _targetPos = ship.position;
        }
    }


    private void Update()
    {
        if (hasReachedTarget)
        {
            ChangePos();
        }
        
        transform.position = Vector3.Lerp(transform.position, _targetPos, lerpSpeed * Time.deltaTime);
        
        float distance = Vector3.Distance(transform.position, _targetPos);
        if (distance < stopDistance)
        {
            hasReachedTarget = true;
            hasAttacked = false;
        }
        else
        {
            hasReachedTarget = false;
        }
        
    }

    public void ChangePos()
    {
        _targetPos = new Vector3(x: Random.Range(minXoffset, maxXoffset), y: Random.Range(minYoffset, maxYoffset));
    }
    

    private void OnDisable()
    {
        SonarEvents.OnSonarActivated -= CheckDistance;
    }
}
