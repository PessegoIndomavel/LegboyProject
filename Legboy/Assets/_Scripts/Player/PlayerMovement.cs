using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    
    #region Components

    private PlayerBrain brain;
    private PlayerWallRun wr;
    private PlayerCollision coll;
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
    public float fallTimeToBigFall = 2f;
    public float bigFallScreenShakeAmplitude = 0.5f;
    public float bigFallScreenShakeFrequency = 0.1f;
    public float bigFallScreenShakeDuration = 0.2f;
    
    [HideInInspector] 
    public bool walking;
    [HideInInspector]
    public bool canMove = true;
    [HideInInspector]
    public bool jumpHold;
    [HideInInspector]
    public int side = 1;
    [HideInInspector] 
    public bool falling;
    [HideInInspector]
    public float currentSpeed;

    private bool jumped;
    private bool canJump;
    private bool triedToJump;
    [HideInInspector] public float initialGravityScale;
    private float fallingTime;
    [HideInInspector] public Vector2 movInput;
    private Coroutine jumpBufferCoroutine;
    
    public Action onChangeDirection, onJump;

    private void Awake()
    {
        brain = GetComponent<PlayerBrain>();
        wr = GetComponent<PlayerWallRun>();
        coll = GetComponent<PlayerCollision>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<PlayerAnimation>();
        sound = GetComponentInChildren<PlayerSounds>();
        jump = GetComponent<PlayerJump>();
        myColl = GetComponent<Collider2D>();
         
        initialGravityScale = rb.gravityScale;
   }

   private void Start()
   {
       ControlsManager.instance.controlInput.Legboy.JumpNext.performed += JumpHandler;
       ControlsManager.instance.controlInput.Legboy.JumpNext.canceled += JumpButtonUp;
       ControlsManager.instance.controlInput.Legboy.JumpNext.Enable();

       ControlsManager.instance.controlInput.Legboy.Move.canceled += MovementHandler;
       ControlsManager.instance.controlInput.Legboy.Move.performed += MovementHandler;
       ControlsManager.instance.controlInput.Legboy.Move.Enable();

       brain.onDisableControls += DisableMovControls;
       brain.onEnableControls += EnableMovControls;
       brain.onDie += OnDie;
       coll.onGroundTouch += GroundTouch;
   }

   private void OnDisable()
   {
       ControlsManager.instance.controlInput.Legboy.JumpNext.performed -= JumpHandler;
       ControlsManager.instance.controlInput.Legboy.JumpNext.canceled -= JumpButtonUp;
       ControlsManager.instance.controlInput.Legboy.JumpNext.Disable();

       ControlsManager.instance.controlInput.Legboy.Move.canceled -= MovementHandler;
       ControlsManager.instance.controlInput.Legboy.Move.performed -= MovementHandler;
       ControlsManager.instance.controlInput.Legboy.Move.Disable();
   }

   private void DisableMovControls()
   {
       ControlsManager.instance.controlInput.Legboy.JumpNext.Disable();
       canMove = false;
       currentSpeed = 0f;
       movInput = Vector2.zero;
   }

   public void EnableMovControls()
   {
       ControlsManager.instance.controlInput.Legboy.JumpNext.Enable();
       canMove = true;
   }

   // Update is called once per frame
    void Update()
    {
        if (GameStateManager.instance.getGameState() == GameState.Paused) return;
        
        if(!wr.backWallrun && !wr.normalWallrun) Walk(movInput);

        //jump
        if (triedToJump && canJump && !jumped)
        {
            Jump(Vector2.up, false);
            triedToJump = false;
        }

        if (coll.onGround)
        {
            canJump = true;
            jump.enabled = true;
        }
        else
        {
            if (!falling && rb.velocity.y < 0f)
            {
                falling = true;
            }
            else if (falling && (rb.velocity.y >= 0.1f || wr.backWallrun) )
            {
                fallingTime = 0f;
                falling = false;
            }
            if (canJump) StartCoroutine(JumpCoyoteTime(jumpCoyoteTime));
        }

        if (falling) Fall();

        anim.SetMovementVars(movInput.x, movInput.y, rb.velocity.x, rb.velocity.y);
    }

    private void Fall()
    {
        fallingTime += Time.deltaTime;
    }

    private void GroundTouch()
    {
        side = anim.sr.flipX ? -1 : 1;
        canJump = true;
        
        if (falling)
        {
            anim.landingParticles.Play();
            
            if (fallingTime >= fallTimeToBigFall)
            {
                sound.FallSound(true);
                ScreenShake.instance.ShakeScreen(bigFallScreenShakeDuration, bigFallScreenShakeAmplitude, bigFallScreenShakeFrequency);
            } else sound.FallSound(false);
            
            fallingTime = 0f;
        }
        falling = false;
    }

    public void Jump(Vector2 dir, bool wall)
    { 
        canJump = false;
          
        rb.velocity = new Vector2(rb.velocity.x, 0);
        currentSpeed = rb.velocity.x;
        rb.velocity += dir * jumpForce;
        sound.JumpSound();
        StartCoroutine(Jumped());

        onJump();
    }

    public void EnemyHeadJump()
    {
        rb.velocity += Vector2.up * enemyHeadJumpForce;
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

        rb.velocity = !wr.wallJumped ? new Vector2(currentSpeed, rb.velocity.y) :
            Vector2.Lerp(rb.velocity, (new Vector2(currentSpeed, rb.velocity.y)), wr.wallJumpLerp * Time.deltaTime); 
    }

    //called by LifeManager singleton
    public void OnDie()
    {
        triedToJump = false;
    }

    private IEnumerator JumpCoyoteTime(float cTime)
    {
        yield return new WaitForSeconds(cTime);
        if(!coll.onGround) canJump = false;
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

    void JumpHandler(InputAction.CallbackContext callbackContext)
    {
        if (GameStateManager.instance.isPaused() || TextBoxManager.instance.GetTextBoxActive()) return;
        jumpHold = true;

        if (wr.OnJumpKeyPress()) return;
        if(jumpBufferCoroutine != null) StopCoroutine(jumpBufferCoroutine);
        jumpBufferCoroutine = StartCoroutine(TriedToJump());
    }

    private void JumpButtonUp(InputAction.CallbackContext callbackContext)
    {
        jumpHold = false;
    }
    #endregion
}