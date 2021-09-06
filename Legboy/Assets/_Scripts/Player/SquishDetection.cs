using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquishDetection : MonoBehaviour
{
    public LayerMask groundLayer;
    public float radius;
    public Vector2 up, down, left, right;
    public bool debugVisible = true;
    public Color debugColor;

    private Vector2 pos;
    private Transform myTransform;

    private void Awake()
    {
        myTransform = GetComponent<Transform>();
    }

    private void FixedUpdate()
    {
        pos = myTransform.position;
        var upCol = Physics2D.OverlapCircle(pos + up, radius, groundLayer);
        var downCol = Physics2D.OverlapCircle(pos + down, radius, groundLayer);
        var leftCol = Physics2D.OverlapCircle(pos + left, radius, groundLayer);
        var rightCol = Physics2D.OverlapCircle(pos + right, radius, groundLayer);

        MovingPlatform m;
        if((upCol && downCol) && (upCol.TryGetComponent(out m) || downCol.TryGetComponent(out m))) LifeManager.instance.Die();
        else if((leftCol && rightCol) && (leftCol.TryGetComponent(out m) || rightCol.TryGetComponent(out m))) LifeManager.instance.Die();
    }

    private void OnDrawGizmos()
    {
        if (!debugVisible) return;
        Gizmos.color = debugColor;
        var pos = (Vector2)transform.position;
        
        Gizmos.DrawWireSphere(pos  + up, radius);
        Gizmos.DrawWireSphere(pos  + down, radius);
        Gizmos.DrawWireSphere(pos  + left, radius);
        Gizmos.DrawWireSphere(pos  + right, radius);
    }
}
