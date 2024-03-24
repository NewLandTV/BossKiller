using UnityEngine;
using UnityEngine.UI;

public class CrossHair : MonoBehaviour
{
    [SerializeField]
    private Image crossHairImage;

    // Ÿ�Կ� ���� ���
    [SerializeField]
    private CrossHairElement[] elements;

    public enum Type
    {
        Basic,
        ZoomIn
    }

    // Ÿ���� �ٲٸ鼭 ũ�ν� �� �ٲߴϴ�.
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
