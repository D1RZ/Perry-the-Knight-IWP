using UnityEngine;

public class AnimationController : MonoBehaviour
{
    public Animator animator;

    private bool _ismoving;
    
    public bool _IsMoving
    {
        get
        {
            return _ismoving;
        }
        set
        {
            _ismoving = value;
            animator.SetBool("IsMoving", value);
        }
    }

    private bool _isrunning;

    public bool _IsRunning
    {
        get
        {
            return _isrunning;
        }
        set
        {
            animator.SetBool("IsRunning", value);
        }
    }

    private float _yvelocity;

    public float _yVelocity
    {
        get
        {
            return _yvelocity;
        }
        set
        {
            animator.SetFloat("yVelocity",value);
        }
    }

    private bool _inair;

    public bool _InAir
    {
        get
        {
            return _inair;
        }
        set
        {
            animator.SetBool("InAir",value);
        }
    }

    public bool _WallSliding
    {
        get
        {
            return _WallSliding;
        }
        set
        {
            animator.SetBool("IsWallSliding",value);
        }
    }

    public bool _ClimbLedge
    {
        get
        {
            return _ClimbLedge;
        }
        set
        {
            animator.SetBool("canClimbLedge",value);
        }
    }

    public bool _Roll
    {
        get
        {
            return _Roll;
        }
        set
        {
            animator.SetTrigger("roll");
        }
    }

    public bool _Block
    {
        get
        {
            return _Block;
        }
        set
        {
            animator.SetBool("isBlocking",value);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void SetAnimation(string animation)
    {
        if (animation == "idle")
        {
            _IsMoving = false;
            _IsRunning = false;
            _Block = false;
        }
        else if(animation == "walk")
        {
            _IsMoving = true;
            _IsRunning = false;
            _Block = false;
        }
        else if(animation == "run")
        {
            _IsMoving = true;
            _IsRunning = true;
            _Block = false;
        }
        else if(animation == "roll")
        {
            _Roll = true;
            _Block = false;
        }
        else if(animation == "block")
        {
            _IsMoving = false;
            _IsRunning = false;
            _Block = true;
        }
    }

}
