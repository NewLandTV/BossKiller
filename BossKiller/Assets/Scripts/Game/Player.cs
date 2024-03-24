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

    // 보스
    [SerializeField]
    private Boss boss;

    // 크로스 헤어
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

    private IEnumerator Start()
    {
        while (true)
        {
            if (maxHp < hp)
            {
                hp = maxHp;
            }

            if (transform.position.y <= -2000f)
            {
                transform.position = Vector3.up * 15f;

                hp -= 100;
            }

            if (!dead)
            {
                if (hp <= 0f)
                {
                    dead = true;

                    mainCamera.fieldOfView = originVieldOfView;

                    crossHair.SetType(CrossHair.Type.Basic);

                    SoundManager.instance.StopBGM();
                }

                ZoomInFromGun();
                ChangeGun();
                UpdateRate();
                Fire();
                Jump();
                Crouch();
                Move();
                PlayerRotate();
                CameraRotate();
            }
            else
            {
                StartCoroutine(manager.Dead());
            }

            yield return null;
        }
    }

    private void ZoomInFromGun()
    {
        if (Input.GetMouseButtonDown(2) && guns[currentGunIndex].type == Gun.Type.SniperRifle)
        {
            toggleGunZoomIn = !toggleGunZoomIn;

            if (toggleGunZoomIn)
            {
                mainCamera.fieldOfView = guns[currentGunIndex].zoomInFOV;

                crossHair.SetType(CrossHair.Type.ZoomIn);

                return;
            }

            mainCamera.fieldOfView = originVieldOfView;

            crossHair.SetType(CrossHair.Type.Basic);
        }
    }

    private void ChangeGun()
    {
        int gunIndex = currentGunIndex;

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            currentGunIndex = 0;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            currentGunIndex = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            currentGunIndex = 2;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            currentGunIndex = 3;
        }

        if (gunIndex != currentGunIndex)
        {
            if (toggleGunZoomIn)
            {
                mainCamera.fieldOfView = originVieldOfView;

                crossHair.SetType(CrossHair.Type.Basic);
            }
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
        if (Input.GetMouseButton(0) && guns[currentGunIndex].currentRate <= 0f)
        {
            guns[currentGunIndex].currentRate = guns[currentGunIndex].rate;

            if (guns[currentGunIndex].type != Gun.Type.SniperRifle)
            {
                manager.SpawnBullet(guns[currentGunIndex].damage * (manager.score + 1), guns[currentGunIndex].speed, guns[currentGunIndex].bulletColor);

                return;
            }

            // Gun.Type is Sniper Rifle
            if (Physics.Raycast(transform.position + Vector3.up * 0.5f, mainCamera.transform.forward, guns[currentGunIndex].speed, 11))
            {
                boss.OnDamage(guns[currentGunIndex].damage * (manager.score + 1));
            }

            cameraRotateX -= 70f;
        }
    }

    private void Jump()
    {
        applyJumpForce = Input.GetKey(KeyCode.LeftControl) ? superJumpForce : jumpForce;

        if (Input.GetKeyDown(KeyCode.Space) && !isJump && !isCrouch && !toggleGunZoomIn)
        {
            isJump = true;

            rigid.AddForce(Vector3.up * applyJumpForce, ForceMode.Impulse);
        }
    }

    private void Crouch()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && !isJump)
        {
            if (isCrouch = !isCrouch)
            {
                applySpeed = crouchSpeed;

                transform.localScale = Vector3.one;
            }
            else
            {
                applySpeed = walkSpeed;

                transform.localScale = Vector3.one + Vector3.up;
            }
        }
    }

    private void Move()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        if (!isCrouch && !toggleGunZoomIn)
        {
            applySpeed = Input.GetKey(KeyCode.LeftControl) ? runSpeed : walkSpeed;
        }

        transform.position += ((transform.right * h) + (transform.forward * v)).normalized * applySpeed * Time.deltaTime;
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
        if (other.CompareTag("Missile"))
        {
            hp -= other.GetComponent<Missile>().damage * (manager.score + 1);

            other.gameObject.SetActive(false);
        }
    }
}
