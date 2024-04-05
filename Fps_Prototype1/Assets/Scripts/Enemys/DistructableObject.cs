using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DistructableObject : MonoBehaviour, IDamageable
{
    [SerializeField] float _hp;
    [SerializeField] Image _enemyHPBar;
    public void Die()
    {
        Destroy(gameObject);
    }

    public void TakeDamage(float DamageToTake)
    {
        _hp = _hp - DamageToTake;
        UpdateHp(_hp);
        if (_hp <= 0)
        {
            Die();
        }
    }
     void UpdateHp(float enemyHp)
    {
        _enemyHPBar.fillAmount = enemyHp / 100.0f;
    }
}

