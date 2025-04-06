using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private int _hpCount;
     public int HP => _hpCount;

    public static event Action OnDeath;

    private void OnEnable()
    {
        Attack.OnAttack += Damage;
    }

    private void Damage()
    {
        _hpCount -= 1;

        if (_hpCount <= 0)
        {
            OnDeath?.Invoke();
        }
    }
    


    private void OnDisable()
    {
        Attack.OnAttack -= Damage;
    }
}
