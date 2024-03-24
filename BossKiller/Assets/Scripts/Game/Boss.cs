using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Boss : MonoBehaviour
{
    public int hp;
    public int damage;

    private int originHp;
    private int originDamage;

    private int nextHp;
    private int nextDamage;

    [SerializeField]
    private float speed;

    [SerializeField]
    private float dashPower;

    private bool dead;
    private bool isChangeState;

    private float timer;
    private int count;

    // Flags
    private bool isJump;
    private bool missileSpawnDirectionRight;

    private Player player;
    private MeshRenderer meshRenderer;
    private BoxCollider boxCollider;
    public Material mat;
    public GameManager manager;
    private Rigidbody rigid;

    [SerializeField]
    private Image healthBar;
    [SerializeField]
    private Gradient healthBarGradient;
    [SerializeField]
    private Transform worldSpaceUI;

    [SerializeField]
    private GameObject hitParticlePrefab;
    [SerializeField]
    private GameObject deadEffectPrefab;

    // Wait For Seconds
    private WaitForSeconds waitTime2f;
    private WaitForSeconds waitTime3f;

    private Transform mainCameraTransform;

    private void Awake()
    {
        originHp = nextHp = hp;
        originDamage = damage;

        // Component Initialized
        player = FindObjectOfType<Player>();
        meshRenderer = GetComponent<MeshRenderer>();
        boxCollider = GetComponent<BoxCollider>();
        rigid = GetComponent<Rigidbody>();

        waitTime2f = new WaitForSeconds(2f);
        waitTime3f = new WaitForSeconds(3f);

        mainCameraTransform = Camera.main.transform;
    }

    private IEnumerator Start()
    {
        while (true)
        {
            SetHealthBar();

            if (transform.position.y < -2000f)
            {
                transform.position = Vector3.up * 10f;
            }

            if (!dead)
            {
                // Move and Rotate
                transform.position += new Vector3(Random.Range(-1, 2), 0f, Random.Range(-1, 2)) * speed * Time.deltaTime;
                transform.eulerAngles += Vector3.up * (missileSpawnDirectionRight ? 3.6f : -3.6f);

                // Check health
                if (hp <= 0f)
                {
                    dead = true;
                    isChangeState = false;

                    Destroy(Instantiate(deadEffectPrefab, transform.position, Quaternion.identity), 1f);
                }

                timer += Time.deltaTime;

                // Spawn Missile
                if (timer >= Random.Range(0.0025f, 0.025f))
                {
                    timer = 0f;

                    manager.SpawnMissile(damage);
                }

                // Jump
                if (!isJump && Random.Range(0, 4) == 2)
                {
                    isJump = true;

                    float jumpForce = Random.Range(2.5f, 6.5f);

                    rigid.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

                    if (jumpForce >= 3.75f)
                    {
                        missileSpawnDirectionRight = !missileSpawnDirectionRight;

                        StartCoroutine(ForceGround(new WaitForSeconds(jumpForce - 2.25f)));
                    }
                }

                // Dash
                if (isJump && Random.Range(0, 120) == 2)
                {
                    rigid.AddForce((player.transform.position - transform.position).normalized * dashPower, ForceMode.Impulse);
                }
            }
            else
            {
                meshRenderer.material.color = Color.gray;
                boxCollider.enabled = false;

                rigid.AddForce(Vector3.up * 50f, ForceMode.Impulse);

                yield return waitTime3f;

                meshRenderer.enabled = false;

                yield return waitTime2f;

                ResetBossState();
            }

            yield return null;
        }
    }

    private void ResetBossState()
    {
        if (!isChangeState)
        {
            isChangeState = true;

            count++;
            manager.score += 50 * count;
            nextHp += originHp * manager.score + 10;
            nextDamage += originDamage * 2;
            hp = nextHp;
            damage = nextDamage;
            meshRenderer.material = mat;
            transform.position = Vector3.up * 4f;
            transform.rotation = Quaternion.identity;
            player.hp += count * manager.score;
            meshRenderer.enabled = true;
            boxCollider.enabled = true;

            dead = false;
        }
    }

    private void SetHealthBar()
    {
        float current = (float)hp / nextHp;

        worldSpaceUI.transform.LookAt(worldSpaceUI.transform.position + mainCameraTransform.forward);

        healthBar.fillAmount = current;

        healthBar.color = healthBarGradient.Evaluate(current);
    }

    private IEnumerator ForceGround(WaitForSeconds waitTime)
    {
        yield return waitTime;

        rigid.AddForce(Vector3.down * 25f, ForceMode.Impulse);
    }

    public void OnDamage(int damage)
    {
        hp -= damage;
    }

    private void OnCollisionStay(Collision collision) => isJump = false;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            Destroy(Instantiate(hitParticlePrefab, transform.position, Quaternion.identity), 3f);

            hp -= other.GetComponent<Bullet>().damage;

            other.gameObject.SetActive(false);
        }
    }
}
