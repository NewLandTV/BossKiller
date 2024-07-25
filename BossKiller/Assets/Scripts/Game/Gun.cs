using UnityEngine;

[CreateAssetMenu(fileName = "Gun", menuName = "New/Gun", order = 1)]
public class Gun : ScriptableObject
{
    public enum Type
    {
        Rifle = 0,
        AssaultRifle = 1,
        SniperRifle = 2,
        Flamethrower = 3
    }

    public new string name;
    public float speed;
    public int damage;
    public float rate;
    public float currentRate;
    public Color bulletColor;
    public Type type;
    public float zoomInFOV;
}
