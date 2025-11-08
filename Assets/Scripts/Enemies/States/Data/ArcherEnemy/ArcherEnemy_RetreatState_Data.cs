using UnityEngine;

[CreateAssetMenu(fileName = "newRetreatStateData", menuName = "Data/State Data/Archer Enemy/Retreat Data")]
public class ArcherEnemy_RetreatState_Data : ScriptableObject
{
    public Vector2 jumpForce;
    public float minRetreatDistance;
    public float retreatDuration;
    public float retreatCooldown;
}
