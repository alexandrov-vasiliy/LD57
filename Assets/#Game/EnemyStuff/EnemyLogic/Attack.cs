using System;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public static event Action OnAttack;
    
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<SonarController>())
        {
            OnAttack?.Invoke();
            gameObject.GetComponent<AudioSource>().Play();
            gameObject.GetComponent<EnemyLogic>().hasAttacked = true;
            gameObject.GetComponent<EnemyLogic>().ChangePos();

        }
    }
}
