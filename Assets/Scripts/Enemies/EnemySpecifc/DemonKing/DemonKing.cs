using UnityEngine;

public class DemonKing : Enemy
{
    public ChaseState_Data ChaseStateData;
    public FlyingEnemyAttackState_Data DashAttackStateData;
    public DemonSlimeMagicAttackData MagicAttackStateData;
    public State ChaseState { get; private set; }
    public State SlashAttackState { get; private set; }
    public State JumpAttackState { get; private set; }
    public State LaserMagicAttackState { get; private set; }
    public State SummonMagicAttackState { get; private set; }

    private bool TriggerTransform = false;

    [SerializeField] private GameObject spikeLeftSpawn;

    [SerializeField] private GameObject spikeRightSpawn;

    public override void Start()
    {
        base.Start();
            
        #region Intialization of states
        ChaseState = new DemonKing_ChaseState(ChaseStateData);
        SlashAttackState = new DemonKing_SlashState();
        JumpAttackState = new DemonKing_JumpState();
        //LaserMagicAttackState = new DemonSlime_MagicAttackState(ParticleManager.Instance.GetParticleEffect("Spike"));
        #endregion

        #region Setting of state names
        ChaseState.SetStateName("CHASE");
        SlashAttackState.SetStateName("SLASH");
        JumpAttackState.SetStateName("JUMP");
        //LaserMagicAttackState.SetStateName("LASER");
        //SummonMagicAttackState.SetStateName("SUMMON");
        #endregion

        #region Adding of states to state machine
        stateMachine.AddState(ChaseState);
        stateMachine.AddState(SlashAttackState);
        stateMachine.AddState(JumpAttackState);
        //stateMachine.AddState(LaserMagicAttackState);
        //stateMachine.AddState(SummonMagicAttackState);
        #endregion

        this.NextState = JumpAttackState;
    }

    private new void Update()
    {
        //if (!TriggerTransform) return;

        base.Update();

        Debug.Log("DEMON SLIME HEALTH: " + health);
    }

    public override void HitConnected(int AttackNo)
    {
        if (!PlayerController.Instance.GetIsBlocking())
            PlayerController.Instance._PlayerData.HealthData -= 50;
        else
            PlayerController.Instance._PlayerData.HealthData -= 20;
    }

    public override void OnStunEnd()
    {
        NextState = ChaseState;
        CurrentState = ChaseState;
        CurrentState.Enter(this);
    }

    public override void SetHealth(float dmg)
    {
        health -= dmg;
        if (health <= 0f) health = 0f;
        OnBossHit.Invoke(this);
    }
     
    public void SetTriggerTransform()
    {
        TriggerTransform = true;
    }

}