using UnityEngine;

[CreateAssetMenu(fileName = "newAttackStateData", menuName = "Data/State Data/Archer Enemy/Attack Data")]
public class ArcherEnemy_AttackState_Data : ScriptableObject
{
    public float minAttackCooldown;
    public float maxAttackCooldown;
}
