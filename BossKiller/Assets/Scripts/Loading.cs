using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum Scenes
{
    Title,
    Loading,
    Game
}

public class Loading : MonoBehaviour
{
    [SerializeField]
    private Image progressBarImage;

    private static Scenes nextScene;

    private void Start() => StartCoroutine(LoadSceneCoroutine());

    public static void LoadScene(Scenes scene)
    {
        nextScene = scene;

        SceneManager.LoadScene((int)Scenes.Loading);
    }

    private IEnumerator LoadSceneCoroutine()
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync((int)nextScene);

        asyncOperation.allowSceneActivation = false;

        float timer = 0f;

        while (!asyncOperation.isDone)
        {
            if (asyncOperation.progress < 0.9f)
            {
                progressBarImage.fillAmount = asyncOperation.progress;
            }
            else
            {
                while (timer <= 1f)
                {
                    progressBarImage.fillAmount = Mathf.Lerp(0.9f, 1f, timer);

                    timer += Time.unscaledDeltaTime;

                    yield return null;
                }

                asyncOperation.allowSceneActivation = true;

                yield break;
            }

            yield return null;
        }
    }
}
