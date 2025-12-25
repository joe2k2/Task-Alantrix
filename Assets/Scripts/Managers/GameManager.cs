using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GameManager : MonoBehaviour
{
    [Header("Level Configurations")]
    [SerializeField] private GameConfig[] allLevelConfigs;
    private GameConfig gameConfig;

    private Queue<CardController> processingQueue = new Queue<CardController>();

    private int movesCount;
    private int matchesFound;
    private int totalPairs;
    private float timeRemaining;
    private bool isGameActive;

    public event Action OnGameWon;
    public event Action OnGameLost;
    private int currentScore = 0;
    private void OnEnable()
    {
        EventManager.OnGameRestart += RestartGame;
    }
    private void OnDisable()
    {
        EventManager.OnGameRestart -= RestartGame;
    }
    private void Start()
    {
        int selectedLevel = ProgressManager.Instance.GetSelectedLevel();
        LoadLevelConfig(selectedLevel);
        StartGame();
    }
    private void LoadLevelConfig(int levelNumber)
    {
        if (allLevelConfigs != null && allLevelConfigs.Length > 0)
        {
            foreach (GameConfig config in allLevelConfigs)
            {
                if (config.levelNumber == levelNumber)
                {
                    gameConfig = config;
                    return;
                }
            }
            gameConfig = allLevelConfigs[0];
        }
    }
    private void Update()
    {
        if (isGameActive && gameConfig.useTimer)
        {
            timeRemaining -= Time.deltaTime;
            EventManager.UpdateTimer?.Invoke(timeRemaining);

            if (timeRemaining <= 0)
            {
                GameOver(false);
            }
        }
    }
    public void StartGame()
    {
        ResetGame();
        EventManager.OnStartGame?.Invoke(gameConfig, OnCardClicked);
        totalPairs = (gameConfig.gridColumns * gameConfig.gridRows) / 2;
        timeRemaining = gameConfig.gameDuration;
        EventManager.UpdateMoves(movesCount);
        EventManager.UpdateTimer?.Invoke(timeRemaining);
        if (gameConfig.showPreview)
        {
            StartCoroutine(ShowPreviewSequence());
        }
        else
        {
            isGameActive = true;
        }
        StartCoroutine(ProcessCardQueue());
    }

    private IEnumerator ShowPreviewSequence()
    {
        isGameActive = false;
        yield return new WaitForSeconds(0.5f);
        EventManager.RevealAllCards?.Invoke();
        yield return new WaitForSeconds(gameConfig.previewDuration);
        EventManager.HideAllCards?.Invoke();
        yield return new WaitForSeconds(0.5f);
        isGameActive = true;
    }
    private void ResetGame()
    {
        processingQueue.Clear();
        movesCount = 0;
        matchesFound = 0;
        currentScore = 0;
    }
    private void OnCardClicked(CardController card)
    {
        if (!isGameActive)
            return;
        if (card.IsRevealed || card.IsMatched)
            return;
        if (processingQueue.Contains(card))
            return;
        processingQueue.Enqueue(card);
        card.Reveal();
    }
    private IEnumerator ProcessCardQueue()
    {
        while (true)
        {
            if (processingQueue.Count >= 2)
            {
                CardController firstCard = processingQueue.Dequeue();
                CardController secondCard = processingQueue.Dequeue();
                movesCount++;
                EventManager.UpdateMoves(movesCount);
                StartCoroutine(CheckPair(firstCard, secondCard));
            }
            yield return null;
        }
    }
    private IEnumerator CheckPair(CardController firstCard, CardController secondCard)
    {
        yield return new WaitForSeconds(0.5f);
        if (firstCard.CardID == secondCard.CardID)
        {
            yield return StartCoroutine(HandleMatch(firstCard, secondCard));
        }
        else
        {
            yield return StartCoroutine(HandleMismatch(firstCard, secondCard));
        }
    }
    private IEnumerator HandleMatch(CardController first, CardController second)
    {
        first.SetMatched();
        second.SetMatched();
        EventManager.PlayMatch?.Invoke();
        StartCoroutine(first.PlayMatchAnimation());
        yield return StartCoroutine(second.PlayMatchAnimation());
        matchesFound++;
        AddScore(100);
        if (matchesFound >= totalPairs)
        {
            GameOver(true);
        }
    }
    private IEnumerator HandleMismatch(CardController first, CardController second)
    {
        EventManager.PlayMismatch?.Invoke();

        yield return new WaitForSeconds(gameConfig.mismatchDelay);
        first.Hide();
        second.Hide();
        SubtractScore(10);
    }
    private void AddScore(int points)
    {
        currentScore += points;
        EventManager.UpdateScore(currentScore);
    }
    private void SubtractScore(int points)
    {
        currentScore = Mathf.Max(0, currentScore - points);
        EventManager.UpdateScore(currentScore);
    }
    private void GameOver(bool won)
    {
        isGameActive = false;
        StopAllCoroutines();
        if (won)
        {
            EventManager.PlayWon?.Invoke();
            int levelNum = gameConfig.levelNumber;
            ProgressManager.Instance.CompleteLevel(levelNum);
            EventManager.ShowWinScreen(movesCount, gameConfig.gameDuration - timeRemaining, currentScore);
        }
        else
        {
            EventManager.PlayGameOver?.Invoke();
            EventManager.ShowLoseScreen();
        }
    }
    public void RestartGame()
    {
        EventManager.ClearGrid?.Invoke();
        StartGame();
        EventManager.HideLoseScreen();
        EventManager.HideWinScreen();
    }
}