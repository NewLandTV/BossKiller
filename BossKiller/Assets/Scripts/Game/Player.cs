using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int hp;
    public int maxHp = 100000;
    public float crouchSpeed;
    public float walkSpeed;
    public float runSpeed;
    private float applySpeed;
    public float ApplySpeed => applySpeed;

    public float Sensitivity;
    private float mouseX;
    private float mouseY;
    private float cameraRotateX;
    public float jumpForce;
    public float superJumpForce;
    private float applyJumpForce;

    private bool isJump;
    private bool isCrouch;
    private bool dead;
    private bool toggleGunZoomIn;

    public Gun[] guns;
    [HideInInspector]
    public int currentGunIndex;

    private Rigidbody rigid;
    public GameManager manager;

    private Camera mainCamera;

    [SerializeField]
    private Boss boss;

    [SerializeField]
    private CrossHair crossHair;

    // 카메라 정보
    private float originVieldOfView;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        mainCamera = Camera.main;

        originVieldOfView = mainCamera.fieldOfView;
    }

    private void Update()
{
        if (maxHp < hp)
        {
            hp = maxHp;
        }

        CheckPosition();

        if (dead)
        {
            StartCoroutine(manager.Dead());

            return;
        }

        CheckHP();
        ZoomInFromGun();
        ChangeGun();
        UpdateRate();
        Fire();
        TryJump();
        TryCrouch();
        Move();
        PlayerRotate();
        CameraRotate();
    }

    private void CheckPosition()
    {
        if (transform.position.y <= -2000f)
        {
            transform.position = Vector3.up * 15f;
            hp -= 100;
        }
    }

    private void CheckHP()
    {
        if (hp > 0f)
        {
            return;
        }

        dead = true;

        mainCamera.fieldOfView = originVieldOfView;

        crossHair.SetType(CrossHair.Type.Basic);

        SoundManager.Instance.StopBGM();
    }

    private void ZoomInFromGun()
    {
        bool canZoomIn = Input.GetMouseButtonDown(2) && guns[currentGunIndex].type == Gun.Type.SniperRifle;

        if (!canZoomIn)
        {
            return;
        }

        if (toggleGunZoomIn = !toggleGunZoomIn)
        {
            mainCamera.fieldOfView = guns[currentGunIndex].zoomInFOV;

            crossHair.SetType(CrossHair.Type.ZoomIn);

            return;
        }

        mainCamera.fieldOfView = originVieldOfView;

        crossHair.SetType(CrossHair.Type.Basic);
    }

    private void ChangeGun()
    {
        int gunIndex = currentGunIndex;

        for (int i = 0; i < 4; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                currentGunIndex = i;

                break;
            }
        }

        if (gunIndex == currentGunIndex)
        {
            return;
        }

        if (toggleGunZoomIn)
        {
            mainCamera.fieldOfView = originVieldOfView;

            crossHair.SetType(CrossHair.Type.Basic);
        }
    }

    private void UpdateRate()
    {
        guns[currentGunIndex].currentRate -= Time.deltaTime;

        if (guns[currentGunIndex].currentRate < 0f)
        {
            guns[currentGunIndex].currentRate = 0f;
        }
    }

    private void Fire()
    {
        bool canFire = Input.GetMouseButton(0) && guns[currentGunIndex].currentRate <= 0f;

        if (!canFire)
        {
            return;
        }

        guns[currentGunIndex].currentRate = guns[currentGunIndex].rate;

        if (guns[currentGunIndex].type != Gun.Type.SniperRifle)
        {
            manager.SpawnBullet(guns[currentGunIndex].damage * (manager.score + 1), guns[currentGunIndex].speed, guns[currentGunIndex].bulletColor);

            return;
        }

        // Gun.Type is Sniper Rifle
        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, mainCamera.transform.forward, guns[currentGunIndex].speed, 11))
        {
            boss.TakeDamage(guns[currentGunIndex].damage * (manager.score + 1));
        }

        cameraRotateX -= 70f;
    }

    private void TryJump()
    {
        applyJumpForce = Input.GetKey(KeyCode.LeftControl) ? superJumpForce : jumpForce;

        bool canJump = Input.GetKeyDown(KeyCode.Space) && !isJump && !isCrouch && !toggleGunZoomIn;

        if (!canJump)
        {
            return;
        }

        isJump = true;

        rigid.AddForce(Vector3.up * applyJumpForce, ForceMode.Impulse);
    }

    private void TryCrouch()
    {
        if (isJump || !Input.GetKeyDown(KeyCode.LeftShift))
        {
            return;
        }

        if (isCrouch = !isCrouch)
        {
            applySpeed = crouchSpeed;
            transform.localScale = Vector3.one;

            return;
        }

        applySpeed = walkSpeed;
        transform.localScale = Vector3.one + Vector3.up;
    }

    private void Move()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 direction = ((transform.right * h) + (transform.forward * v)).normalized;

        if (!isCrouch && !toggleGunZoomIn)
        {
            applySpeed = Input.GetKey(KeyCode.LeftControl) ? runSpeed : walkSpeed;
        }

        transform.position += direction * applySpeed * Time.deltaTime;
    }

    private void PlayerRotate()
    {
        mouseX = Input.GetAxis("Mouse X");
        transform.eulerAngles += new Vector3(0f, mouseX * Sensitivity, 0f);
    }

    private void CameraRotate()
    {
        mouseY = Input.GetAxis("Mouse Y");

        cameraRotateX -= mouseY * Sensitivity;
        cameraRotateX = Mathf.Clamp(cameraRotateX, -85f, 85f);

        mainCamera.transform.eulerAngles = new Vector3(cameraRotateX, transform.eulerAngles.y, 0f);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.CompareTag("Floor") && rigid.velocity.y <= 0f)
        {
            isJump = false;
        }
        else if (collision.collider.CompareTag("Boss"))
        {
            hp -= collision.collider.GetComponent<Boss>().damage * 10;

            rigid.AddForce((transform.position - collision.transform.position).normalized * 16f, ForceMode.Impulse);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Missile"))
        {
            return;
        }

        hp -= other.GetComponent<Missile>().Damage * (manager.score + 1);

        other.gameObject.SetActive(false);
    }
}
