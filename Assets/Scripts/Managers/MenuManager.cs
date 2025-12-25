using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [Header("Panel References")]
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject levelSelectionPanel;

    [Header("Menu Buttons")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button quitButton;

    [Header("Level Selection")]
    [SerializeField] private Transform levelButtonsContainer;
    [SerializeField] private GameObject levelButtonPrefab;
    [SerializeField] private GameConfig[] allLevels;

    private void Start()
    {
        startButton.onClick.AddListener(() => ShowLevelSelection());
        quitButton.onClick.AddListener(() => Application.Quit());

        CreateLevelButtons();
        ShowMenu();
    }

    private void CreateLevelButtons()
    {
        foreach (GameConfig level in allLevels)
        {
            GameObject btn = Instantiate(levelButtonPrefab, levelButtonsContainer);
            LevelButton levelBtn = btn.GetComponent<LevelButton>();
            levelBtn.Setup(level);
        }
    }

    private void ShowMenu()
    {
        menuPanel.SetActive(true);
        levelSelectionPanel.SetActive(false);
    }

    private void ShowLevelSelection()
    {
        menuPanel.SetActive(false);
        levelSelectionPanel.SetActive(true);
        RefreshLevelButtons();
    }

    private void RefreshLevelButtons()
    {
        foreach (LevelButton btn in levelButtonsContainer.GetComponentsInChildren<LevelButton>())
        {
            btn.Refresh();
        }
    }
}
