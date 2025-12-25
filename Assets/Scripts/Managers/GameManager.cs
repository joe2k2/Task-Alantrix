using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Level Configurations")]
    [SerializeField] private GameConfig[] allLevelConfigs;
    private GameConfig gameConfig;
    
    private List<CardController> selectedCards = new List<CardController>();
    private int maxSelectableCards = 2;
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
    }
    private IEnumerator ShowPreviewSequence()
    {
        isGameActive = false;
        //gridManager.SetCardsInteractable(false);

        yield return new WaitForSeconds(0.5f);

        EventManager.RevealAllCards?.Invoke();

        yield return new WaitForSeconds(gameConfig.previewDuration);

        EventManager.HideAllCards?.Invoke();

        yield return new WaitForSeconds(0.5f);

        isGameActive = true;
        //gridManager.SetCardsInteractable(true);
    }


    private void ResetGame()
    {
        selectedCards.Clear();
        movesCount = 0;
        matchesFound = 0;
        currentScore = 0;
    }


    private void OnCardClicked(CardController card)
    {
        if (!isGameActive)
            return;

        if (selectedCards.Count >= maxSelectableCards)
            return;

        if (selectedCards.Contains(card))
            return;

        selectedCards.Add(card);
        card.Reveal();

        if (selectedCards.Count == maxSelectableCards)
        {
            movesCount++;
            EventManager.UpdateMoves(movesCount);
            StartCoroutine(CheckMatch());
        }
    }


    private IEnumerator CheckMatch()
    {
        CardController firstCard = selectedCards[0];
        CardController secondCard = selectedCards[1];

        yield return new WaitForSeconds(0.5f);

        if (firstCard.CardID == secondCard.CardID)
        {
            yield return StartCoroutine(HandleMatch(firstCard, secondCard));
        }
        else
        {
            yield return StartCoroutine(HandleMismatch(firstCard, secondCard));
        }

        selectedCards.Clear();
    }


    private IEnumerator HandleMatch(CardController first, CardController second)
    {
        first.SetMatched();
        second.SetMatched();

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

        if (won)
        {
            int levelNum = gameConfig.levelNumber;
            ProgressManager.Instance.CompleteLevel(levelNum);

            EventManager.ShowWinScreen(movesCount, gameConfig.gameDuration - timeRemaining, currentScore);
        }
        else
        {
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
