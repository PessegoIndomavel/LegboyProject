using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerAnimation : MonoBehaviour
{
    public float xVelScaleMult = 0.1f;
    public float yVelScaleMultiplier = 0.3f;
    
    public float jumpParticlesTime = 0.05f;
    
    [SerializeField]
    private GameObject sweatGO;
    
    [Header("Particle Systems")]
    public ParticleSystem landingParticles;
    public ParticleSystem wallrunParticles;
    public ParticleSystem jumpingParticles;
    public ParticleSystem dyingParticles;
    
    [HideInInspector]
    public SpriteRenderer sr;

    private PlayerBrain brain;
    private PlayerWallRun wr;
    private SpriteRenderer sweatSr;
    private Animator anim;
    public Animator sweatAnim;
    private PlayerMovement move;
    private PlayerCollision coll;
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

    private void Start()
    {
        brain = GetComponentInParent<PlayerBrain>();
        wr = GetComponentInParent<PlayerWallRun>();
        anim = GetComponent<Animator>();
        coll = GetComponentInParent<PlayerCollision>();
        move = GetComponentInParent<PlayerMovement>();
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponentInParent<Rigidbody2D>();
        sweatAnim = sweatGO.GetComponent<Animator>();
        sweatSr = sweatGO.GetComponent<SpriteRenderer>();
        myTransform = transform;

        brain.onDie += OnDie;
        move.onJump += OnJump;
    }

    private void Update()
    {
        anim.SetBool(Walking, move.walking);
        anim.SetBool(ONGround, coll.onGround);
        anim.SetBool(ONWall, coll.onWall);
        anim.SetBool(ONRightWall, coll.onRightWall);
        anim.SetBool(NormalWallrun, wr.normalWallrun);
        anim.SetBool(CanMove, move.canMove);
        anim.SetBool(BackWallrun, wr.backWallrun);
        //anim.SetBool(BwrEndPause, move.bwrEndPause);
        anim.SetBool(Dead, LifeManager.instance.isDead);
        
        if(!wr.normalWallrun) VelocitySquish();
        
        if (wr.normalWallrun || wr.backWallrun) return;
        SetLookDirection(rb.velocity.x);
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

    /*public void Sweat(bool value)
    {
        sweatAnim.gameObject.SetActive(value);
        if(value) sweatAnim.Play("sweatAnim");
    }*/

    private void OnDie()
    {
        StopAllDistanceParticles();
        dyingParticles.Play();
    }

    private void OnJump()
    {
        jumpingParticles.Play();
        StartCoroutine(StopJumpParticlesAfterSeconds(jumpParticlesTime));
        anim.SetTrigger("Jump");
    }
    
    public void SetLookDirection(float xDir)
    {
        if(xDir > 0)
        {
            move.side = 1;
            Flip(move.side);
            TabletFollowPoint.instance.FlipPos(-move.side);
        } else if (xDir < 0)
        {
            move.side = -1;
            Flip(move.side);
            TabletFollowPoint.instance.FlipPos(-move.side);
        }
    }

    private IEnumerator StopJumpParticlesAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        jumpingParticles.Stop();
    }

    public void StopAllDistanceParticles()
    {
        jumpingParticles.Stop();
        wallrunParticles.Stop();
    }
}