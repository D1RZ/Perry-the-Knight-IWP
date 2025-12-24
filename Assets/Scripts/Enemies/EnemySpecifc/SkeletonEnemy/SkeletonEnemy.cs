using UnityEngine;

public class SkeletonEnemy : Enemy
{
    public IdleState_Data IdleStateData;

    public PatrolState_Data PatrolStateData;

    public ChaseState_Data ChaseStateData;

    public GuardState_Data GuardStateData;

    public SpearEnemy_AttackState_Data AttackStateData;

    public State IdleState { get; private set; }

    public State PatrolState { get; private set; }

    public State ChaseState { get; private set; }

    public State GuardState { get; private set; }

    public State AttackState { get; private set; }

    public override void Start()
    {
        base.Start();
        
        #region Intialization of states
        IdleState = new SkeletonEnemy_IdleState(IdleStateData);
        PatrolState = new SkeletonEnemy_PatrolState(PatrolStateData);
        ChaseState = new SkeletonEnemy_ChaseState(ChaseStateData);
        GuardState = new SkeletonEnemy_GuardState(GuardStateData);
        AttackState = new SkeletonEnemy_AttackState(AttackStateData);
        #endregion

        #region Setting of state names 
        IdleState.SetStateName("IDLE");
        PatrolState.SetStateName("PATROL");
        ChaseState.SetStateName("CHASE");
        GuardState.SetStateName("GUARD");
        AttackState.SetStateName("ATTACK");
        #endregion

        #region Adding of states to state machine
        stateMachine.AddState(IdleState);
        stateMachine.AddState(PatrolState);
        stateMachine.AddState(ChaseState);
        stateMachine.AddState(GuardState);
        stateMachine.AddState(AttackState);
        #endregion

        this.NextState = IdleState;
    }

    // Update is called once per frame
    private new void Update()
    {
        base.Update();

        Debug.Log("Next State: " + NextState);
        Debug.Log("Current State: " + CurrentState);
    }

    public override void HitConnected(int AttackNo)
    {
        // since spear enemy only has 1 type of attack for now so therefore can ignore AttackNo
        if (!PlayerController.Instance.GetIsBlocking())
            PlayerController.Instance._PlayerData.HealthData -= AttackStateData.Damage;
        else
            PlayerController.Instance._PlayerData.HealthData -= 5;
    }

}