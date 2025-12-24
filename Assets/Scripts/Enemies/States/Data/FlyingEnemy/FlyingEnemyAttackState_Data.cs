using UnityEngine;

[CreateAssetMenu(fileName = "newFlyingAttackStateData", menuName = "Data/State Data/Flying Enemy/Flying Enemy Attack Data")]
public class FlyingEnemyAttackState_Data : ScriptableObject
{
    public float minAttackWindup;
    public float maxAttackWindup;
    public float dashSpeed;
    public float dashTime;
    public float damage;
}
