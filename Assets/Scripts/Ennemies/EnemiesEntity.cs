using Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemiesEntity : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private float _distance;
    [SerializeField] private Transform _groundDetection;
    [SerializeField] private Rigidbody2D _enemyRb;

    private bool _movingRight = false;
    private float _direction = 1;

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

    public virtual void DetectPlayer()
    {

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
        //transform.Translate(Vector2.right * _speed * Time.deltaTime);

        RaycastHit2D groundInfo = Physics2D.Raycast(_groundDetection.position, Vector2.down, _distance);
        
        _enemyRb.velocity = new Vector3((_direction * _speed) , _enemyRb.velocity.y);

        if (!groundInfo.collider && _movingRight)
        {
            transform.eulerAngles = new Vector3(0, -180, 0);
            _movingRight = false;
            _direction = -1;
        }
        else if (!groundInfo.collider && !_movingRight)
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
            _movingRight = true;
            _direction = 1;
        }

        
    }

    private void DoSkeletonMovement()
    {

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawRay(_groundDetection.position, Vector2.down * _distance);
    }
}
