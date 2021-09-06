using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine.Utility;
using UnityEngine;
using DG.Tweening;

public class Patrol : MonoBehaviour
{
    [Tooltip("Whether to control patrol via speed or duration.")]
    public bool useSpeed;
    public float speed = 10f;
    public float moveDuration = 2f;
    
    [Tooltip("Animation curve.")]
    public Ease moveEase = Ease.Linear;
    
    public bool flipSprite = false;
    public SpriteRenderer spriteRenderer;

    [Header("References")]
    public Transform start;
    public Transform end;

    private Transform myTransform;
    private Vector2 startPos, endPos;
    private Tween curTween;
    private bool initialFlip;

    private void Awake()
    {
        myTransform = transform;
        if (flipSprite && spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            initialFlip = spriteRenderer.flipX;
        }
        
        startPos = start.position;
        endPos = end.position;

        if (useSpeed)
        {
            float distance = Vector2.Distance(startPos,endPos);
            moveDuration = distance / speed;
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        myTransform.position = startPos;

        curTween = DOTween.Sequence()
            .Append(myTransform.DOMove(endPos, moveDuration).SetEase(moveEase)).AppendCallback(FlipSprite)
            .Append(myTransform.DOMove(startPos, moveDuration).SetEase(moveEase)).AppendCallback(FlipSprite).SetLoops(-1).SetUpdate(UpdateType.Fixed);
    }

    private void FlipSprite()
    {
        if(flipSprite && spriteRenderer) spriteRenderer.flipX = !spriteRenderer.flipX;
    }
    
    public void RestartTween()
    {
        myTransform.position = startPos;
        spriteRenderer.flipX = !initialFlip;
        curTween = DOTween.Sequence()
            .Append(myTransform.DOMove(endPos, moveDuration).SetEase(moveEase)).AppendCallback(FlipSprite)
            .Append(myTransform.DOMove(startPos, moveDuration).SetEase(moveEase)).AppendCallback(FlipSprite).SetLoops(-1).SetUpdate(UpdateType.Fixed);
    }
    
    public void StopTween()
    {
        curTween.Kill();
    }

    void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(start.position, 0.5f);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(end.position, 0.5f);
        }
        else
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(startPos, 0.5f);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(endPos, 0.5f);
        }
    }
}
