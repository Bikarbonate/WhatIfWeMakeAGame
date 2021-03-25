using Assets;
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

    public virtual void EnemyDoesDamage()
    {

    }

    public virtual void EnemyDie(IntVariable enemyCurrentHp, GameObject enemy)
    {
        if (enemyCurrentHp._value <= 0)
        {
            enemy.SetActive(false);
        }
    }

    public enum MovementBehaviour
    {
        CREEPMOVEMENT,
        SKELETONMOVEMENT
    }

    public void MovementLogic(MovementBehaviour behaviour)
    {
        switch (behaviour)
        {
            case MovementBehaviour.CREEPMOVEMENT:
                DoCreepMovement();
                break;

            case MovementBehaviour.SKELETONMOVEMENT:
                DoSkeletonMovement();
                break;
        }
    }

    private void DoCreepMovement()
    {

    }

    private void DoSkeletonMovement()
    {

    }
}
