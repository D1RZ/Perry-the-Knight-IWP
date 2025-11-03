using UnityEngine;

[CreateAssetMenu(fileName = "newChaseStateData",menuName = "Data/State Data/Spear Enemy/Chase Data")]
public class SpearEnemy_ChaseState_Data : ScriptableObject
{
    public float ChaseMovementSpeed;
    public float MaxChaseDistance;
}
