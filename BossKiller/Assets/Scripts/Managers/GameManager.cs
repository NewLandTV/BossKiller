using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Object Pooling")]

    // 총알 오브젝트 풀링
    private List<Bullet> bullets = new List<Bullet>();
    private int cursor;
    [Range(1, 100000)]
    [SerializeField]
    private int makeCount;
    [SerializeField]
    private GameObject bulletObj;
    [SerializeField]
    private Transform bulletSpawnTransform;

    // 미사일 오브젝트 풀링
    private List<Missile> missiles = new List<Missile>();
    private int cursor_Missile;
    [Range(1, 100000)]
    [SerializeField]
    private int makeCount_Missile;
    [SerializeField]
    private GameObject missileObj;
    [SerializeField]
    private Transform missileSpawnTransform;

    [Header("Player Status")]

    // 플레이어의 현재 스텟 UI
    [SerializeField]
    private Text hpText;
    [SerializeField]
    private Image healthBarImage;
    [SerializeField]
    private Text scoreText;
    [SerializeField]
    private Text coolDownText;
    [SerializeField]
    private Image coolDownImage;
    [SerializeField]
    private Text speedText;

    [SerializeField]
    private Gradient healthBarGradient;

    public GameObject[] deadObjects;

    // 게임 종료시 나오는 텍스트
    private Text deadScoreText;
    private Text deadMaxScoreText;

    [HideInInspector]
    public int score;
    
    [Header("World")]

    [SerializeField]
    private Player player;
    [SerializeField]
    private Boss boss;

    private Camera mainCamera;

    private void Awake()
    {
        SoundManager.instance.PlayBGM("Desert Adventure", true);

        deadScoreText = deadObjects[1].GetComponent<Text>();
        deadMaxScoreText = deadObjects[2].GetComponent<Text>();

        for (int i = 0; i < makeCount; i++)
        {
            MakeBullet();
        }
        for (int i = 0; i < makeCount_Missile; i++)
        {
            MakeMissile();
        }

        mainCamera = Camera.main;
    }

    private IEnumerator Start()
    {
        while (true)
        {
            // 플레이어 스텟 반연
            hpText.text = string.Format("HP : {0:n0}", player.hp);
            healthBarImage.fillAmount = (float)player.hp / player.maxHp;

            healthBarImage.color = healthBarGradient.Evaluate((float)player.hp / player.maxHp);

            scoreText.text = string.Format("Score : {0:n0}", score);
            coolDownText.text = string.Format("Cool Down : {0:n2}s", player.guns[player.currentGunIndex].currentRate);
            coolDownImage.fillAmount = player.guns[player.currentGunIndex].currentRate / player.guns[player.currentGunIndex].rate;

            speedText.text = string.Format("Speed : {0:n3}", player.ApplySpeed);

            deadScoreText.text = string.Format("Score : {0:n0}", score);

            // 최고 기록 갱신
            if (PlayerPrefs.HasKey("MaxScore"))
            {
                deadMaxScoreText.text = string.Format("Max Score : {0:n0}", PlayerPrefs.GetInt("MaxScore"));
            }

            yield return null;
        }
    }

    public void SpawnBullet(int p_dmg, float p_spd, Color p_clr)
    {
        if (CheckBulletEnable())
        {
            MakeBullet();
        }
        else
        {
            if (!bullets[cursor].initialized)
            {
                bullets[cursor].Initialize();
            }

            bullets[cursor].transform.position = player.transform.position + Vector3.up * 0.5f;
            bullets[cursor].transform.rotation = Quaternion.Euler(new Vector3(mainCamera.transform.eulerAngles.x, player.transform.eulerAngles.y, 0f));
            bullets[cursor].damage = p_dmg;
            bullets[cursor].speed = p_spd;
            bullets[cursor].Color = p_clr;

            bullets[cursor].gameObject.SetActive(true);

            cursor = (cursor + 1) % bullets.Count;
        }
    }

    private Bullet MakeBullet()
    {
        GameObject bullet = Instantiate(bulletObj, transform.position, Quaternion.identity);

        bullet.transform.SetParent(bulletSpawnTransform);

        Bullet bulletScript = bullet.GetComponent<Bullet>();

        bullets.Add(bulletScript);

        return bulletScript;
    }

    private bool CheckBulletEnable()
    {
        for (int i = 0; i < bullets.Count; i++)
        {
            if (!bullets[i].gameObject.activeSelf)
            {
                return false;
            }
        }

        return true;
    }

    public void SpawnMissile(int p_dmg)
    {
        if (CheckMissileEnable())
        {
            MakeMissile();
        }
        else
        {
            missiles[cursor_Missile].transform.position = boss.transform.position;
            missiles[cursor_Missile].transform.rotation = boss.transform.rotation;
            missiles[cursor_Missile].damage = p_dmg;

            missiles[cursor_Missile].gameObject.SetActive(true);

            cursor_Missile = (cursor_Missile + 1) % missiles.Count;
        }
    }

    private Missile MakeMissile()
    {
        GameObject missile = Instantiate(missileObj, transform.position, Quaternion.identity);

        missile.transform.SetParent(missileSpawnTransform);

        Missile missileScript = missile.GetComponent<Missile>();

        missileScript.Initialize();

        missiles.Add(missileScript);

        return missileScript;
    }

    private bool CheckMissileEnable()
    {
        for (int i = 0; i < missiles.Count; i++)
        {
            if (!missiles[i].gameObject.activeSelf)
            {
                return false;
            }
        }

        return true;
    }

    public IEnumerator Dead()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (PlayerPrefs.GetInt("MaxScore") < score)
        {
            PlayerPrefs.SetInt("MaxScore", score);
        }

        for (int i = 0; i < deadObjects.Length; i++)
        {
            deadObjects[i].SetActive(true);

            yield return new WaitForSeconds(0.7f);
        }
    }

    public void Retry()
    {
        SoundManager.instance.StopBGM();

        Loading.LoadScene(Scenes.Title);
    }

    public void Quit() => Application.Quit();
}
