public class FlyingEnemy : Enemy
{
    public State IdleState { get; private set; }

    public State ChaseState { get; private set; }

    public State AttackState { get; private set; }

    public State CooldownState { get; private set; }

    public FlyingEnemyIdleState_Data IdleStateData;

    public ChaseState_Data ChaseStateData;

    public FlyingEnemyAttackState_Data AttackStateData;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        #region Intialization of states
        IdleState = new FlyingEnemy_IdleState(IdleStateData);
        ChaseState = new FlyingEnemy_ChaseState(ChaseStateData);
        AttackState = new FlyingEnemy_AttackState(AttackStateData);
        CooldownState = new FlyingEnemy_CooldownState();
        #endregion

        #region Setting of state names 
        IdleState.SetStateName("IDLE");
        ChaseState.SetStateName("CHASE");
        AttackState.SetStateName("ATTACK");
        CooldownState.SetStateName("COOLDOWN");
        #endregion

        #region Adding of states to state machine
        stateMachine.AddState(IdleState);
        stateMachine.AddState(ChaseState);
        stateMachine.AddState(AttackState);
        stateMachine.AddState(CooldownState);
        #endregion 

        this.NextState = IdleState;
    }

    public override void HitConnected(int AttackNo)
    {
        if (!PlayerController.Instance.GetIsBlocking())
            PlayerController.Instance._PlayerData.HealthData -= AttackStateData.damage;
        else
            PlayerController.Instance._PlayerData.HealthData -= 15;
    }

}
