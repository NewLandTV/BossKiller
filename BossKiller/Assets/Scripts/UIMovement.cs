using UnityEngine;

public class UIMovement : MonoBehaviour
{
    [SerializeField]
    private float multiply;

    private RectTransform rectTransform;

    private void Awake() => rectTransform = GetComponent<RectTransform>();

    private void Update() => rectTransform.anchoredPosition += Vector2.up * Mathf.Cos(Time.time) * multiply;
}
