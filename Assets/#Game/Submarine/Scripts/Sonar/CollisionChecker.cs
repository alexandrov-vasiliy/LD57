using System;
using _Game;
using UnityEngine;

public class CollisionChecker : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;
    public static event Action OnCollision;
    
    private void OnCollisionEnter(Collision other)
    {
        
        if (other.gameObject.CompareTag("Obstacle"))
        {
            OnCollision?.Invoke();
            G.sfx.PlayEffect(G.sfx.collideEffect);
            _audioSource.Play();
            Debug.Log(other.gameObject.tag);
        }
    }
}
