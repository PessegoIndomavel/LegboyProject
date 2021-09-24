using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerAnimation : MonoBehaviour
{
    /*public float maxScale = 1.5f;*/
    public float xVelScaleMult = 0.1f;
    public float yVelScaleMultiplier = 0.3f;
    /*public float xAccScaleMult = 0.0015f;
    public float yAccScaleMult = 0.0015f;*/
    
    private Animator anim;
    private PlayerMovement move;
    private Collision coll;
    [HideInInspector]
    public SpriteRenderer sr;

    private Rigidbody2D rb;
    private Transform myTransform;

    private Vector2 acceleration, velocity, lastVelocity = Vector2.zero, lastPos = Vector2.zero;

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

    void Start()
    {
        anim = GetComponent<Animator>();
        coll = GetComponentInParent<Collision>();
        move = GetComponentInParent<PlayerMovement>();
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponentInParent<Rigidbody2D>();
        myTransform = transform;
        lastPos = myTransform.position.AsVector2();
    }

    void Update()
    {
        anim.SetBool(Walking, move.walking);
        anim.SetBool(ONGround, coll.onGround);
        anim.SetBool(ONWall, coll.onWall);
        anim.SetBool(ONRightWall, coll.onRightWall);
        anim.SetBool(NormalWallrun, move.normalWallrun);
        anim.SetBool(CanMove, move.canMove);
        anim.SetBool(BackWallrun, move.backWallrun);
        anim.SetBool(Dead, LifeManager.instance.isDead);
        
        StartCoroutine(CalculateVelAcc());
        VerticalSquish();
        
    }

    private void FixedUpdate()
    {
        velocity = (myTransform.position.AsVector2() - lastPos) / Time.fixedDeltaTime;
        acceleration = (velocity - lastVelocity) / Time.fixedDeltaTime;
        /*if ((acceleration.x > 0f && acceleration < 0.2f) || (acceleration < 0f && acceleration > -0.2f))
            acceleration = 0f;#1#*/
        /*print(acceleration);*/
        lastPos = myTransform.position.AsVector2();
        lastVelocity = velocity;
    }

    IEnumerator CalculateVelAcc()
    {
        yield return new WaitForSeconds(0.1f);
        velocity = (myTransform.position.AsVector2() - lastPos) / 0.1f;
        acceleration = (velocity - lastVelocity) / 0.1f;
        lastPos = myTransform.position.AsVector2();
        lastVelocity = velocity;
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
        sr.flipX = (side != 1);
    }

    private void VerticalSquish()
    {
        //velocity squish
        var scale = new Vector2(1f - Mathf.Abs(rb.velocity.y * xVelScaleMult) + Mathf.Abs(rb.velocity.x * xVelScaleMult),
            Mathf.Abs(rb.velocity.y * yVelScaleMultiplier) + 1f - Mathf.Abs(rb.velocity.x * yVelScaleMultiplier));
        /*Vector2 scale = new Vector2(0f,0f);
        //acceleration squish
        scale += new Vector2(Mathf.Clamp( 1f + Mathf.Abs(acceleration.x * xAccScaleMult) - Mathf.Abs(acceleration.y * yAccScaleMult), 0f, maxScale),
            Mathf.Clamp(1f + Mathf.Abs(acceleration.y * yVelScaleMultiplier) - Mathf.Abs(acceleration.x * xAccScaleMult), 0f, maxScale));*/
        
        var yPos = (scale.y - 1) / 2f;

        float xPos;
        if (move.currentSpeed > 0f) xPos = -(scale.x - 1) / 2f;
        else xPos = (scale.x - 1) / 2f;
        
        myTransform.localScale = scale;
        myTransform.localPosition = new Vector2(xPos, yPos);
    }
}