using UnityEngine;

[CreateAssetMenu (fileName = "AttackTypeData",menuName = "SO/AttackTypeData")]
public class AttackTypeData : ScriptableObject
{
    public float[] attackStepDamages;
    public int maxAttackSteps;
    public string attackTypeBool;
}
