using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class ImageLoad : MonoBehaviour
{
    [SerializeField]
    private string url;

    private RawImage image;

    private void Start()
    {
        image = GetComponent<RawImage>();

        StartCoroutine(TextureLoad());
    }

    private IEnumerator TextureLoad()
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            DontDestroyOnLoadUI.Instance.ShowPanel(ShowType.Error, "네트워크에 연결되지 않음", "현재 네트워크 상태를 확인해주세요.", false);
        }

        image.texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
    }
}
