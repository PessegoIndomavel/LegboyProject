using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerAnimation : MonoBehaviour
{
    public float xVelScaleMult = 0.1f;
    public float yVelScaleMultiplier = 0.3f;
    [SerializeField]
    private GameObject sweatGO;
    [HideInInspector]
    public SpriteRenderer sr;

    private SpriteRenderer sweatSr;
    private Animator anim;
    private Animator sweatAnim;
    private PlayerMovement move;
    private Collision coll;
    private Rigidbody2D rb;
    private Transform myTransform;

    private static readonly int ONGround = Animator.StringToHash("onGround");
    private static readonly int ONWall = Animator.StringToHash("onWall");
    private static readonly int ONRightWall = Animator.StringToHash("onRightWall");
    private static readonly int NormalWallrun = Animator.StringToHash("normalWallrun");
    private static readonly int BackWallrun = Animator.StringToHash("backWallrun");
    private static readonly int CanMove = Animator.StringToHash("canMove");
    private static readonly int HorizontalAxis = Animator.StringToHash("HorizontalAxis");
    private static readonly int VerticalAxis = Animator.StringToHash("VerticalAxis");
    private static readonly int VerticalVelocity = Animator.StringToHash("VerticalVelocity");
    private static readonly int Walking = Animator.StringToHash("walking");
    private static readonly int Dead = Animator.StringToHash("dead");
    private static readonly int HorizontalVelocity = Animator.StringToHash("HorizontalVelocity");
    private static readonly int BwrEndPause = Animator.StringToHash("bwrEndPause");

    void Start()
    {
        anim = GetComponent<Animator>();
        coll = GetComponentInParent<Collision>();
        move = GetComponentInParent<PlayerMovement>();
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponentInParent<Rigidbody2D>();
        sweatAnim = sweatGO.GetComponent<Animator>();
        sweatSr = sweatGO.GetComponent<SpriteRenderer>();
        myTransform = transform;
    }

    private void Update()
    {
        anim.SetBool(Walking, move.walking);
        anim.SetBool(ONGround, coll.onGround);
        anim.SetBool(ONWall, coll.onWall);
        anim.SetBool(ONRightWall, coll.onRightWall);
        anim.SetBool(NormalWallrun, move.normalWallrun);
        anim.SetBool(CanMove, move.canMove);
        anim.SetBool(BackWallrun, move.backWallrun);
        anim.SetBool(BwrEndPause, move.bwrEndPause);
        anim.SetBool(Dead, LifeManager.instance.isDead);
        
        if(!move.normalWallrun) VelocitySquish();
    }

    public void SetMovementVars(float x,float y, float xVel, float yVel)
    {
        anim.SetFloat(HorizontalAxis, x);
        anim.SetFloat(VerticalAxis, y);
        anim.SetFloat(HorizontalVelocity, xVel);
        anim.SetFloat(VerticalVelocity, yVel);
    }

    public void SetTrigger(string trigger)
    {
        anim.SetTrigger(trigger);
    }

    public void Flip(int side)
    {
        sweatSr.flipX = sr.flipX = (side != 1);
    }

    private void VelocitySquish()
    {
        //scale
        var scale = new Vector2(1f - Mathf.Abs(rb.velocity.y * xVelScaleMult) + Mathf.Abs(rb.velocity.x * xVelScaleMult),
            Mathf.Abs(rb.velocity.y * yVelScaleMultiplier) + 1f - Mathf.Abs(rb.velocity.x * yVelScaleMultiplier));
        
        //position offsets
        var yPos = (scale.y - 1) / 2f;
        float xPos;
        if (move.currentSpeed > 0f) xPos = -(scale.x - 1) / 2f;
        else xPos = (scale.x - 1) / 2f;
        
        myTransform.localScale = scale;
        myTransform.localPosition = new Vector2(xPos, yPos);
    }

    public void Sweat(bool value)
    {
        sweatAnim.gameObject.SetActive(value);
        if(value) sweatAnim.Play("sweatAnim");
    }
    
}