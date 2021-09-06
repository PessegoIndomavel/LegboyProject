using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collision : MonoBehaviour
{
    [Header("Layers")]
    public LayerMask groundLayer;
    public LayerMask backWallLayer;

    [Space] 
    public bool onCeiling;
    public bool onGround;
    public bool onWall;
    public bool onRightWall;
    public bool onLeftWall;
    public bool onBackWall;
    [HideInInspector] public Transform curBackWall;
    // int wallSide;
    private Transform _myTransform;

    [Space] [Header("Collision")] 
    public bool debugVisible = true;
    public float collisionRadius = 0.25f, backWallColRadius = 0.4f, groundDetectDistance = 0.1f;
    public Vector2 groundDetectOffsetLeft, groundDetectOffsetRight, rightOffset, leftOffset, upOffset, backWallOffset;//, bottomOffset;
    public Color debugCollisionColor = Color.red;

    // Start is called before the first frame update
    void Start()
    {
        _myTransform = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        var pos = (Vector2) _myTransform.position;
        
        onGround = Physics2D.Raycast(pos + groundDetectOffsetLeft, Vector2.down, groundDetectDistance, groundLayer) || 
                   Physics2D.Raycast(pos + groundDetectOffsetRight, Vector2.down, groundDetectDistance, groundLayer) || 
                   Physics2D.Raycast(pos + new Vector2(0f,groundDetectOffsetLeft.y), Vector2.down, groundDetectDistance, groundLayer);
        onCeiling = Physics2D.OverlapCircle(pos + upOffset, collisionRadius, groundLayer);
        
        var hit = Physics2D.OverlapCircle(pos + backWallOffset, backWallColRadius, backWallLayer);
        if (!onBackWall && hit)
            curBackWall = hit.GetComponent<Transform>();
        onBackWall = hit;
        
        onRightWall = Physics2D.OverlapCircle(pos + rightOffset, collisionRadius, groundLayer);
        onLeftWall = Physics2D.OverlapCircle(pos + leftOffset, collisionRadius, groundLayer);
        onWall = onRightWall || onLeftWall;
    }

    private void OnDrawGizmos()
    {
        if (!debugVisible) return;
        Gizmos.color = debugCollisionColor;
        var pos = (Vector2)transform.position;
        
        Gizmos.DrawLine(pos + groundDetectOffsetLeft, pos + groundDetectOffsetLeft + Vector2.down*groundDetectDistance);
        Gizmos.DrawLine(pos + groundDetectOffsetRight, pos + groundDetectOffsetRight + Vector2.down*groundDetectDistance);
        Gizmos.DrawWireSphere(pos  + upOffset, collisionRadius);
        Gizmos.DrawWireSphere(pos + rightOffset, collisionRadius);
        Gizmos.DrawWireSphere(pos + leftOffset, collisionRadius);
        Gizmos.DrawWireSphere(pos + backWallOffset, backWallColRadius);
        Gizmos.DrawWireSphere(pos, collisionRadius);
    }
}