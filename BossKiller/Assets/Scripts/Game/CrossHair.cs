using UnityEngine;
using UnityEngine.UI;

public class CrossHair : MonoBehaviour
{
    [SerializeField]
    private Image crossHairImage;

    // 타입에 따른 요소
    [SerializeField]
    private CrossHairElement[] elements;

    public enum Type
    {
        Basic,
        ZoomIn
    }

    // 타입을 바꾸면서 크로스 헤어를 바꿉니다.
    public void SetType(Type type)
    {
        crossHairImage.sprite = elements[(int)type].sprite;
        crossHairImage.rectTransform.sizeDelta = elements[(int)type].size;
    }

    [System.Serializable]
    private class CrossHairElement
    {
        public Sprite sprite;
        public Vector2 size;
    }
}
