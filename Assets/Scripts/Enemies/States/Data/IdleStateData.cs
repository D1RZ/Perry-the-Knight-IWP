using UnityEngine;

[CreateAssetMenu(fileName = "newIdleStateData", menuName = "Data/State Data/Idle Data")]
public class IdleState_Data : ScriptableObject
{
    public float minIdleTime = 1f;
    public float maxIdleTime = 2f;
}
