using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtBox : MonoBehaviour
{
    public Collider2D[] _overlapHitBuffer = new Collider2D[1];

    #region Show In Inspector
    [SerializeField] Transform _topLeftPosition;
    [SerializeField] Transform _bottomRightPosition;
    [SerializeField] LayerMask _whatIsPunchable;
    #endregion
    private void OnDrawGizmos()
    {
        //Gizmos.color = _color;
        //Gizmos.DrawRay(_groundCheckTransform.position, Vector2.down * _checkDistance);
        Gizmos.color = Color.red;

        Vector2 topLeft = _topLeftPosition.position;
        Vector2 topRight = new Vector2(_topLeftPosition.position.x, _bottomRightPosition.position.y);
        Vector2 bottomLeft = new Vector2(_bottomRightPosition.position.x, _topLeftPosition.position.y);
        Vector2 bottomRight = _bottomRightPosition.position;

        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
        Gizmos.DrawLine(bottomLeft, topLeft);
    }

    #region Public Methods

    public bool ICanPunchThat()
    {
        int colliderCount = Physics2D.OverlapAreaNonAlloc(_topLeftPosition.position, _bottomRightPosition.position, _overlapHitBuffer, _whatIsPunchable);

        return colliderCount > 0;
    }

    #endregion
}
