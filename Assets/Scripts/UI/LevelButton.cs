using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelButton : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private GameObject lockedIcon;
    [SerializeField] private GameObject completedIcon;

    private GameConfig levelConfig;

    public void Setup(GameConfig config)
    {
        levelConfig = config;
        button.onClick.AddListener(OnClick);
        Refresh();
    }

    public void Refresh()
    {
        if (levelConfig == null) return;

        int levelNum = levelConfig.levelNumber;
        bool isUnlocked = ProgressManager.Instance.IsLevelUnlocked(levelNum);
        bool isCompleted = ProgressManager.Instance.IsLevelCompleted(levelNum);

        levelText.text = $"Level {levelNum.ToString()}";
        button.interactable = isUnlocked;

        lockedIcon.SetActive(!isUnlocked);
        completedIcon.SetActive(isCompleted);

        GetComponent<Image>().color = isUnlocked ? (isCompleted ? Color.green : Color.white) : Color.gray;
    }

    private void OnClick()
    {
        ProgressManager.Instance.SetSelectedLevel(levelConfig.levelNumber);
        SceneManager.LoadScene("GameScene");
    }
}
