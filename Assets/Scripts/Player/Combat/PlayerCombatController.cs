using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatController : MonoBehaviour
{
    private int attackStep = 0;

    public List<StringAttackTypeDataPair> attackTypesData;

    private AttackTypeData currentAttackType;

    private Animator animator;

    private Coroutine lastAttackCoroutine = null;

    private float damage;

    private static PlayerCombatController _instance;

    private Vector2 knockbackForce;

    public static PlayerCombatController Instance
    {
        get
        {
            if (_instance == null) Debug.Log("GameManager is null");

            return _instance;
        }
    }

    private string previousAttackType;

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        PlayerController.OnPlayerReady += Register;
    }

    private void HandleAttack(string attackType)
    {
        // Prevent attack spam
        if (lastAttackCoroutine != null)
        {
            return; // Don't start new attack if one is already in progress
        }
        
        lastAttackCoroutine = StartCoroutine(StartHandleAttack(attackType));
    }

    IEnumerator StartHandleAttack(string attackType)
    {
        // First, find and set the current attack type
        FindAttackType(attackType);

        Debug.Log("ATTACK TYPE:" + attackType);

        // Ensure attackStep is within bounds
        if (currentAttackType != null)
        {
            attackStep = (attackStep < currentAttackType.maxAttackSteps) ? attackStep + 1 : 1;

            // Set the damage for this step
            float stepDamage = currentAttackType.attackSteps[attackStep - 1].damage; // -1 if your array is 0-based
            SetAttackDamage(stepDamage);

            knockbackForce = currentAttackType.attackSteps[attackStep - 1].knockbackForce;
        }

            if (currentAttackType.attackStepAnimVarName != "nil")
        animator.SetInteger(currentAttackType.attackStepAnimVarName, attackStep);

            if (attackType == "Air Attack")
        Debug.Log("Air Attack Step: " + animator.GetInteger("AirAttackStep"));

        animator.SetBool(currentAttackType.attackTypeBool, true);

        currentAttackType.ApplyStepEffects(attackStep - 1);

        yield return new WaitForSeconds(currentAttackType.attackSteps[attackStep - 1].attackAnimTime);

        animator.SetBool(currentAttackType.attackTypeBool, false);

        PlayerController.Instance.EndAttack();

        lastAttackCoroutine = null;
    }

    private void FindAttackType(string attackType)
    {
        Debug.Log("Finding attack type " + attackType);

        foreach(StringAttackTypeDataPair attackTypeDataPair in attackTypesData)
        {
            if(attackTypeDataPair.key.Equals(attackType))
            {
                if (previousAttackType != null && attackType != previousAttackType && currentAttackType != null) attackStep = 0; // reset attack step for every new type of attack the player attacks with

                previousAttackType = attackType;
                currentAttackType = attackTypeDataPair.value;
                return;
            }
        }

        currentAttackType = null; // if attack type not found
    }

    public void SetAttackDamage(float Damage)
    {
        damage = Damage;
    }

    public float GetAttackDamage()
    {
        return damage;
    }

    public Vector2 GetKnockbackForce()
    {
        return knockbackForce;
    }

}
