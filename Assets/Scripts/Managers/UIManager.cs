using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Game UI")]
    [SerializeField] private TextMeshProUGUI movesText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI scoreText;

    [Header("Game Over UI")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI gameOverTitle;
    [SerializeField] private TextMeshProUGUI statsText;
    [SerializeField] private Button restartButton;
    
    private GameManager gameManager;
    
    private void Awake()
    {
        restartButton.onClick.AddListener(OnRestartClicked);
        
        HideGameOverScreen();
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
         gameOverPanel.SetActive(true);
         gameOverTitle.text = "YOU WIN!";

         int minutes = Mathf.FloorToInt(timeTaken / 60f);
         int seconds = Mathf.FloorToInt(timeTaken % 60f);

         statsText.text = $"Final Score: {finalScore}\nMoves: {moves}\nTime: {minutes:00}:{seconds:00}";
    }


    public void ShowLoseScreen()
    {
         gameOverPanel.SetActive(true);
         gameOverTitle.text = "TIME'S UP!";
         statsText.text = "Try again!";
    }
    
    public void HideGameOverScreen()
    {
         gameOverPanel.SetActive(false);
    }
    
    private void OnRestartClicked()
    {
        gameManager.RestartGame();
    }
}
