using System;
using UnityEngine;

public class CollisionChecker : MonoBehaviour
{

    public static event Action OnCollision;
    
    private void OnCollisionEnter(Collision other)
    {
        
        if (other.gameObject.CompareTag("Obstacle"))
        {
            OnCollision?.Invoke();
            Debug.Log(other.gameObject.tag);
        }
    }
}
