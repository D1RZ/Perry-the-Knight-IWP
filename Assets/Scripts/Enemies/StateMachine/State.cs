using System.Collections;

public abstract class State
{
    protected string StateName;

    public abstract void Enter(Entity entity); // Stores enter state for enemy state class example is NormalZombieIdle class then Enter funcion is also defined in NormalZombieIdle class

    public abstract void onUpdate(Entity entity); // Stores update state for enemy state class example is NormalZombieIdle class then update funcion is also defined in NormalZombieIdle class

    public abstract void Exit(Entity entity); // Stores exit state for enemy state class example is NormalZombieIdle class then exit function is also defined in NormalZombieIdle class

    public string GetStateName()
    {
        return StateName;
    }

    public void SetStateName(string stateName)
    {
        StateName = stateName;
    }

    public void StartStateCoroutine(Entity entity, IEnumerator routine)
    {
        entity.StartCoroutineFromState(routine);
    }

}
