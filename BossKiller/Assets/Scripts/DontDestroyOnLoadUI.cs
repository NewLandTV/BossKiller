using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum ShowType
{
    Basic,
    Warning,
    Error
}

public class DontDestroyOnLoadUI : Singleton<DontDestroyOnLoadUI>
{
    [Header("Show Panel Group")]
    [SerializeField]
    private GameObject showPanelGroup;
    [SerializeField]
    private Text showPanelTitletext;
    [SerializeField]
    private Text showPanelDescriptiontext;
    [SerializeField]
    private Button showPanelOkButton;
    [SerializeField]
    private Color[] showPanelTitleColors;

    private bool isOnPanel;

    private void Awake() => Setup(this);

    public void ShowPanel(ShowType showType, string title, string description, bool quit)
    {
        if (isOnPanel)
        {
            return;
        }

        isOnPanel = true;

        showPanelTitletext.text = title;
        showPanelDescriptiontext.text = description;

        SetShowErrorOkButtonClickEvent(() =>
        {
            isOnPanel = false;

            showPanelOkButton.onClick.RemoveAllListeners();

            if (quit)
            {
                Application.Quit();
            }
            else
            {
                showPanelGroup.SetActive(false);
            }
        });

        showPanelTitletext.color = showPanelTitleColors[(int)showType];

        showPanelGroup.SetActive(true);
    }

    public void SetShowErrorOkButtonClickEvent(UnityAction action)
    {
        showPanelOkButton.onClick.RemoveAllListeners();
        showPanelOkButton.onClick.AddListener(action);
    }
}
