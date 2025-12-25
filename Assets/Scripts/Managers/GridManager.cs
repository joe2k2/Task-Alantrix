using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CardController cardPrefab;
    [SerializeField] private Transform gridParent;
    [SerializeField] private GridLayoutGroup gridLayoutGroup;
    
    private List<CardController> spawnedCards = new List<CardController>();

    private void OnEnable()
    {
        EventManager.OnStartGame += GenerateGrid;
        EventManager.SetCardsInteractable += SetCardsInteractable;
        EventManager.RevealAllCards += RevealAllCards;
        EventManager.HideAllCards += HideAllCards;
        EventManager.ClearGrid += ClearGrid;
    }
    private void OnDisable()
    {
        EventManager.OnStartGame -= GenerateGrid;
        EventManager.SetCardsInteractable -= SetCardsInteractable;
        EventManager.RevealAllCards -= RevealAllCards;
        EventManager.HideAllCards -= HideAllCards;
        EventManager.ClearGrid -= ClearGrid;
    }
    public void GenerateGrid(GameConfig config, Action<CardController> onCardClicked)
    {
        ClearGrid();
        
        int totalCards = config.gridColumns * config.gridRows;
        int pairCount = totalCards / 2;
        
        List<CardData> cardPool = CreateCardPool(config.availableCards, pairCount);
        ShuffleCards(cardPool);
        
        ConfigureGridLayout(config);
        
        for (int i = 0; i < totalCards; i++)
        {
            CardController card = Instantiate(cardPrefab, gridParent);
            card.Initialize(cardPool[i], config.flipDuration);
            card.OnCardClicked += onCardClicked;
            spawnedCards.Add(card);
        }
    }
    public void RevealAllCards()
    {
        foreach (var card in spawnedCards)
        {
            card.Reveal();
        }
    }

    public void HideAllCards()
    {
        foreach (var card in spawnedCards)
        {
            card.Hide();
        }
    }

    private List<CardData> CreateCardPool(CardData[] availableCards, int pairCount)
    {
        List<CardData> cardPool = new List<CardData>();
        
        for (int i = 0; i < pairCount; i++)
        {
            CardData selectedCard = availableCards[i % availableCards.Length];
            cardPool.Add(selectedCard);
            cardPool.Add(selectedCard);
        }
        
        return cardPool;
    }
    
    private void ShuffleCards(List<CardData> cards)
    {
        for (int i = cards.Count - 1; i > 0; i--)
        {
            int randomIndex = UnityEngine.Random.Range(0, i + 1);
            CardData temp = cards[i];
            cards[i] = cards[randomIndex];
            cards[randomIndex] = temp;
        }
    }

    private void ConfigureGridLayout(GameConfig config)
    {
            gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayoutGroup.constraintCount = config.gridColumns;

            gridLayoutGroup.spacing = new Vector2(config.gridSpacing, config.gridSpacing);
            gridLayoutGroup.padding = new RectOffset(
                (int)config.gridPadding,
                (int)config.gridPadding,
                (int)config.gridPadding,
                (int)config.gridPadding
            );

            CalculateAndSetCellSize(config);
    }


    private void CalculateAndSetCellSize(GameConfig config)
    {
        RectTransform containerRect = gridParent.GetComponent<RectTransform>();

        float containerWidth = containerRect.rect.width;
        float containerHeight = containerRect.rect.height;

        float spacing = gridLayoutGroup.spacing.x;
        float paddingHorizontal = gridLayoutGroup.padding.left + gridLayoutGroup.padding.right;
        float paddingVertical = gridLayoutGroup.padding.top + gridLayoutGroup.padding.bottom;

        float availableWidth = containerWidth - paddingHorizontal - (spacing * (config.gridColumns - 1));
        float availableHeight = containerHeight - paddingVertical - (spacing * (config.gridRows - 1));
        float cellWidth = availableWidth / config.gridColumns;
        float cellHeight = availableHeight / config.gridRows;

        float cellSize = Mathf.Min(cellWidth, cellHeight);
        gridLayoutGroup.cellSize = new Vector2(cellSize, cellSize);
    }


    public void SetCardsInteractable(bool interactable)
    {
        foreach (var card in spawnedCards)
        {
            card.SetInteractable(interactable);
        }
    }
    
    public void ClearGrid()
    {
        foreach (var card in spawnedCards)
        {
            if (card != null)
            {
                Destroy(card.gameObject);
            }
        }
        spawnedCards.Clear();
    }
    public List<int> GetMatchedCardIndices()
    {
        List<int> matchedIndices = new List<int>();

        for (int i = 0; i < spawnedCards.Count; i++)
        {
            if (spawnedCards[i].IsMatched)
            {
                matchedIndices.Add(i);
            }
        }
        return matchedIndices;
    }

    public void RestoreMatchedCards(List<int> matchedIndices)
    {
        foreach (int index in matchedIndices)
        {
            if (index < spawnedCards.Count)
            {
                spawnedCards[index].Reveal();
                spawnedCards[index].SetMatched();
            }
        }
    }

}
