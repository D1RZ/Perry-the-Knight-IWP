using UnityEngine;
[CreateAssetMenu(fileName = "entityData", menuName = "Data/ Entity Data/Base Data")]

public class D_Entity : ScriptableObject
{
    public float MaxHealth;

    public float wallCheckDistance = 0.2f;

    public float ledgeCheckDistance = 0.4f;

    public float attackDetectionRadius = 0.15f;

    public float chaseRaycastDistance = 0.5f;
}
