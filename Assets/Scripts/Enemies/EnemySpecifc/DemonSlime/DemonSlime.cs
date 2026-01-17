using UnityEngine;

public class DemonSlime : Enemy
{
    public ChaseState_Data ChaseStateData;
    public FlyingEnemyAttackState_Data DashAttackStateData;
    public DemonSlimeMagicAttackData MagicAttackStateData;
    public State ChaseState { get; private set; }
    public State DashAttackState { get; private set; }
    public State MagicAttackState { get; private set; }
    private bool TriggerTransform = false;
    [SerializeField] private BossRoomCutscenes cutsceneManager;

    public override void Start()
    {
        base.Start();

        #region Intialization of states
        ChaseState = new DemonSlime_ChaseState(ChaseStateData);
        DashAttackState = new DemonSlime_DashAttackState(DashAttackStateData);
        MagicAttackState = new DemonSlime_MagicAttackState(ParticleManager.Instance.GetParticleEffect("Spike"));
        #endregion

        #region Setting of state names
        ChaseState.SetStateName("CHASE");
        DashAttackState.SetStateName("DASHATTACK");
        MagicAttackState.SetStateName("MAGICATTACK");
        #endregion

        #region Adding of states to state machine
        stateMachine.AddState(ChaseState);
        stateMachine.AddState(DashAttackState);
        stateMachine.AddState(MagicAttackState);
        #endregion

        this.NextState = MagicAttackState;
    }

    private new void Update()
    {
        base.Update();

        Debug.Log("DEMON SLIME HEALTH: " + health);
    }

    public override void DeadEvent()
    {
        if (TriggerTransform) return;

        Debug.Log("DEAD EVENT TRIGGERED");
        TriggerTransform = true;
        cutsceneManager.StartCoroutine(cutsceneManager.BossTransitionSegment());
    }

    public override void HitConnected(int AttackNo)
    {
        if (!PlayerController.Instance.GetIsBlocking())
            PlayerController.Instance._PlayerData.HealthData -= 40;
        else
            PlayerController.Instance._PlayerData.HealthData -= 8;
    }

    public override void OnStunEnd()
    {
        transform.GetChild(0).GetComponent<Collider2D>().enabled = true;
        NextState = ChaseState;
        CurrentState = ChaseState;
        CurrentState.Enter(this);
    }

    public override void SetHealth(float dmg)
    {
        health -= dmg;
        if (health <= 0f) health = 0f;
        HealthBar.SetActive(true);
        OnBossHit.Invoke(this);
    }

    public void ResetSlime()
    {
        NextState = MagicAttackState;
        CurrentState = MagicAttackState;
        CurrentState.Enter(this);
    }

}
