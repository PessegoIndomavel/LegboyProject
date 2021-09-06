using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{

    private Animator anim;
    private PlayerMovement move;
    private Collision coll;
    [HideInInspector]
    public SpriteRenderer sr;

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
}