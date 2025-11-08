using System;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class ArcherEnemy : Enemy,IDamageable
{
    public IdleState_Data IdleStateData;

    public SpearEnemy_PatrolState_Data PatrolStateData;

    public ArcherEnemy_AttackState_Data AttackStateData;

    public ArcherEnemy_RetreatState_Data RetreatStateData;

    public State IdleState { get; private set; }

    public State PatrolState { get; private set; }
    
    public State RetreatState { get; private set; }
    
    public State AttackState { get; private set; }
    
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        #region Intialization of states
        IdleState = new ArcherEnemy_IdleState(IdleStateData);
        PatrolState = new ArcherEnemy_PatrolState(PatrolStateData);
        AttackState = new ArcherEnemy_AttackState(AttackStateData);
        RetreatState = new ArcherEnemy_RetreatState(RetreatStateData);
        #endregion

        #region Setting of state names 
        IdleState.SetStateName("IDLE");
        PatrolState.SetStateName("PATROL");
        AttackState.SetStateName("ATTACK");
        RetreatState.SetStateName("RETREAT");
        #endregion

        #region Adding of states to state machine
        stateMachine.AddState(IdleState);
        stateMachine.AddState(PatrolState);
        stateMachine.AddState(AttackState);
        stateMachine.AddState(RetreatState);
        #endregion

        this.NextState = IdleState;
    }
    
    // Update is called once per frame
    private new void Update()
    {
        base.Update();
    }

    public void TakeDamage(float amount)
    {
        throw new System.NotImplementedException();
    }
    
}
