using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Game UI")]
    [SerializeField] private TextMeshProUGUI movesText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI scoreText;

    [Header("Won UI")]
    [SerializeField] private GameObject wonPanel;
    [SerializeField] private TextMeshProUGUI wonStatsText;
    [SerializeField] private Button restartWonButton;

    [Header("Game Over UI")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI gameOverStatsText;
    [SerializeField] private Button restarGameOvertButton;
    
    private void Awake()
    {
        restartWonButton.onClick.AddListener(OnRestartClicked);
        restarGameOvertButton.onClick.AddListener(OnRestartClicked);
        
        HideGameOverScreen();
        HideWonScreen();
    }
    private void OnEnable()
    {
        EventManager.UpdateTimer += UpdateTimer;
        EventManager.UpdateMoves += UpdateMoves;
        EventManager.UpdateScore += UpdateScore;
        EventManager.ShowWinScreen += ShowWinScreen;
        EventManager.ShowLoseScreen += ShowLoseScreen;
        EventManager.HideWinScreen += HideWonScreen;
        EventManager.HideLoseScreen += HideGameOverScreen;
    }
    private void OnDisable()
    {
        EventManager.UpdateTimer -= UpdateTimer;
        EventManager.UpdateMoves -= UpdateMoves;
        EventManager.UpdateScore -= UpdateScore;
        EventManager.ShowWinScreen -= ShowWinScreen;
        EventManager.ShowLoseScreen -= ShowLoseScreen;
        EventManager.HideWinScreen -= HideWonScreen;
        EventManager.HideLoseScreen -= HideGameOverScreen;
    }
    public void UpdateMoves(int moves)
    {
        movesText.text = $"Moves: {moves}";
    }
    
    public void UpdateTimer(float timeRemaining)
    {
         int minutes = Mathf.FloorToInt(timeRemaining / 60f);
         int seconds = Mathf.FloorToInt(timeRemaining % 60f);
         timerText.text = $"Time: {minutes:00}:{seconds:00}";
    }
    public void UpdateScore(int score)
    {
         scoreText.text = $"Score: {score}";
    }

    public void ShowWinScreen(int moves, float timeTaken, int finalScore)
    {
         wonPanel.SetActive(true);

         int minutes = Mathf.FloorToInt(timeTaken / 60f);
         int seconds = Mathf.FloorToInt(timeTaken % 60f);

         wonStatsText.text = $"Final Score: {finalScore}\nMoves: {moves}\nTime: {minutes:00}:{seconds:00}";
    }


    public void ShowLoseScreen()
    {
         gameOverPanel.SetActive(true);
         gameOverStatsText.text = "Try again!";
    }
    
    public void HideGameOverScreen()
    {
         gameOverPanel.SetActive(false);
    }
    public void HideWonScreen()
    {
        wonPanel.SetActive(false);
    }

    private void OnRestartClicked()
    {
        EventManager.OnGameRestart?.Invoke();
    }
}
