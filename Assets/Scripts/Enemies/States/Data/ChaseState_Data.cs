using UnityEngine;

[CreateAssetMenu(fileName = "newChaseStateData", menuName = "Data/State Data/Chase Data")]
public class ChaseState_Data : ScriptableObject
{
    public float ChaseMovementSpeed;
    public float MaxChaseDistance;
    public float VerticalCorrectionSpeed;
}