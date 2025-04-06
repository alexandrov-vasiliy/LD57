using System;
using _Game;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public static event Action OnAttack;
    
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<SonarController>())
        {
            OnAttack?.Invoke();
            G.sfx.PlayEffect(G.sfx.attackEffect, 0.4f);
            gameObject.GetComponent<EnemyLogic>().hasAttacked = true;
            gameObject.GetComponent<EnemyLogic>().ChangePos();

        }
    }
}
