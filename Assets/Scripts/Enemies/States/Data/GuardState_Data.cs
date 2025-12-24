using UnityEngine;

[CreateAssetMenu(fileName = "newGuardStateData", menuName = "Data/State Data/Guard Data")]
public class GuardState_Data : ScriptableObject
{
    public float minGuardTime = 0.8f;
    public float maxGuardTime = 1.5f;
}
