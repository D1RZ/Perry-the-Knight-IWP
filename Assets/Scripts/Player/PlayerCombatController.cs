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

    [SerializeField] private PlayerAttackChecker playerAttackChecker;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        PlayerController.OnPlayerAttack += HandleAttack;
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

        // Check if we have a valid attack type and if we can increment
        if (currentAttackType != null && attackStep < currentAttackType.maxAttackSteps)
        {
            attackStep += 1;
        }
        else
        {
            // Reset to step 1 (or 0 if you prefer 0-based indexing)
            attackStep = 1;
        }

        animator.SetInteger("attackStep", attackStep);
        animator.SetBool(currentAttackType.attackTypeBool, true);

        yield return new WaitForSeconds(0.1f);

        animator.SetBool(currentAttackType.attackTypeBool, false);

        lastAttackCoroutine = null;
    }

    private void FindAttackType(string attackType)
    {
        foreach(StringAttackTypeDataPair attackTypeDataPair in attackTypesData)
        {
            if(attackTypeDataPair.key.Equals(attackType))
            {
                currentAttackType = attackTypeDataPair.value;
                return;
            }
        }

        currentAttackType = null; // if attack type not found
    }

    public void SetAttackDamage(float Damage)
    {
        playerAttackChecker.SetDamage(Damage);
    }

    public float GetAttackDamage()
    {
        return playerAttackChecker.GetDamage();
    }

}
