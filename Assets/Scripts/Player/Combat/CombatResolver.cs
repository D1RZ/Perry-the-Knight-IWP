using UnityEngine;

public static class CombatResolver
{
    public static bool enemyAttack = false;
    public static bool playerAttack = false;

    public static void EnemyAttacked()
    {
        enemyAttack = true;
    }

    public static void PlayerAttacked()
    {
        playerAttack = true;
    }

    public static bool CheckParry()
    {
        if (enemyAttack && playerAttack) return true;

        return false;
    }

}