using System;
using UnityEngine;

public class SonarCamera : MonoBehaviour
{
    public Transform targetTransform;

    private void Update()
    {
        transform.position = new Vector3(targetTransform.position.x, targetTransform.position.y, transform.position.z);
    }
}
