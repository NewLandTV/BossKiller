using UnityEngine;
using UnityEngine.UI;

public class Title : MonoBehaviour
{
    [SerializeField]
    private Text versionText;

    private void Awake()
    {
        Application.targetFrameRate = 60;

        versionText.text = $"v{Application.version}";
    }

    private void Start() => SoundManager.Instance.PlayBGM("Quiet Time", true);

    public void GameStart() => Loading.LoadScene(Scenes.Game);

    public void OnNewLandCafeIconClick() => Application.OpenURL("https://cafe.naver.com/2019newland");
}
