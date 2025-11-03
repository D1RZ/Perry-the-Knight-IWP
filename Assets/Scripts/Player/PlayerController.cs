using System;
using UnityEngine;

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

    [SerializeField] private float dashTime;

    [SerializeField] private float dashSpeed;

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
                transform.localScale *= new Vector2(-1,1);
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

    private bool isDashing;

    private bool isRolling = false;

    private bool isBlocking = false;

    private float dashTimeLeft;

    private float lastImageXPos;

    private float lastDash = -100f;

    private bool canFlip;

    private static PlayerController _instance;

    private bool normalAttack = false; // just for normal attack

    private bool canAttack = true; // for all attacks shared

    private float currentAttackInputTimer = 0;

    [SerializeField] private float AttackInputDelay;

    private Animator animator;

    private bool isHit = false;

    private float startBlockTime = 0;

    public static PlayerController Instance
    { 
        get
        {
            if (_instance == null) Debug.Log("GameManager is null");

            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
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
        if(Player.HealthData <= 0)
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
    }

    private void HandleMovementState()
    {
        if (!InAir && canMove)
        {
            if (!isRolling && !isBlocking)
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
                else if (!walk && !run)
                {
                    rb.constraints = RigidbodyConstraints2D.FreezePositionX; // freezing of x position to prevent character from sliding due to 2d physics material
                    animationController.SetAnimation("idle");
                }
                else // if running
                {
                    rb.constraints = RigidbodyConstraints2D.None; // reset constraints of rigidbody 2d
                    rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                    if (walkSpeed < maxSpeed)
                    {
                        walkSpeed = walkSpeed * 1.2f;
                    }
                    else
                    {
                        walkSpeed = maxSpeed;
                    }
                    animationController.SetAnimation("run");
                }
            }

            if (normalAttack)
            {
                canMove = false;
                rb.constraints = RigidbodyConstraints2D.FreezePositionX;
                OnPlayerAttack.Invoke("Normal Attack");
                normalAttack = false;
            }
        }
        else
        {
            rb.constraints = RigidbodyConstraints2D.None; // reset constraints of rigidbody 2d
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            walkSpeed = defaultwalkspeed;
        }
    }

    private void HandleGravity()
    {
         // Fallback check: if roll got cancelled by falling
        if (isRolling && InAir && rb.velocity.y < 0)
        {
            EndRoll();
        }

        if (rb.velocity.y < 0f && canMove)
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
        if (Input.GetKeyDown(KeyCode.I))
        {
            audioSettingsManager.TurnOnOffAudioMenu();
        }

        if (canMove)
        {
            dirH = Input.GetAxis("Horizontal");
        }
        else
        {
            dirH = 0.0f;
        }

        walk = Mathf.Abs(dirH) > 0.01f && defaultwalkspeed > 0 && canMove; // 0.01f so that walk animation will end early and not have delay between transition between walk and idle animation
        animationController._yVelocity = rb.velocity.y;

        if (!isWallSliding && canMove)
        {
            SetFacingDirection();
        }

        if (Input.GetKeyDown(KeyCode.Space) && !jump && !normalAttack && !isRolling && AmtOfJumpsLeft > 0 && canMove)
        {
            jump = true;
        }

        if (Input.GetKeyUp(KeyCode.Space) && canMove && !normalAttack && !isRolling)
        {
            variablejump = true;
        }

        if (Input.GetMouseButtonDown(0) && !InAir && !isRolling && currentAttackInputTimer <= 0 && !isHit)
        {
            currentAttackInputTimer = AttackInputDelay; // resets attack input timer
            normalAttack = true;
        }

        if (Input.GetKeyDown(KeyCode.V) && !jump && !normalAttack && !isRolling && canMove && !InAir && !isHit)
        {
            StartRoll();
        }

        if (Input.GetMouseButtonDown(1) && !jump && !normalAttack && !isRolling && !InAir && !isHit && !isBlocking)
        {
            StartBlock();
        }

        if (Input.GetMouseButtonUp(1) && isBlocking)
        {
            EndBlock();
        }
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
            rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        else
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        walk = Mathf.Abs(dirH) > 0.01f;
        run = false;

        // blend back to idle or walk depending on input
        if (Mathf.Abs(dirH) > 0.01f)
            animationController.SetAnimation("walk");
        else
            animationController.SetAnimation("idle");
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
        walk = Mathf.Abs(dirH) > 0.01f;
        run = false;
        if (walk)
            animationController.SetAnimation("walk");
        else
            animationController.SetAnimation("idle");
    }

    public void OnHit()
    {
        if (isHit) return; // already in hit state

        isHit = true;
        canMove = false;
        isRolling = false;
        normalAttack = false;
        isDashing = false;

        // Stop all motion immediately
        rb.velocity = Vector2.zero;
    }

    private void AttemptToDash()
    {
        isDashing = true;
        dashTimeLeft = dashTime;
        lastDash = Time.time;

        PlayerAfterImagePool.Instance.GetFromPool();
        lastImageXPos = transform.position.x;
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
        if(dirH > 0f && !_IsFacingRight && canFlip)
        {
            facingDirection *= -1;
            _IsFacingRight = true;
        }
        else if(dirH < 0f && _IsFacingRight && canFlip)
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
        if(isDashing)
        {
            if(dashTimeLeft > 0)
            {
                canMove = false;
                rb.velocity = new Vector2(dashSpeed * facingDirection, 0); // 0 so that when dashing player will not fall
                dashTimeLeft -= Time.deltaTime;

                if (Mathf.Abs(transform.position.x - lastImageXPos) > distanceBetweenImages)
                {
                    PlayerAfterImagePool.Instance.GetFromPool();
                    lastImageXPos = transform.position.x;
                }
            }

            if(dashTimeLeft <= 0 || TouchingWall)
            {
                canMove = true;
                isDashing = false;
            }
        }
        
        if (walk && !InAir && !isRolling && !isBlocking) movementController.MoveHorizontal(dirH * walkSpeed);
        else if(InAir && !isWallSliding && dirH != 0)
        {
            Vector2 ForceToAdd = new Vector2(movementForceInAir * dirH, 0);
            rb.AddForce(ForceToAdd);

            if(Mathf.Abs(rb.velocity.x) > walkSpeed)
            {
                rb.velocity = new Vector2(walkSpeed * dirH,rb.velocity.y);
            }
        }
        else if(InAir && !isWallSliding && dirH == 0)
        {
            rb.velocity = new Vector2(rb.velocity.x * airDragMultiplier,rb.velocity.y);
        }

        if(isWallSliding && canMove)
        {
            if (rb.velocity.y < -WallSlideSpeed)
            {
                rb.velocity = new Vector2(rb.velocity.x, -WallSlideSpeed);
            }
        }

        if (jump && !isWallSliding)
        {
            AmtOfJumpsLeft--;
            movementController.MoveVertical(jumpSpeed);
            jump = false;
        }
        else if (isWallSliding && dirH <= Math.Abs(0.1f) && jump) // if no direction specified when on wall
        {
            isWallSliding = false;
            Vector2 forceToAdd = new Vector2(wallHopForce * wallHopDirection.x * -facingDirection, wallHopForce * wallHopDirection.y);
            rb.AddForce(forceToAdd, ForceMode2D.Impulse);
        }
        else if ((isWallSliding || TouchingWall) && dirH > Math.Abs(0.1f) && jump) // if there is direction specified when on wall
        {
            Vector2 forceToAdd = new Vector2(wallJumpForce * wallJumpDirection.x * dirH, wallJumpForce * wallJumpDirection.y);
            rb.AddForce(forceToAdd, ForceMode2D.Impulse);
        }
         
        if (variablejump)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * variableJumpHeightMultiplier);
            variablejump = false;
        }
    }

    public void ResetCanMove()
    {
        if(!canMove) canMove = true;
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
        parryAnimator.SetTrigger("Block");
    }

    public void ResetParryVFX()
    {
        parryAnimator.ResetTrigger("Block");
        animator.SetBool("Block", false);
    }

}

