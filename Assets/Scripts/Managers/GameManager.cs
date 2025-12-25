using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private GameConfig gameConfig;
    
    [Header("Managers")]
    [SerializeField] private GridManager gridManager;
    [SerializeField] private UIManager uiManager;

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
    private int highScore = 0;

    private void Start()
    {
        StartGame();
    }
    
    private void Update()
    {
        if (isGameActive && gameConfig.useTimer)
        {
            timeRemaining -= Time.deltaTime;
            uiManager.UpdateTimer(timeRemaining);
            
            if (timeRemaining <= 0)
            {
                GameOver(false);
            }
        }
    }

    public void StartGame()
    {
        ResetGame();
        gridManager.GenerateGrid(gameConfig, OnCardClicked);
        totalPairs = (gameConfig.gridColumns * gameConfig.gridRows) / 2;
        timeRemaining = gameConfig.gameDuration;

        uiManager.UpdateMoves(movesCount);
        uiManager.UpdateTimer(timeRemaining);

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
        gridManager.SetCardsInteractable(false);

        yield return new WaitForSeconds(0.5f);

        gridManager.RevealAllCards();

        yield return new WaitForSeconds(gameConfig.previewDuration);

        gridManager.HideAllCards();

        yield return new WaitForSeconds(0.5f);

        isGameActive = true;
        gridManager.SetCardsInteractable(true);
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
            uiManager.UpdateMoves(movesCount);
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
        uiManager.UpdateScore(currentScore);
    }

    private void SubtractScore(int points)
    {
        currentScore = Mathf.Max(0, currentScore - points);
        uiManager.UpdateScore(currentScore);
    }

    private void CalculateFinalScore()
    {
        int timeBonus = gameConfig.useTimer ? Mathf.FloorToInt(timeRemaining * 10) : 0;
        int movesPenalty = movesCount * 5;

        currentScore = currentScore + timeBonus - movesPenalty;
        currentScore = Mathf.Max(0, currentScore);

        if (currentScore > highScore)
        {
            highScore = currentScore;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
        }
    }
    private void GameOver(bool won)
    {
        isGameActive = false;

        if (won)
        {
            CalculateFinalScore();
            uiManager.ShowWinScreen(movesCount, gameConfig.gameDuration - timeRemaining, currentScore);
            OnGameWon?.Invoke();
        }
        else
        {
            uiManager.ShowLoseScreen();
            OnGameLost?.Invoke();
        }
    }


    public void RestartGame()
    {
        gridManager.ClearGrid();
        StartGame();
        uiManager.HideGameOverScreen();
    }
}
