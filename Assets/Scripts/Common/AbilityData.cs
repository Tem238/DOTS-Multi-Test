using UnityEngine;

[CreateAssetMenu(menuName ="SO_AbilityData")]
public class AbilityData : ScriptableObject
{
    public int AbilityIndex;
    public GameObject AbilityPrefab;
    public int Damage;
    public float CoolDownTime;
    public float DestroyOnTimer;
    public float MoveSpeed;
}
