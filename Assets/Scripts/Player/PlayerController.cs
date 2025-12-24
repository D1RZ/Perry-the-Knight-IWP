using System;
using UnityEngine;

public enum AttackType
{
    None,
    Normal,
    Lift,
    Air,
    GroundSlam,
    Dash
}

public class PlayerController : Entity
{
    // Static event that passes a float
    public static event Action<float> OnPlayerHit;

    public float walkSpeed = 4.5f;

    [SerializeField] private float jumpSpeed = 8f;

    [SerializeField] private float maxSpeed = 5.5f;

    [SerializeField]
    private float fallMultiplier = 2.5f;

    [SerializeField]
    private float lowJumpMultiplier = 2f;

    [SerializeField]
    private AudioClip WalkAudioClip;

    [SerializeField] private int AmtOfJumps = 2;

    [SerializeField] private float WallSlideSpeed = 2f;

    [SerializeField] private float movementForceInAir;

    [SerializeField] private float airDragMultiplier = 0.95f;

    [SerializeField] private float variableJumpHeightMultiplier = 0.5f;

    [SerializeField] private float wallHopForce;

    [SerializeField] private float wallJumpForce;

    [SerializeField] private Vector2 wallHopDirection;

    [SerializeField] private Vector2 wallJumpDirection;

    [SerializeField] private float ledgeClimbXOffset1 = 0f;

    [SerializeField] private float ledgeClimbYOffset1 = 0f;

    [SerializeField] private float ledgeClimbXOffset2 = 0f;

    [SerializeField] private float ledgeClimbYOffset2 = 0f;

    [SerializeField] private float distanceBetweenImages;

    [SerializeField] private float dashCooldown;

    [SerializeField] private AudioSettingsManager audioSettingsManager;

    [SerializeField] private PlayerData Player;

    [SerializeField] private float rollForce;

    private float rollTimer;

    public static Action<string> OnPlayerAttack;

    public PlayerData _PlayerData
    {
        get
        {
            return Player;
        }

        set
        {
            Player = value;
        }
    }

    private AudioSource sfxAudioSource;

    private MovementController movementController;

    public AnimationController animationController;

    private bool _isfacingright = true;

    private bool InAir = false;

    private bool TouchingWall = false;

    private int AmtOfJumpsLeft;

    public float defaultwalkspeed;

    private float WallRayOffset = 0;

    private bool isWallSliding;

    private bool variablejump = false;

    private bool isTouchingLedge;

    private bool canClimbLedge = false;

    private bool ledgeDetected;

    private Vector2 ledgePosBot;

    private Vector2 ledgePos1;

    private Vector2 ledgePos2;

    private bool canMove = true;

    private bool BodyTouchingLedge; // checks if body touches specifically ledges

    [SerializeField] private Animator parryAnimator;

    private bool isAttacking;   // true when any attack is active

    public bool _IsFacingRight
    {
        get
        {
            return _isfacingright;
        }
        set
        {
            if (_isfacingright != value)
            {
                transform.localScale *= new Vector2(-1, 1);
            }
            _isfacingright = value;
        }
    }

    private float dirH = 0.0f;

    public float DirH
    {
        get
        { return dirH; }
        set
        {
            dirH = value;
        }
    }

    private bool walk = false;

    private bool run = false;

    private bool jump = false;

    private float jumptimer;

    private bool isDashAttacking = false;

    private bool isRolling = false;

    private bool isBlocking = false;

    private float lastImageXPos;

    private float lastDash = -100f;

    private bool canFlip;

    private bool normalAttack = false; // for normal attack

    private bool liftAttack = false; // for lift attack

    private bool airAttack = false;

    private bool dashAttack = false;

    private int airAttackCount = 0;

    private bool canAttack = true; // for all attacks shared

    private float currentAttackInputTimer = 0;

    [SerializeField] private float AttackInputDelay;

    private Animator animator;

    private bool isHit = false;

    private float startBlockTime = 0;

    public bool blockSuccess = false;

    public bool parrySuccess = false;

    private float airAttackHangTimer = 0f;

    public float maxAirHangTime = 0.45f; // tweak this

    // for dash attack input check
    private bool isHolding = false;

    private float holdTime = 0f;

    public float dashAttackThreshold = 0.4f;

    private float dashAttackTimeLeft;

    [SerializeField] private float dashAttackTime;

    [SerializeField] private float dashAttackSpeed;

    private static PlayerController _instance;

    public static PlayerController Instance
    {
        get
        {
            if (_instance == null) Debug.Log("GameManager is null");

            return _instance;
        }
    }

    public enum BufferedInputType { None, Press, Hold , Lift} // stores next input

    public BufferedInputType bufferedInput = BufferedInputType.None;

    private AttackType currentAttackType = AttackType.None; // keeps track of current attack type for buffer input

    [SerializeField] private Transform smokeVFXCreatePos;

    [SerializeField] private DashAttackChecker dashAttackChecker;

    public static event Action<PlayerController> OnPlayerReady;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        OnPlayerReady?.Invoke(this);
    }

    // Start is called before the first frame update
    private new void Start()
    {
        base.Start();
        movementController = GetComponent<MovementController>();
        animationController = GetComponent<AnimationController>();
        sfxAudioSource = GetComponent<AudioSource>();
        AmtOfJumpsLeft = AmtOfJumps;
        defaultwalkspeed = walkSpeed;
        wallHopDirection.Normalize();
        wallJumpDirection.Normalize();
        canFlip = true;
        animator = GetComponent<Animator>();
        Player.HealthData = Player.MaxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (Player.HealthData <= 0)
        {
            gameObject.SetActive(false);
            return;
        }

        if (isHit)
        {
            return; // skip all input logic when hit
        }

        UpdateTimers();
        HandleInput();
        HandleGroundCheck();
        HandleGravity();
        HandleMovementState();

        Debug.Log("IS HOLDING: " + isHolding);
    }

    private void HandleMovementState()
    {
        if (!InAir && canMove)
        {
            if (!isRolling && !isBlocking)
            {
                HandleWalkIdle();
            }
        }
        else
        {
            rb.constraints = RigidbodyConstraints2D.None; // reset constraints of rigidbody 2d
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            walkSpeed = defaultwalkspeed;
        }

        HandleAttackState();
    }

    private void HandleWalkIdle()
    {
        if (walk && !run)
        {
            Debug.Log("Activated! Walking");
            rb.constraints = RigidbodyConstraints2D.None; // reset constraints of rigidbody 2d
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            walkSpeed = defaultwalkspeed;
            animationController.SetAnimation("walk");
            if (!sfxAudioSource.isPlaying)
            {
                sfxAudioSource.clip = WalkAudioClip;
                sfxAudioSource.Play();
            }
        }
        else if (!walk && !run && !isDashAttacking)
        {
            rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation; // freezing of x position to prevent character from sliding due to 2d physics material
            animationController.SetAnimation("idle");
        }
    }

    private void HandleAttackState()
    {
        if (normalAttack)
        {
            ExecuteAttack(AttackType.Normal);
            return;
        }

        if (liftAttack)
        {
            ExecuteAttack(AttackType.Lift);
            return;
        }
        
        if(airAttack)
        {
            Debug.Log("Call execute!");
            ExecuteAttack(AttackType.Air);
            return;
        }

        if (dashAttack) ExecuteAttack(AttackType.Dash);
        
        // if (slamAttack) ExecuteAttack(AttackType.GroundSlam);
    }

    private void ExecuteAttack(AttackType type)
    {
        isAttacking = true;
        canMove = false;
        if(currentAttackType != AttackType.Dash && currentAttackType != AttackType.Lift) rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;

        switch (type)
        {
            case AttackType.Normal:
                OnPlayerAttack?.Invoke("Normal Attack");
                normalAttack = false;
                break;

            case AttackType.Lift:
                OnPlayerAttack?.Invoke("Lift Attack");
                liftAttack = false;
                break;

            case AttackType.Air:
                Debug.Log("Invoked Event!");
                OnPlayerAttack?.Invoke("Air Attack");
                airAttack = false;
                break;

            case AttackType.Dash:
                OnPlayerAttack?.Invoke("Dash Attack");
                dashAttack = false;
                break;

            // Add more here later
            case AttackType.GroundSlam:
                OnPlayerAttack?.Invoke("Ground Slam");
                break;
        }
    }

    private void HandleGravity()
    {
        if(!InAir)
        {
            if(airAttackCount > 0) airAttackCount = 0;
            if(rb.gravityScale > 1) rb.gravityScale = 1;
        }

        if (airAttack)
            return; // ignore gravity during air attack

        // Fallback check: if roll got cancelled by falling
        if (isRolling && InAir)
        {
            EndRoll();
        }

        if (rb.velocity.y < 0f)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0f && !jump)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    private void HandleInput()
    {
        if(isHolding && Input.GetMouseButtonUp(0))
        {
            isHolding = false;
            holdTime = 0;
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            audioSettingsManager.TurnOnOffAudioMenu();
        }

        if (canMove && !isDashAttacking)
        {
            dirH = Input.GetAxis("Horizontal");
        }
        else
        {
            dirH = 0.0f;
        }

        walk = Mathf.Abs(dirH) > 0.01f && defaultwalkspeed > 0 && canMove; // 0.01f so that walk animation will end early and not have delay between transition between walk and idle animation
        animationController._yVelocity = rb.velocity.y;

        if (!isWallSliding && canMove && !isDashAttacking)
        {
            SetFacingDirection();
        }

        if (Input.GetKeyDown(KeyCode.Space) && !jump && !isAttacking && !isRolling && AmtOfJumpsLeft > 0 && canMove)
        {
            jump = true;
        }

        if (Input.GetKeyUp(KeyCode.Space) && canMove && !isAttacking && !isRolling)
        {
            variablejump = true;
        }

        AttackType attack = ResolveAttack();
        if (attack != AttackType.None)
        {
            PerformAttack(attack);
        }

        if (Input.GetKeyDown(KeyCode.V) && !jump && !isAttacking && !isRolling && canMove && !InAir && !isHit)
        {
            StartRoll();
        }

        if (Input.GetMouseButtonDown(1) && !jump && !isAttacking && !isRolling && !InAir && !isHit && !isBlocking)
        {
            StartBlock();
        }

        if (!Input.GetMouseButton(1) && isBlocking && !blockSuccess)
        {
            EndBlock();
        }
    }

    private AttackType ResolveAttack()
    {
        if (isRolling || isHit || isBlocking || isDashAttacking)
            return AttackType.None;

        if (currentAttackType == AttackType.Dash && isHolding) return AttackType.None;

        if (currentAttackInputTimer > 0)
        {
            Debug.Log("RUNNING BUFFER " + currentAttackType);

            if(bufferedInput == BufferedInputType.None && currentAttackType == AttackType.Normal)
            {
                CheckForBufferInput();
            }

            return AttackType.None;
        }

        if (InAir)
        {
            Debug.Log("IN AIR!");

            if (Input.GetMouseButtonDown(0) && airAttackCount <= 3)
            {
                airAttackCount++;
                currentAttackInputTimer = 0.35f;
                currentAttackType = AttackType.Air;
                Debug.Log("Air Attack!");
                return AttackType.Air;
            }

            return AttackType.None;
        }

        if (Input.GetKey(KeyCode.W) && Input.GetMouseButtonDown(0))
        {
            currentAttackInputTimer = 0.5f;
            currentAttackType = AttackType.Lift;
            return AttackType.Lift;
        }

        if (bufferedInput != BufferedInputType.None)
        {
            holdTime = 0;
            if (bufferedInput == BufferedInputType.Press)
            {
                currentAttackInputTimer = 0.5f;
                bufferedInput = BufferedInputType.None;
                currentAttackType = AttackType.Normal;
                Debug.Log("NORMAL ATTACK BUFFER!");
                return AttackType.Normal;
            }
            else if(bufferedInput == BufferedInputType.Hold)
            {
                currentAttackInputTimer = 0.6f;
                bufferedInput = BufferedInputType.None;
                currentAttackType = AttackType.Dash;
                Debug.Log("DASH ATTACK BUFFER!");
                return AttackType.Dash;
            }
            else if(bufferedInput == BufferedInputType.Lift)
            {
                currentAttackInputTimer = 0.5f;
                bufferedInput = BufferedInputType.None;
                currentAttackType = AttackType.Lift;
                return AttackType.Lift;
            }
        }
       
        if(Input.GetMouseButtonDown(0))
        {
            currentAttackInputTimer = 0.5f;
            currentAttackType = AttackType.Normal;
            return AttackType.Normal;
        }

        return AttackType.None;
    }

    private void CheckForBufferInput()
    {
        Debug.Log("CHECKING FOR BUFFER");

        if (!isHolding && Input.GetKey(KeyCode.W) && Input.GetMouseButtonDown(0))
        {
            bufferedInput = BufferedInputType.None;
            return;
        }

        if(Input.GetMouseButton(0) && !isHolding)
        {
            Debug.Log("IS HOLDING");
            holdTime = 0;
            isHolding = true;
            return;
        }

        if(isHolding && Input.GetMouseButtonUp(0))
        {
            Debug.Log("PRESS BUFFER");
            bufferedInput = BufferedInputType.Press;
            isHolding = false;
            return;
        }

        if(isHolding && Input.GetMouseButton(0))
        {
            holdTime += Time.deltaTime;

            if(holdTime > dashAttackThreshold)
            {
                Debug.Log("HOLD BUFFER");
                bufferedInput = BufferedInputType.Hold;
            }
        }
    }

    private void PerformAttack(AttackType attack)
    {
        switch (attack)
        {
            case AttackType.Normal:
                liftAttack = false;
                airAttack = false;
                normalAttack = true;
                break;

            case AttackType.Lift:
                normalAttack = false;
                airAttack = false;
                liftAttack = true;
                rb.gravityScale = 1.25f;
                break;

            case AttackType.Air:
                Debug.Log("Air Resolved!");
                normalAttack = false;
                liftAttack = false;
                airAttack = true;
                rb.velocity = Vector2.zero;  // stop drifting
                rb.gravityScale = 0f;
                airAttackHangTimer = maxAirHangTime;
                break;

            case AttackType.Dash:
                normalAttack = false;
                liftAttack = false;
                airAttack = false;
                dashAttack = true;
                rb.velocity = Vector2.zero;
                dashAttackChecker.ResetDashHits();
                break;
        }

        isAttacking = true;
    }
    
    private void StartAttack()
    {
        currentAttackInputTimer = AttackInputDelay;
        normalAttack = true;
    }

    public static void InvokeOnPlayerHit()
    {
        OnPlayerHit?.Invoke(Instance.Player.HealthData);
    }

    private void StartRoll()
    {
        isRolling = true;
        canMove = false;

        // Clear any movement locks
        walk = false;
        run = false;
        rb.constraints = RigidbodyConstraints2D.None;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        transform.GetComponent<BoxCollider2D>().enabled = false;
        rb.gravityScale = 0;

        // Clear existing velocity so the roll feels snappy
        rb.velocity = Vector2.zero;

        // Apply one-time impulse in facing direction
        float direction = _IsFacingRight ? 1f : -1f;
        rb.AddForce(new Vector2(rollForce * direction, 0f), ForceMode2D.Impulse);

        animationController.SetAnimation("roll");
    }

    private void UpdateTimers()
    {
        if (currentAttackInputTimer > 0)
            currentAttackInputTimer -= Time.deltaTime;

        // Air attack hang failsafe
        if (airAttackHangTimer > 0)
        {
            airAttackHangTimer -= Time.deltaTime;

            if (airAttackHangTimer <= 0)
            {
                EndAirAttack(); // auto recover
            }
        }

        if (dashAttackTimeLeft > 0) dashAttackTimeLeft -= Time.deltaTime;
    }

    public void EndRoll()
    {
        isRolling = false;
        canMove = true;
        // stop any leftover sliding motion
        rb.velocity = Vector2.zero;
        animationController.animator.ResetTrigger("roll");
        // Only freeze X if player is grounded
        if (!InAir)
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        else
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        rb.gravityScale = 1;
        transform.GetComponent<BoxCollider2D>().enabled = true;

        walk = Mathf.Abs(dirH) > 0.01f && defaultwalkspeed > 0 && canMove;
        run = false;
    }

    private void StartBlock()
    {
        isBlocking = true;
        canMove = false;
        startBlockTime = Time.time;

        walk = false;
        run = false;
        rb.velocity = Vector2.zero;

        // Clear any movement locks
        rb.constraints = RigidbodyConstraints2D.None;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        rb.velocity = Vector2.zero;

        animationController.SetAnimation("block");
    }

    private void EndBlock()
    {
        startBlockTime = 0;
        isBlocking = false;
        canMove = true;
        rb.velocity = Vector2.zero;
        walk = Mathf.Abs(dirH) > 0.01f && defaultwalkspeed > 0 && canMove;
        run = false;
    }

    public void OnHit()
    {
        if (isHit) return; // already in hit state

        isHit = true;
        canMove = false;
        isRolling = false;
        normalAttack = false;
        //isDashing = false;

        // Stop all motion immediately
        rb.velocity = Vector2.zero;
    }

    public void FinishLedgeClimb()
    {
        canClimbLedge = false;
        transform.position = ledgePos2;
        canMove = true;
        ledgeDetected = false;
    }

    public void SetFacingDirection()
    {
        if (dirH > 0f && !_IsFacingRight && canFlip)
        {
            facingDirection *= -1;
            _IsFacingRight = true;
        }
        else if (dirH < 0f && _IsFacingRight && canFlip)
        {
            facingDirection *= -1;
            _IsFacingRight = false;
        }
    }

    public void EnableCanFlip()
    {
        canFlip = true;
    }

    public void DisableCanFlip()
    {
        canFlip = false;
    }

    private void FixedUpdate()
    {
        HandleDashMovement();
        HandleGroundMovement();
        HandleAirMovement();
        HandleWallSlide();
        HandleJumpLogic();
        HandleVariableJump();
    }

    private void HandleDashMovement()
    {
        if(isDashAttacking)
        {
            if(dashAttackTimeLeft <= 0f)
            {
                dashAttackTimeLeft = 0f;
                rb.velocity = Vector3.zero;
                isDashAttacking = false;
            }
        }
    }

    private void HandleGroundMovement()
    {
        if (!walk || InAir || isRolling || isBlocking || isDashAttacking) return;

        rb.constraints = RigidbodyConstraints2D.None;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        movementController.MoveHorizontal(dirH * walkSpeed);
    }

    private void HandleAirMovement()
    {
        if (!InAir || isWallSliding || isDashAttacking || (isAttacking && currentAttackType != AttackType.Lift)) return;

        if (dirH != 0)
        {
            if (airAttackCount == 0)
            {
                Vector2 force = new Vector2(movementForceInAir * dirH, 0);
                rb.AddForce(force);

                if (Mathf.Abs(rb.velocity.x) > walkSpeed)
                    rb.velocity = new Vector2(walkSpeed * dirH, rb.velocity.y);
            }
        }
        else
        {
            // No horizontal input — apply drag
            rb.velocity = new Vector2(rb.velocity.x * airDragMultiplier, rb.velocity.y);
        }
    }

    private void HandleWallSlide()
    {
        if (!isWallSliding || !canMove) return;

        if (rb.velocity.y < -WallSlideSpeed)
            rb.velocity = new Vector2(rb.velocity.x, -WallSlideSpeed);
    }

    private void HandleJumpLogic()
    {
        if (!jump) return;

        if (!isWallSliding)
        {
            // Normal jump
            AmtOfJumpsLeft--;
            movementController.MoveVertical(jumpSpeed);
            jump = false;
            return;
        }

        // Wall slide jumps
        if (dirH <= Mathf.Abs(0.1f))
        {
            // Wall hop (no direction)
            isWallSliding = false;
            Vector2 force = new Vector2(
                wallHopForce * wallHopDirection.x * -facingDirection,
                wallHopForce * wallHopDirection.y
            );
            rb.AddForce(force, ForceMode2D.Impulse);
        }
        else
        {
            // Wall jump (with direction)
            Vector2 force = new Vector2(
                wallJumpForce * wallJumpDirection.x * dirH,
                wallJumpForce * wallJumpDirection.y
            );
            rb.AddForce(force, ForceMode2D.Impulse);
        }

        jump = false;
    }

    private void HandleVariableJump()
    {
        if (!variablejump) return;

        rb.velocity = new Vector2(
            rb.velocity.x,
            rb.velocity.y * variableJumpHeightMultiplier
        );

        variablejump = false;
    }

    public void ResetCanMove()
    {
        if (!canMove) canMove = true;
        animationController.animator.SetBool("liftAttack", false);
    }

    public void SetCanMove(bool move)
    {
        canMove = move;
    }

    public bool GetIsRolling()
    {
        return isRolling;
    }

    public bool GetIsBlocking()
    {
        return isBlocking;
    }

    public float GetStartBlockTime()
    {
        return startBlockTime;
    }

    public void SetIsHit(bool hit)
    {
        isHit = hit;
    }

    private void HandleGroundCheck()
    {
        Vector2 origin = new Vector2(transform.position.x, transform.position.y - 0.45f);
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, 1.25f, LayerMask.GetMask("Platforms", "Ledges"));
        InAir = hit.collider == null;
        animationController._InAir = InAir;

        if (!InAir && rb.velocity.y <= 0)
            AmtOfJumpsLeft = AmtOfJumps;
    }

    public void ActivateParryVFX()
    {
        if(parrySuccess) parryAnimator.SetTrigger("Block");
    }

    public void ResetParryVFX()
    {
        parryAnimator.ResetTrigger("Block");
        animator.SetBool("Block", false);
        blockSuccess = false;
        parrySuccess = false;
    }

    public void StartDashAttack()
    {
        animator.speed = 0;

        GameObject smokeVFX = Instantiate(ParticleManager.Instance.GetParticleEffect("Smoke"), smokeVFXCreatePos.transform.position, Quaternion.identity);

        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        rb.velocity = new Vector2(facingDirection * dashAttackSpeed, 0);

        Debug.Log("DASH RB VELOCITY: " + rb.velocity);

        isDashAttacking = true;

        dashAttackTimeLeft = dashAttackTime;

        animator.speed = 1;
    }

    public void EndAttack()
    {
        isAttacking = false;
        normalAttack = false;
    }

    public void EndAirAttack()
    {
        airAttack = false;
        isAttacking = false;
        canMove = true;
        rb.gravityScale = 1;
        rb.velocity = new Vector2(rb.velocity.x,0);
    }

    public bool GetIsDashAttacking()
    {
        return isDashAttacking;
    }

}

