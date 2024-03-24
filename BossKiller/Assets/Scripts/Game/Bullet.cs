using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed;
    public int damage;

    public bool initialized;

    protected MeshRenderer meshRenderer;
    protected Rigidbody rigid;

    public Color Color
    {
        get
        {
            return meshRenderer.material.color;
        }
        set
        {
            meshRenderer.material.color = value;
        }
    }

    // Wait For Seconds
    protected WaitForSeconds waitTime5f;
    protected WaitForFixedUpdate waitTimeFixedUpdate;

    private IEnumerator Start()
    {
        Initialize();
        StartCoroutine(Disable());

        while (true)
        {
            Tick();

            yield return waitTimeFixedUpdate;
        }
    }

    protected virtual void Tick() => rigid.AddForce(rigid.position + rigid.transform.forward * speed, ForceMode.Force);

    protected virtual IEnumerator Disable()
    {
        yield return waitTime5f;

        gameObject.SetActive(false);

        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
    }

    public void Initialize()
    {
        initialized = true;

        meshRenderer = GetComponent<MeshRenderer>();
        rigid = GetComponent<Rigidbody>();

        waitTime5f = new WaitForSeconds(5f);
        waitTimeFixedUpdate = new WaitForFixedUpdate();
    }
}
