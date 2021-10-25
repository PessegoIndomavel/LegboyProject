using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWallRun : MonoBehaviour
{
    [Header("Wallrun")]
    public float wallRunSpeed = 10;
    public float wallRunTime = 2f;
    [Tooltip("Max amount of time in seconds between the player pressing the wallrun command and the wallrun being possible to perform.")]
    public float wallRunBufferTime = 0.3f;
    
    [Header("Frontal Wall Run")] 
    [Range(0, 1)]
    [Tooltip("Legboy starts sweating after wallrunTime*percToSweat seconds when frontal wall running.")]
    public float percentToSweat = 0.5f;
    
    [Header("Back Wall Run")]
    public float bwrJumpCoyoteTime = 0.08f;
    //public float bwrEndPauseTime = 2f;
    
    [Header("Wall Jump")]
    public float afterWJGravity = 0.1f;

    [Header("Frontal Wall Jump")]
    public float fWallJumpMultiplier = 1f;
    public float fWallJumpForceDuration = 0.5f;
    [Range(0, 90)]
    public float fWallJumpAngle = 45f;

    [Header("Horizontal Back Wall Jump")]
    public float horBWallJumpMultiplier = 1f;
    public float horBWallJumpForceDuration = 0.5f;
    [Range(0, 90)]
    public float horBWallJumpAngle = 45f;
    
    [Header("Vertical Back Wall Jump")]
    public float vertBWallJumpMultiplier = 1f;
    public float vertBWallJumpForceDuration = 0.5f;
    [Range(0, 90)]
    public float vertBWallJumpAngle = 45f;
    
    [HideInInspector]
    public bool wallJumped;
    [HideInInspector]
    public bool normalWallrun;
    [HideInInspector] 
    public bool backWallrun;
    [HideInInspector]
    public float wallJumpLerp = 10;
    
    private bool canBWRJump;
    private bool triedBWRjump;
    private bool triedToWallrun;
    private int lastWallrunSide = -1; //-1 == null; 0 == left; 1 == right; 2 == back
    private float curWallrunTime;
    private Vector2 backWallrunDir = Vector2.up;
    private IEnumerator wallrunCoroutine;
    private Coroutine wallrunBufferCoroutine, bwrBufferCoroutine, sweatCoroutine;
    private Transform lastBackWall;
    

    private PlayerBrain brain;
    private PlayerCollision coll;
    private PlayerMovement move;
    private PlayerAnimation anim;
    private Rigidbody2D rb;

    private void Awake()
    {
        wallrunCoroutine = WallrunTimer();

        brain = GetComponent<PlayerBrain>();
        coll = GetComponent<PlayerCollision>();
        move = GetComponent<PlayerMovement>();
        anim = GetComponentInChildren<PlayerAnimation>();
        rb = GetComponent<Rigidbody2D>();

        brain.onDie += OnDie;
    }

    // Start is called before the first frame update
    void Start()
    {
        ControlsManager.instance.controlInput.Legboy.Wallrun.performed += WallrunHandler;
        ControlsManager.instance.controlInput.Legboy.Wallrun.Enable();

        brain.onDisableControls += DisableWRControls;
        brain.onEnableControls += EnableWRControls;
        coll.onGroundTouch += GroundTouch;
    }

    private void OnDisable()
    {
        ControlsManager.instance.controlInput.Legboy.Wallrun.performed -= WallrunHandler;
        ControlsManager.instance.controlInput.Legboy.Wallrun.Disable();
    }

    private void DisableWRControls()
    {
        ControlsManager.instance.controlInput.Legboy.Wallrun.Disable();
    }

    private void EnableWRControls()
    {
        ControlsManager.instance.controlInput.Legboy.Wallrun.Enable();
    }

    // Update is called once per frame
    private void Update()
    {
        if (GameStateManager.instance.getGameState() == GameState.Paused) return;

        if (triedToWallrun && !(normalWallrun || backWallrun))
        {
            if (coll.onWall) StartFrontalWallrun();
            else if (coll.onBackWall)
            {
                if (move.movInput.y > 0f) StartBackWallrun(Vector2.up);
                else if (move.movInput.x != 0f) StartBackWallrun(new Vector2(move.movInput.x, 0f));
            }
        }
        else
        {
            if (normalWallrun) FrontalWallrun();
            else if (backWallrun) BackWallrun();
        }

        //back wall run jump
        if (triedBWRjump && canBWRJump)
        {
            if (backWallrunDir != Vector2.up) //horizontal back wall run
            {
                WallJump();
                triedBWRjump = false;
            }
            else
            {
                if (triedBWRjump && move.movInput.x != 0f) //vertical back wall run
                {
                    WallJump();
                    triedBWRjump = false;
                }
            }
        }

        if (coll.onGround)
        {
            canBWRJump = false;
            wallJumped = false;
        }
    }
    
    #region Wallrun
        private void FrontalWallrun()
    {
        if(!coll.onWall || coll.onGround) CancelWallrun();

        WallrunTimeCount();
        
        rb.velocity = new Vector2(0f, wallRunSpeed);
    }

    private void BackWallrun()
    {
        if (!backWallrun) return;
         //if back wall ended or hit ceiling)
         if (!coll.onBackWall)
         {
             lastWallrunSide = -1;
             CancelWallrun();
         }
         else if (coll.onCeiling)
         {
             CancelWallrun();
             rb.velocity = Vector2.zero;
         }
         
         if (!backWallrun) return;
         
         WallrunTimeCount();
         rb.velocity = backWallrunDir == Vector2.up ? new Vector2(0f, wallRunSpeed) : new Vector2(wallRunSpeed * backWallrunDir.x, 0f);
    }

    private void StartFrontalWallrun()
    {
        if (coll.onGround)
        {
            CancelWallrun();
            return;
        }
        if (coll.onLeftWall)
        {
            if (lastWallrunSide == 0)
            {
                CancelWallrun();
                return;
            }
            lastWallrunSide = 0;
            anim.Flip(-1);
            TabletFollowPoint.instance.FlipPos(1);
        }
        else if (coll.onRightWall)
        {
            if (lastWallrunSide == 1)
            {
                CancelWallrun();
                return;
            }
            lastWallrunSide = 1;
            anim.Flip(1);
            TabletFollowPoint.instance.FlipPos(-1);
        }
        
        anim.SetLookDirection(lastWallrunSide - 0.5f);
        
        lastBackWall = null;
        triedToWallrun = false;
        rb.gravityScale = 0f;
        normalWallrun = true;
        
        FrontalWallrun();
        
        anim.SetTrigger("StartNormalWallrun");

        StartWallrunTimer();
        sweatCoroutine = StartCoroutine(SweatTimer(percentToSweat));
        StopCoroutine(ForceAfterJump(Vector2.one, 0f));
    }

    private void StartBackWallrun(Vector2 direction)
    {
        if (coll.onGround)
        {
            CancelWallrun();
            return;
        }
        if (coll.curBackWall == lastBackWall)
        {
            CancelWallrun();
            return;
        }

        anim.SetLookDirection(direction.x);
        
        canBWRJump = true;
        triedToWallrun = false;
        
        lastWallrunSide = 2;

        rb.gravityScale = 0;
        backWallrunDir = direction;
        lastBackWall = coll.curBackWall;
        
        backWallrun = true;
        
        BackWallrun();
        
        anim.SetTrigger("StartBackWallrun");
        
        //wallrunParticles.transform.localPosition = new Vector3(0.277f*side, wallrunParticles.transform.localPosition.y);
        anim.wallrunParticles.Play();
        StartWallrunTimer();
        StopCoroutine(ForceAfterJump(Vector2.one, 0f));
    }

    private void CancelWallrun()
    {
        if(backWallrun) StartCoroutine(BWRJumpCoyoteTime(bwrJumpCoyoteTime));
        rb.gravityScale = move.initialGravityScale;
        normalWallrun = false;
        backWallrun = false;
        anim.wallrunParticles.Stop();
        StopSweat();
        StopCoroutine(wallrunCoroutine);
        ResetWallrunTimeCount();
        Invoke("Exclamation", 0.2f);
    }

    private void Exclamation()
    {
        GameplayUIManager.instance.Exclamation(false);
    }

    private void StartWallrunTimer()
    {
        StopCoroutine(wallrunCoroutine);
        wallrunCoroutine = WallrunTimer();
        StartCoroutine(wallrunCoroutine);
        ResetWallrunTimeCount();
    }
    
    private void WallJump()
    {
        StopCoroutine(DisableMovement(0));
        StartCoroutine(DisableMovement(.2f));
        Vector2 jumpDir = Vector2.zero;
        float angle, multiplier, forceDuration;

        if (!canBWRJump)
        {
            if ((move.side == 1 && coll.onRightWall) || move.side == -1 && !coll.onRightWall)
            {
                move.side *= -1;
                anim.Flip(move.side);
            }

            jumpDir = coll.onRightWall ? Vector2.left : Vector2.right;
            angle = fWallJumpAngle;
            multiplier = fWallJumpMultiplier;
            forceDuration = fWallJumpForceDuration;
        } else
        {
            if (backWallrunDir == Vector2.up)
            {
                //vertical back wallrun
                jumpDir = new Vector2(move.movInput.x, 0f);

                angle = vertBWallJumpAngle;
                multiplier = vertBWallJumpMultiplier;
                forceDuration = vertBWallJumpForceDuration;
            }
            else
            {
                //horizontal back wallrun
                jumpDir = new Vector2(backWallrunDir.x, 0f);

                angle = horBWallJumpAngle;
                multiplier = horBWallJumpMultiplier;
                forceDuration = horBWallJumpForceDuration;
            }
            canBWRJump = false;
        }

        var jumpForce = new Vector2(Mathf.Cos(Mathf.Deg2Rad * angle) * jumpDir.x, Mathf.Sin(Mathf.Deg2Rad * angle)) * multiplier;

        CancelWallrun();
        rb.velocity = Vector2.zero;
        rb.gravityScale = move.initialGravityScale;
        
        move.Jump(jumpForce, true);
        StartCoroutine(ForceAfterJump(jumpForce, forceDuration));
        wallJumped = true;
    }
    
    private IEnumerator ForceAfterJump(Vector2 dir, float forceDuration)
    {
        var curTime = forceDuration;
        dir *= move.jumpForce;
        
        while (curTime > 0f)
        {
            rb.velocity = new Vector2(dir.x, rb.velocity.y - afterWJGravity);
            
            curTime -= Time.deltaTime;
            yield return null;
        }
    }
    
    private void Sweat(bool value)
    {
        anim.sweatAnim.gameObject.SetActive(value);
        if(value) anim.sweatAnim.Play("sweatAnim");
    }

    private IEnumerator SweatTimer(float timePercentage)
    {
        yield return new WaitForSeconds(wallRunTime*timePercentage);
        Sweat(true);
    }

    private void StopSweat()
    {
        if(sweatCoroutine != null) StopCoroutine(sweatCoroutine); 
        Sweat(false);
    }

    private void ResetWallrunTimeCount()
    {
        curWallrunTime = wallRunTime;
        GameplayUIManager.instance.UpdateWallRunBarFill(curWallrunTime/wallRunTime);
    }

    private void WallrunTimeCount()
    {
        curWallrunTime -= Time.deltaTime;
        if(curWallrunTime < wallRunTime*0.1f) GameplayUIManager.instance.Exclamation(true);
        GameplayUIManager.instance.UpdateWallRunBarFill(curWallrunTime/wallRunTime);
    }

    private IEnumerator BWRJumpCoyoteTime(float cTime)
    {
        yield return new WaitForSeconds(cTime);
        if(!backWallrun) canBWRJump = false;
    }

    private IEnumerator WallrunTimer()
    {
        yield return new WaitForSeconds(wallRunTime);
        CancelWallrun();
    }

    //input buffer
    IEnumerator TriedToWallrun()
    {
        triedToWallrun = true;
        yield return new WaitForSeconds(wallRunBufferTime);
        triedToWallrun = false;
    }

    //back wallrun jump buffer
    IEnumerator TriedToBWRJump()
    {
        triedBWRjump = true;
        yield return new WaitForSeconds(wallRunBufferTime);
        triedBWRjump = false;
    }
    
    private IEnumerator DisableMovement(float time)
    {
        move.canMove = false;
        yield return new WaitForSeconds(time);
        move.canMove = true;
    }
    
    #endregion

    #region Actions
    private void GroundTouch()
    {
        canBWRJump = false;
        lastBackWall = null;
        lastWallrunSide = -1;
        StopCoroutine(ForceAfterJump(Vector2.one, 0f));
    }

    private void OnDie()
    {
        triedToWallrun = false;
        triedBWRjump = false;
    }
    #endregion
    
    #region Input Handlers
    
    void WallrunHandler(InputAction.CallbackContext callbackContext)
    {
        if (GameStateManager.instance.isPaused()) return;
        if (coll.onGround) return;
        if (backWallrun || normalWallrun)
        {
            CancelWallrun();
            return;
        }
        if(wallrunBufferCoroutine != null) StopCoroutine(wallrunBufferCoroutine);
        wallrunBufferCoroutine = StartCoroutine(TriedToWallrun());
    }

    public bool OnJumpKeyPress()
    {
        if (coll.onWall && !coll.onGround)
        {
            WallJump();
            return true;
        }

        if (!canBWRJump) return false;
        
        if(bwrBufferCoroutine != null) StopCoroutine(bwrBufferCoroutine);
        bwrBufferCoroutine = StartCoroutine(TriedToBWRJump());
        return true;
    }
    
    #endregion
}
