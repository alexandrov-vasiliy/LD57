using System;
using _Game;
using UnityEngine;

public class CollisionChecker : MonoBehaviour
{
    public static event Action OnCollision;
    
    private void OnCollisionEnter(Collision other)
    {
        
        if (other.gameObject.CompareTag("Obstacle"))
        {
            OnCollision?.Invoke();
            G.sfx.PlayEffect(G.sfx.collideEffect);
            Debug.Log(other.gameObject.tag);
        }
    }
}
