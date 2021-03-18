using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreepBehaviour : MonoBehaviour
{
    #region Show In Inspector

    [SerializeField] private Transform _wayPoint1;
    [SerializeField] private Transform _wayPoint2;
    [SerializeField] private Transform _startPos;
    [SerializeField] private Transform _playerPos;
    [SerializeField] private float _creepSpeed;

    [Header("PlayerDetection")]
    [SerializeField] private RaycastHit2D[] _detectionBuffer = new RaycastHit2D[1];
    [SerializeField] private float _detectionDistance;
    [SerializeField] private LayerMask _whatIsPlayer;

    [Header("Guizmos Drawing")]
    [SerializeField] private Color _guizmoColor;

    #endregion

    private float _enemyDir;
    private Vector2 _nextPos;
    private Transform _enemyTransform;
    private Rigidbody2D _enemyRb;
    private Vector2 _enemyMovement;

    #region Unity Lifecycle

    private void Awake()
    {
        _enemyDir = 1f;
        _enemyTransform = GetComponent<Transform>();
        _enemyRb = GetComponent<Rigidbody2D>();
        _nextPos = _wayPoint1.position;
        _enemyMovement = new Vector2(1, 0) * _creepSpeed;
    }

    private void Start()
    {

    }

    private void Update()
    {
        DetectPlayer();
        FlipSprite();
        EnemiesWandering();
    }

    #endregion

    #region Private Methods

    private bool DetectPlayer()
    {
        int hitCount = Physics2D.RaycastNonAlloc(transform.position, transform.right, _detectionBuffer, _detectionDistance, _whatIsPlayer);
        return hitCount > 0;
    }

    private void FlipSprite()
    {
    }

    private void EnemiesWandering()
    {
        if (!DetectPlayer())
        {
            if (VectorComparaison.VectorComp(transform.position, _wayPoint1.position))
            {
                _enemyMovement = Vector2.one * -_creepSpeed;
            }

            if (VectorComparaison.VectorComp(transform.position, _wayPoint2.position))
            {
                _enemyMovement = Vector2.one * _creepSpeed;
            }

            _enemyRb.velocity = _enemyMovement;
        }
    }

    #endregion

    #region Draw Gizmos

    private void OnDrawGizmos()
    {
        Gizmos.color = _guizmoColor;
        Gizmos.DrawRay(transform.position, transform.right * _detectionDistance);
    }

    #endregion
}
