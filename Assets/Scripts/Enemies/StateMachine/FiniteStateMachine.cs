using System.Collections.Generic;
using UnityEngine;

public class FiniteStateMachine : MonoBehaviour
{
    private Dictionary<string, State> states = new Dictionary<string, State>(); // to store each state class to a string e.g if there are 2 states in the dictionary like walk and run then the dictionary would be <walk,WalkState> and <run,RunState>

    public void AddState(State newState)
    {
        if (states.Count != 0)
        {
            if (!states.ContainsKey(newState.GetStateName()))
            {
                states.Add(newState.GetStateName(), newState);
            }
        }
        else
        {
            states.Add(newState.GetStateName(), newState);
        }
    }

    public void SetNextState(string nextStateName, Entity enemy)
    {
        if (states.ContainsKey(nextStateName))
        {
            enemy.SetNextState(states[nextStateName]);
        }
    }

    public void OnUpdate(float dt, Entity enemy)
    {
        if (enemy.GetNextState() != enemy.GetCurrentState())
        {
            if (enemy.GetCurrentState() != null) enemy.GetCurrentState().Exit(enemy);
            enemy.SetCurrentState(enemy.GetNextState());
            enemy.GetCurrentState().Enter(enemy);
        }

        if (enemy.GetCurrentState() != null) enemy.GetCurrentState().onUpdate(enemy);
    }

    // for transitioning back to same state
    public void ForceSetNextState(string nextStateName, Entity enemy)
    {
        if (states.ContainsKey(nextStateName))
        {
            // Force exit current state and enter the same state again
            if (enemy.GetCurrentState() != null)
                enemy.GetCurrentState().Exit(enemy);

            enemy.SetCurrentState(states[nextStateName]);
            enemy.GetCurrentState().Enter(enemy);
        }
    }

}
