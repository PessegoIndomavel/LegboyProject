using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    
    #region Components
    
        private Collision coll;
        private Rigidbody2D rb;
        private PlayerAnimation anim;
        private PlayerJump jump;
        private PlayerSounds sound;
        private Collider2D myColl;
       
        
    #endregion

    [Header("Stats")]
    public float maxSpeed = 10;
    public float acceleration = 20f;
    public float airAcceleration = 10f;
    public float jumpForce = 50f;
    public float jumpCoyoteTime = 0.08f;
    public float jumpBufferTime = 0.1f;
    public float enemyHeadJumpForce = 30f;
    [Tooltip("Max velocity to be considered stopped.")]
    public float stoppedThreshold = 0.05f;
    public float jumpParticlesTime = 0.05f;
    public float fallTimeToBigFall = 2f;
    public float bigFallScreenShakeAmplitude = 0.5f;
    public float bigFallScreenShakeFrequency = 0.1f;
    public float bigFallScreenShakeDuration = 0.2f;
    
    [Header("Wall Jump")]
    public float afterWJGravity = 0.1f;

    [Header("Frontal Wall Run")] 
    [Range(0, 1)]
    [Tooltip("Legboy starts sweating after wallrunTime*percToSweat seconds when frontal wall running.")]
    public float percentToSweat = 0.5f;
           
    [Header("Frontal Wall Jump")]
    public float fWallJumpMultiplier = 1f;
    public float fWallJumpForceDuration = 0.5f;
    [Range(0, 90)]
    public float fWallJumpAngle = 45f;

    
    [Header("Back Wall Run")]
    public float bwrJumpCoyoteTime = 0.08f;
    //public float bwrEndPauseTime = 2f;
    
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
    
    [Header("Wallrun")]
    public float wallRunSpeed = 10;
    public float wallRunTime = 2f;
    [Tooltip("Max amount of time in seconds between the player pressing the wallrun command and the wallrun being possible to perform.")]
    public float wallRunBufferTime = 0.3f;
    
    [Header("References")]
    public ParticleSystem landingParticles;
    public ParticleSystem wallrunParticles;
    public ParticleSystem jumpingParticles;
    public ParticleSystem dyingParticles;
    
    [HideInInspector]
    public float wallJumpLerp = 10;
    [HideInInspector] 
    public bool walking;
    [HideInInspector]
    public bool canMove = true;
    [HideInInspector]
    public bool jumpHold;
    [HideInInspector]
    public bool wallGrab;
    [HideInInspector]
    public bool wallJumped;
    [HideInInspector]
    public bool normalWallrun;
    [HideInInspector] 
    public bool backWallrun;
    [HideInInspector] 
    public bool bwrEndPause;
    [HideInInspector]
    public int side = 1;
    [HideInInspector] 
    public bool falling;
    [HideInInspector]
    public float currentSpeed;

    private bool jumped;
    private bool canJump;
    private bool canBWRJump;
    private bool triedToJump;
    private bool triedBWRjump;
    private bool triedToWallrun;
    private bool groundTouch;
    private float initialGravityScale;
    private float fallingTime = 0f;
    private int lastWallrunSide = -1; //-1 == null; 0 == left; 1 == right; 2 == back
    private Vector2 backWallrunDir = Vector2.up;
    private Vector2 movInput;
    private IEnumerator wallrunCoroutine;
    private Coroutine sweatCoroutine;
    private Coroutine bwrEndPauseCoroutine;
    private Coroutine wallrunBufferCoroutine, bwrBufferCoroutine, jumpBufferCoroutine;
    private Transform lastBackWall;
    
    public Action onChangeDirection;

    private void Awake()
   {
       coll = GetComponent<Collision>();
       rb = GetComponent<Rigidbody2D>();
       anim = GetComponentInChildren<PlayerAnimation>();
       sound = GetComponentInChildren<PlayerSounds>();
       jump = GetComponent<PlayerJump>();
       myColl = GetComponent<Collider2D>();
       
       wallrunCoroutine = WallrunTimer();
       initialGravityScale = rb.gravityScale;
   }

   private void Start()
   {
       ControlsManager.instance.controlInput.Legboy.JumpNext.performed += JumpHandler;
       ControlsManager.instance.controlInput.Legboy.JumpNext.canceled += JumpButtonUp;
       ControlsManager.instance.controlInput.Legboy.JumpNext.Enable();
       
       ControlsManager.instance.controlInput.Legboy.Wallrun.performed += WallrunHandler;
       ControlsManager.instance.controlInput.Legboy.Wallrun.Enable();

       ControlsManager.instance.controlInput.Legboy.Move.canceled += MovementHandler;
       ControlsManager.instance.controlInput.Legboy.Move.performed += MovementHandler;
       ControlsManager.instance.controlInput.Legboy.Move.Enable();
   }

   private void OnDisable()
   {
       ControlsManager.instance.controlInput.Legboy.JumpNext.performed -= JumpHandler;
       ControlsManager.instance.controlInput.Legboy.JumpNext.canceled -= JumpButtonUp;
       ControlsManager.instance.controlInput.Legboy.JumpNext.Disable();
       
       ControlsManager.instance.controlInput.Legboy.Wallrun.performed -= WallrunHandler;
       ControlsManager.instance.controlInput.Legboy.Wallrun.Disable();

       ControlsManager.instance.controlInput.Legboy.Move.canceled -= MovementHandler;
       ControlsManager.instance.controlInput.Legboy.Move.performed -= MovementHandler;
       ControlsManager.instance.controlInput.Legboy.Move.Disable();
   }

   public void DisableControls()
   {
       ControlsManager.instance.controlInput.Legboy.JumpNext.Disable();
       ControlsManager.instance.controlInput.Legboy.Wallrun.Disable();
       //ControlsManager.instance.controlInput.Legboy.Move.Disable();
       canMove = false;
       currentSpeed = 0f;
       movInput = Vector2.zero;
   }

   public void EnableControls()
   {
       ControlsManager.instance.controlInput.Legboy.JumpNext.Enable();
       ControlsManager.instance.controlInput.Legboy.Wallrun.Enable();
       //ControlsManager.instance.controlInput.Legboy.Move.Enable();
       canMove = true;
   }

   // Update is called once per frame
    void Update()
    {
        if (GameStateManager.instance.getGameState() == GameState.Paused) return;
        
        Walk(movInput);
        

        //wallrun section
        if (triedToWallrun && !(normalWallrun || backWallrun))
        {
            if(coll.onWall) StartFrontalWallrun();
            else if (coll.onBackWall)
            {
                if(movInput.y > 0f) StartBackWallrun(Vector2.up);
                else if(movInput.x != 0f) StartBackWallrun(new Vector2(movInput.x, 0f));
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
            //if(bwrEndPause) CancelBWREndPause();
            if (backWallrunDir != Vector2.up) //horizontal back wall run
            {
                WallJump();
                triedBWRjump = false;
            }
            else
            {
                if (triedBWRjump && movInput.x != 0f) //vertical back wall run
                {
                    WallJump();
                    triedBWRjump = false;
                }
            }
        }

        /*if (bwrEndPause)
        {
            BWREndPause();
        }*/

        //jump
        if (triedToJump && canJump && !jumped)
        {
            Jump(Vector2.up, false);
            triedToJump = false;
        }

        if (coll.onGround && !groundTouch)
        {
            GroundTouch();
        }

        if(!coll.onGround && groundTouch)
        {
            groundTouch = false;
        }
        
        if (coll.onGround)
        {
            canBWRJump = false;
            canJump = true;
            wallJumped = false;
            jump.enabled = true;
        }
        else
        {
            if (!falling && rb.velocity.y < 0f)
            {
                falling = true;
            }
            else if (falling && (rb.velocity.y >= 0.1f || backWallrun) )
            {
                fallingTime = 0f;
                falling = false;
            }
            //falling = rb.velocity.y < 0f;
            if (canJump) StartCoroutine(JumpCoyoteTime(jumpCoyoteTime));
        }

        if (falling) Fall();

        anim.SetMovementVars(movInput.x, movInput.y, rb.velocity.x, rb.velocity.y);
        
        if (normalWallrun || backWallrun || bwrEndPause) return;
        SetLookDirection(rb.velocity.x);
    }

    private void Fall()
    {
        fallingTime += Time.deltaTime;
    }

    private void SetLookDirection(float xDir)
    {
        if(xDir > 0 /*&& side == -1 && Time.timeScale != 0f*/)
        {
            side = 1;
            anim.Flip(side);
            TabletFollowPoint.instance.FlipPos(-side);
        } else if (xDir < 0 /*&& side == 1 && Time.timeScale != 0f*/)
        {
            side = -1;
            anim.Flip(side);
            TabletFollowPoint.instance.FlipPos(-side);
        }
    }

    #region Wallrun

    private void FrontalWallrun()
    {
        if(!coll.onWall || coll.onGround) CancelWallrun();

        rb.velocity = new Vector2(0f, wallRunSpeed);
    }

    private void BackWallrun()
    {
        if (!backWallrun) return;
         //if back wall ended or hit ceiling)
         if (!coll.onBackWall)
         {
             lastWallrunSide = -1;
             //StartCoroutine(BWRJumpCoyoteTime(bwrJumpCoyoteTime));
             CancelWallrun();
         }
         else if (coll.onCeiling)
         {
             CancelWallrun();
             rb.velocity = Vector2.zero;
         }

         if (!backWallrun) return;
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
        
        SetLookDirection(lastWallrunSide - 0.5f);
        
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

        SetLookDirection(direction.x);
        
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
        wallrunParticles.Play();
        StartWallrunTimer();
        StopCoroutine(ForceAfterJump(Vector2.one, 0f));
    }

    private void CancelWallrun()
    {
        if(backWallrun) StartCoroutine(BWRJumpCoyoteTime(bwrJumpCoyoteTime));
        rb.gravityScale = initialGravityScale;
        normalWallrun = false;
        backWallrun = false;
        wallrunParticles.Stop();
        StopSweat();
        StopCoroutine(wallrunCoroutine);
    }

    /*private IEnumerator BWREndPauseTimer()
    {
        bwrEndPause = true;
        rb.gravityScale = 0f;
        
        yield return new WaitForSeconds(bwrEndPauseTime);

        /*if (!bwrEndPause) yield break;#1#
        CancelBWREndPause();
        StartCoroutine(BWRJumpCoyoteTime(bwrJumpCoyoteTime));
    }
    
    /*private void BWREndPause()
    {
        rb.velocity = Vector2.zero;
    }

    private void CancelBWREndPause()
    {
        if(bwrEndPauseCoroutine != null) StopCoroutine(bwrEndPauseCoroutine);
        rb.gravityScale = initialGravityScale;
        bwrEndPause = false;
    }*/

    private void StartWallrunTimer()
    {
        StopCoroutine(wallrunCoroutine);
        wallrunCoroutine = WallrunTimer();
        StartCoroutine(wallrunCoroutine);
    }

    private IEnumerator SweatTimer(float timePercentage)
    {
        yield return new WaitForSeconds(wallRunTime*timePercentage);
        anim.Sweat(true);
    }

    private void StopSweat()
    {
        if(sweatCoroutine != null) StopCoroutine(sweatCoroutine);
        anim.Sweat(false);
    }

    private IEnumerator WallrunTimer()
    {
        yield return new WaitForSeconds(wallRunTime);
        CancelWallrun();
        /*if (backWallrun)
        {
            CancelWallrun();
            //if(!bwrEndPause) bwrEndPauseCoroutine = StartCoroutine(BWREndPauseTimer());
        }
        else
            CancelWallrun();*/
    }

    /*IEnumerator BWREndPause(float bwrEndPauseTime)
    {
        rb.velocity = Vector2.zero;
        backWallrun = false;
        yield return new WaitForSeconds(bwrEndPauseTime);
        CancelWallrun();
    }*/
    
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
    
    #endregion

    private void GroundTouch()
    {
        side = anim.sr.flipX ? -1 : 1;

        canBWRJump = false;
        canJump = true;
        lastBackWall = null;
        lastWallrunSide = -1;
        //afterBackWallrun = false;
        StopCoroutine(ForceAfterJump(Vector2.one, 0f));
        if (falling)
        {
            landingParticles.Play();
            
            if (fallingTime >= fallTimeToBigFall)
            {
                sound.FallSound(true);
                ScreenShake.instance.ShakeScreen(bigFallScreenShakeDuration, bigFallScreenShakeAmplitude, bigFallScreenShakeFrequency);
            } else sound.FallSound(false);
            
            fallingTime = 0f;
        }
        groundTouch = true;
        falling = false;
    }

    private void WallJump()
    {
        StopCoroutine(DisableMovement(0));
        StartCoroutine(DisableMovement(.2f));
        Vector2 jumpDir = Vector2.zero;
        float angle, multiplier, forceDuration;

        if (!canBWRJump)
        {
            if ((side == 1 && coll.onRightWall) || side == -1 && !coll.onRightWall)
            {
                side *= -1;
                anim.Flip(side);
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
                jumpDir = new Vector2(movInput.x, 0f);

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
        rb.gravityScale = initialGravityScale;
        
        Jump(jumpForce, true);
        StartCoroutine(ForceAfterJump(jumpForce, forceDuration));
        wallJumped = true;
    }
    
    private void Jump(Vector2 dir, bool wall)
    { 
        canJump = false;
        jumpingParticles.Play();
        StartCoroutine(StopJumpParticlesAfterSeconds(jumpParticlesTime));
          
        rb.velocity = new Vector2(rb.velocity.x, 0);
        currentSpeed = rb.velocity.x;
        rb.velocity += dir * jumpForce;
        sound.JumpSound();
        anim.SetTrigger("Jump");
        StartCoroutine(Jumped());
    }

    public void EnemyHeadJump()
    {
        rb.velocity += Vector2.up * enemyHeadJumpForce;
    }

    private IEnumerator ForceAfterJump(Vector2 dir, float forceDuration)
    {
        var curTime = forceDuration;
        dir *= jumpForce;
        
        while (curTime > 0f)
        {
            rb.velocity = new Vector2(dir.x, rb.velocity.y - afterWJGravity);
            
            curTime -= Time.deltaTime;
            yield return null;
        }
    }

    private void Walk(Vector2 dir)
    { 
        if (!canMove)
        {
            walking = false;
            return;
        }

        if (coll.onGround)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, maxSpeed * dir.x, acceleration * Time.deltaTime);

            walking = movInput.x > stoppedThreshold || movInput.x < -stoppedThreshold;
        }
        else
        {
            currentSpeed = Mathf.Lerp(currentSpeed, maxSpeed * dir.x, airAcceleration * Time.deltaTime);
        }

        rb.velocity = !wallJumped ? new Vector2(currentSpeed, rb.velocity.y) :
            Vector2.Lerp(rb.velocity, (new Vector2(currentSpeed, rb.velocity.y)), wallJumpLerp * Time.deltaTime); 
    }

    //called by LifeManager singleton
    public void OnDie()
    {
        triedToJump = false;
        triedToWallrun = false;
        triedBWRjump = false;
        
        StopAllDistanceParticles();
        dyingParticles.Play();
        DisableControls();
    }

    private IEnumerator DisableMovement(float time)
    {
        canMove = false;
        yield return new WaitForSeconds(time);
        canMove = true;
    }

    private void StopJumpParticles()
    {
        jumpingParticles.Stop();
    }

    public void StopAllDistanceParticles()
    {
        jumpingParticles.Stop();
        wallrunParticles.Stop();
    }

    private IEnumerator StopJumpParticlesAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        StopJumpParticles();
    }

    private IEnumerator JumpCoyoteTime(float cTime)
    {
        yield return new WaitForSeconds(cTime);
        if(!coll.onGround) canJump = false;
    }
    
    private IEnumerator BWRJumpCoyoteTime(float cTime)
    {
        yield return new WaitForSeconds(cTime);
        if(!backWallrun) canBWRJump = false;
    }

    //jump buffer
    private IEnumerator TriedToJump()
    {
        triedToJump = true;
        yield return new WaitForSeconds(jumpBufferTime);
        triedToJump = false;
    }

    private IEnumerator Jumped()
    {
        jumped = true;
        yield return new WaitForSeconds(0.1f);
        jumped = false;
    }

    #region InputHandlers
    void MovementHandler(InputAction.CallbackContext callbackContext)
    {
        movInput = callbackContext.ReadValue<Vector2>().normalized;
    }

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
    
    void JumpHandler(InputAction.CallbackContext callbackContext)
    {
        if (GameStateManager.instance.isPaused() || TextBoxManager.instance.GetTextBoxActive()) return;
        jumpHold = true;

        if(coll.onWall && !coll.onGround) WallJump();
        else if (canBWRJump)
        {
            if(bwrBufferCoroutine != null) StopCoroutine(bwrBufferCoroutine);
            bwrBufferCoroutine = StartCoroutine(TriedToBWRJump());
        } else 
        {
            if(jumpBufferCoroutine != null) StopCoroutine(jumpBufferCoroutine);
            jumpBufferCoroutine = StartCoroutine(TriedToJump());
        }
    }

    void JumpButtonUp(InputAction.CallbackContext callbackContext)
    {
        jumpHold = false;
    }
    #endregion
}