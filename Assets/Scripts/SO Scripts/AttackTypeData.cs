using UnityEngine;

[CreateAssetMenu (fileName = "AttackTypeData",menuName = "SO/AttackTypeData")]
public class AttackTypeData : ScriptableObject
{
    public string attackStepAnimVarName;
    public AttackStep[] attackSteps;
    public int maxAttackSteps;
    public string attackTypeBool;

    public void ApplyStepEffects(int stepIndex)
    {
        AttackStep step = attackSteps[stepIndex];

        if (step.applyForceToPlayer)
        {
            if (PlayerController.Instance.rb != null)
            {
                PlayerController.Instance.rb.constraints = RigidbodyConstraints2D.FreezeRotation;

                PlayerController.Instance.rb.AddForce(new Vector2(step.ForceToPlayerAmt.x,step.ForceToPlayerAmt.y), ForceMode2D.Impulse);
            }
        }
    }
}

[System.Serializable] 
public class AttackStep
{
    public float damage;
    public float attackAnimTime;
    public bool applyForceToPlayer;
    public Vector2 ForceToPlayerAmt;
    public Vector2 knockbackForce;
}
