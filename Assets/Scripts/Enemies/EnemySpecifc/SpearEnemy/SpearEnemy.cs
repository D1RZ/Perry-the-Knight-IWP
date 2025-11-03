using System.Collections;
using UnityEngine;

public class SpearEnemy : Enemy,IDamageable
{
    public SpearEnemy_PatrolState_Data PatrolStateData;

    public SpearEnemy_IdleState_Data IdleStateData;

    public SpearEnemy_ChaseState_Data ChaseStateData;

    public SpearEnemy_Attack1State_Data Attack1StateData;

    public Transform VFXPos;

    public State IdleState { get; private set; }

    public State PatrolState { get; private set; }

    public State ChaseState { get; private set; }

    public State Attack1State { get; private set; }

    public override void Start()
    {
        base.Start();

        #region Intialization of states
        IdleState = new SpearEnemy_IdleState(IdleStateData);
        PatrolState = new SpearEnemy_PatrolState(PatrolStateData);
        ChaseState = new SpearEnemy_ChaseState(ChaseStateData);
        Attack1State = new SpearEnemy_Attack1State(Attack1StateData);
        #endregion

        #region Setting of state names 
        IdleState.SetStateName("IDLE");
        PatrolState.SetStateName("PATROL");
        ChaseState.SetStateName("CHASE");
        Attack1State.SetStateName("ATTACK1");
        #endregion

        #region Adding of states to state machine
        stateMachine.AddState(IdleState);
        stateMachine.AddState(PatrolState);
        stateMachine.AddState(ChaseState);
        stateMachine.AddState(Attack1State);
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
    
    public void TakeDamage(float amount)
    {
       throw new System.NotImplementedException();
    }

    public override void AttackVFX(int VFXAttackNo)
    {
        // since spear enemy only has 1 type of attack for now so therefore can ignore VFXAttackNo
        var vfx = Instantiate(ParticleManager.Instance.GetParticleEffect("SpearThrust"), VFXPos.position, Quaternion.identity);
        if (facingDirection < 0) vfx.transform.localScale = new Vector3(vfx.transform.localScale.x * -1, vfx.transform.localScale.y, vfx.transform.localScale.z);
        StartCoroutine(DestroyVFX(0.52f,vfx));
    }

    private IEnumerator DestroyVFX(float delay,GameObject vfx)
    {
        yield return new WaitForSeconds(delay);
        Destroy(vfx);
    }
    
    public override void HitConnected(int AttackNo)
    {
        // since spear enemy only has 1 type of attack for now so therefore can ignore AttackNo
        if (!PlayerController.Instance.GetIsBlocking())
            PlayerController.Instance._PlayerData.HealthData -= 40;
        else
            PlayerController.Instance._PlayerData.HealthData -= 8;
    }

}
