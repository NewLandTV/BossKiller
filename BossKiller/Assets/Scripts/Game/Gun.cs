using UnityEngine;

[CreateAssetMenu(fileName = "Gun", menuName = "New/Gun", order = 1)]
public class Gun : ScriptableObject
{
    public string name;
    public float speed;
    public int damage;
    public float rate;
    public float currentRate;
    public Color bulletColor;
    public Type type;
    public float zoomInFOV;

    public enum Type
    {
        Rifle,
        AssaultRifle,
        SniperRifle,
        Flamethrower
    }
}
