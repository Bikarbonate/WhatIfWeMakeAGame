using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemiesEntity : MonoBehaviour
{

    public virtual void EnemySetHP(IntVariable enemyStartHp, IntVariable enemyCurrentHp)
    {
        enemyCurrentHp._value = enemyStartHp._value;
    }

    public virtual void EnemyTakingDamage(IntVariable enemyCurrentHp)
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            enemyCurrentHp._value--;
        }
    }

    public virtual void EnemyDie(IntVariable enemyCurrentHp, GameObject enemy)
    {
        if (enemyCurrentHp._value <= 0)
        {
            enemy.SetActive(false);
        }
    }
}
