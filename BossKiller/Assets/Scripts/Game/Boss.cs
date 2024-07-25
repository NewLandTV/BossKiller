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
            if (CheckDead())
            {
                yield return waitTime3f;

                meshRenderer.enabled = false;

                yield return waitTime2f;

                ResetBossState();

                continue;
            }

            CheckHP();

            MoveAndRotate();
            SpawnMissile();

            TryJump();
            TryDash();

            yield return null;
        }
    }

    private void LateUpdate()
    {
        SetHealthBar();
        CheckPosition();
    }

    private bool CheckDead()
    {
        if (!dead)
        {
            return false;
        }

        meshRenderer.material.color = Color.gray;
        boxCollider.enabled = false;

        rigid.AddForce(Vector3.up * 50f, ForceMode.Impulse);

        return true;
    }

    private void CheckHP()
    {
        if (hp > 0f)
        {
            timer += Time.deltaTime;

            return;
        }

        dead = true;
        isChangeState = false;

        GameObject deadEffect = Instantiate(deadEffectPrefab, transform.position, Quaternion.identity);

        Destroy(deadEffect, 1f);
    }

    private void MoveAndRotate()
    {
        int x = Random.Range(-1, 2);
        int z = Random.Range(-1, 2);

        transform.position += new Vector3(x, 0f, z) * speed * Time.deltaTime;
        transform.eulerAngles += Vector3.up * (missileSpawnDirectionRight ? 3.6f : -3.6f);
    }

    private void SpawnMissile()
    {
        float rate = Random.Range(0.0025f, 0.025f);

        if (timer < rate)
        {
            return;
        }

        timer -= rate;

        manager.SpawnMissile(damage);
    }

    private void TryJump()
    {
        if (isJump || Random.Range(0, 4) != 2)  // 25%
        {
            return;
        }

        isJump = true;

        float jumpForce = Random.Range(2.5f, 6.5f);

        rigid.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        if (jumpForce < 3.75f)
        {
            return;
        }

        missileSpawnDirectionRight = !missileSpawnDirectionRight;

        WaitForSeconds wait = new WaitForSeconds(jumpForce - 2.25f);

        StartCoroutine(ForceGround(wait));
    }

    private void TryDash()
    {
        if (!isJump || Random.Range(0, 120) != 2)   // 0.833333%
        {
            return;
        }

        Vector3 direction = player.transform.position - transform.position;

        rigid.AddForce(direction.normalized * dashPower, ForceMode.Impulse);
    }

    private void ResetBossState()
    {
        if (isChangeState)
        {
            return;
        }

        isChangeState = true;

        manager.score += 50 * (++count);

        nextHp += originHp * manager.score + 10;
        nextDamage += originDamage * 2;

        hp = nextHp;
        damage = nextDamage;
        meshRenderer.material = mat;

        transform.SetPositionAndRotation(Vector3.up * 4f, Quaternion.identity);

        player.hp += count * manager.score;

        meshRenderer.enabled = true;
        boxCollider.enabled = true;

        dead = false;
    }

    private void SetHealthBar()
    {
        float curHP = (float)hp / nextHp;

        Vector3 pos = worldSpaceUI.transform.position + mainCameraTransform.forward;

        healthBar.fillAmount = curHP;

        worldSpaceUI.transform.LookAt(pos);
        healthBar.color = healthBarGradient.Evaluate(curHP);
    }

    private void CheckPosition()
    {
        if (transform.position.y <= -2000f)
        {
            transform.position = Vector3.up * 10f;
        }
    }

    private IEnumerator ForceGround(WaitForSeconds waitTime)
    {
        yield return waitTime;

        rigid.AddForce(Vector3.down * 25f, ForceMode.Impulse);
    }

    public void TakeDamage(int damage) => hp -= damage;

    private void OnCollisionStay(Collision collision) => isJump = false;

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Bullet"))
        {
            return;
        }

        GameObject hitParticle = Instantiate(hitParticlePrefab, transform.position, Quaternion.identity);

        Destroy(hitParticle, 3f);
        TakeDamage(other.GetComponent<Bullet>().Damage);

        other.gameObject.SetActive(false);
    }
}
