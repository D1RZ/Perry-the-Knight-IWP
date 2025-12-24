using UnityEngine;

[CreateAssetMenu(fileName = "newFlyingIdleStateData", menuName = "Data/State Data/Flying Enemy/Flying Enemy Idle Data")]
public class FlyingEnemyIdleState_Data : ScriptableObject
{
    public float minHoverAmplitude = -0.5f;
    public float maxHoverAmplitude = 0.5f;
    public float hoverSpeed = 1f;
}
