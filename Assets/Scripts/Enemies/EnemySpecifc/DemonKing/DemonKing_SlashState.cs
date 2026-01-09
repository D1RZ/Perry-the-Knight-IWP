using UnityEngine;

public class DemonKing_SlashState : State
{
    public override void Enter(Entity entity)
    {
        entity.SetVelocity(0);
        entity.Anim.speed = 1f;
        entity.Anim.Play("Demon_Slash");
        entity.CheckFacingDirectionBasedOnTargetPos();
        entity.hasFinishedAttack = false;
    }

    public override void onUpdate(Entity entity)
    {
        if (entity.hasFinishedAttack)
        {
            entity.Anim.speed = 1; // reset after animation ends
            entity.CheckFacingDirectionBasedOnTargetPos();
            entity.stateMachine.SetNextState("CHASE",entity);
            return;
        }
    }

    public override void Exit(Entity entity)
    {
        entity.hasFinishedAttack = false;
    }

}