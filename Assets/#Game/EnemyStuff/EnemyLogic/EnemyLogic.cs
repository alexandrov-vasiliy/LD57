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
        Debug.Log("Distance: " + Vector3.Distance(ship.position, gameObject.transform.position));
        
        if (Vector3.Distance(ship.position, gameObject.transform.position) < RangeRadius)
        {
            _isSonarActive = isSonarActive;
            _targetPos = ship.position;
        }
    }


    private void Update()
    {
        if (hasReachedTarget)
        {
            _targetPos = new Vector3(x: Random.Range(minXoffset, maxXoffset), y: Random.Range(minYoffset, maxYoffset));
        }
        
        transform.position = Vector3.Lerp(transform.position, _targetPos, lerpSpeed * Time.deltaTime);
        
        float distance = Vector3.Distance(transform.position, _targetPos);
        if (distance < stopDistance)
        {
            hasReachedTarget = true;
            //Debug.Log("Цель достигнута!");
        }
        else
        {
            hasReachedTarget = false;
        }
        
    }

    private void OnDisable()
    {
        SonarEvents.OnSonarActivated -= CheckDistance;
    }
}
