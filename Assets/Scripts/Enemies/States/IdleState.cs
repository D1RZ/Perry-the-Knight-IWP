using UnityEngine;

public abstract class IdleState : State
{
    protected bool flipAfterIdle;

    protected bool isIdleTimeOver;

    protected float idleTime;
}
