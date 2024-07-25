using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float Speed { get; private set; }
    public int Damage { get; private set; }

    public bool initialized;

    protected MeshRenderer meshRenderer;
    protected Rigidbody rigid;

    public Color Color
    {
        get => meshRenderer.material.color;
        private set => meshRenderer.material.color = value;
    }

    // Wait For Seconds
    protected WaitForSeconds waitTime5f;

    private void Awake() => Initialize();

    private void Start() => StartCoroutine(Disable());

    private void FixedUpdate() => Tick();

    protected virtual void Tick() => rigid.AddForce(rigid.position + rigid.transform.forward * Speed, ForceMode.Force);

    public void Initialize()
    {
        initialized = true;

        meshRenderer = GetComponent<MeshRenderer>();
        rigid = GetComponent<Rigidbody>();

        waitTime5f = new WaitForSeconds(5f);
    }

    public void Set(float speed, int damage, Color color)
    {
        Speed = speed;
        Damage = damage;
        Color = color;
    }

    public void SetDamage(int damage) => Damage = damage;

    protected virtual IEnumerator Disable()
    {
        yield return waitTime5f;

        gameObject.SetActive(false);
        transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
    }
}
