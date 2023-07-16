using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Unit")]
public class UnitSO : ScriptableObject
{
    public string nameString;
    public int unitID;
    public Sprite unitSprite;
    public float unitTrainTime;
    public int unitPrice;
    public int unitHealth;
    public int power;
    public int fireRate;
    public float walkSpeed;
}