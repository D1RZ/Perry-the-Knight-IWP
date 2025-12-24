using UnityEngine;

[CreateAssetMenu(fileName = "newAttack1StateData", menuName = "Data/State Data/Spear Enemy/Attack 1 Data")]
public class SpearEnemy_AttackState_Data : ScriptableObject
{
    public float Damage;
    public int attackCounts;
    public float attackRadius;
}
