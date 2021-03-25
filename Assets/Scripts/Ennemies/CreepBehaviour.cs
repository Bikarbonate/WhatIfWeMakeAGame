using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreepBehaviour : EnemiesEntity
{
    [SerializeField] IntVariable _creepStartHp;
    [SerializeField] IntVariable _creepCurrentHp;
    private void Awake()
    {
        EnemySetHP(_creepStartHp, _creepCurrentHp);                
    }

    private void Update()
    {
        EnemyTakingDamage(_creepCurrentHp);
        EnemyDie(_creepCurrentHp, gameObject);
        MovementLogic(MovementBehaviour.CREEPMOVEMENT);
    }



}
