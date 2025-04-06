using System;
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
            _audioSource.Play();
            Debug.Log(other.gameObject.tag);
        }
    }
}
