using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CardController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image cardBack;
    [SerializeField] private Image cardFront;
    [SerializeField] private Button button;
    
    [Header("Animation Settings")]
    [SerializeField] private float flipDuration = 0.3f;
    
    public int CardID { get; private set; }
    public bool IsRevealed { get; private set; }
    public bool IsMatched { get; private set; }
    
    private RectTransform rectTransform;
    private Coroutine flipCoroutine;
    
    public event Action<CardController> OnCardClicked;
    
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        button.onClick.AddListener(OnClick);
    }
    
    public void Initialize(CardData cardData, float animDuration)
    {
        CardID = cardData.cardID;
        flipDuration = animDuration;
        
        if (cardData.cardSprite != null)
        {
            cardFront.sprite = cardData.cardSprite;
        }

        SetCardState(false);
    }
    
    private void OnClick()
    {
        if (!IsRevealed && !IsMatched)
        {
            OnCardClicked?.Invoke(this);
        }
    }
    
    public void Reveal()
    {
        if (flipCoroutine != null)
            StopCoroutine(flipCoroutine);
        
        flipCoroutine = StartCoroutine(FlipCard(true));
    }
    
    public void Hide()
    {
        if (flipCoroutine != null)
            StopCoroutine(flipCoroutine);
        
        flipCoroutine = StartCoroutine(FlipCard(false));
    }
    
    public void SetMatched()
    {
        IsMatched = true;
        button.interactable = false;
    }
    
    public void SetInteractable(bool interactable)
    {
        button.interactable = interactable && !IsMatched;
    }
    
    private IEnumerator FlipCard(bool showFront)
    {
        float elapsed = 0f;
        float halfDuration = flipDuration / 2f;
        Vector3 startRotation = rectTransform.localEulerAngles;
        Vector3 midRotation = new Vector3(0, 90, 0);
        bool hasSwapped = false;

        EventManager.playCardFlip?.Invoke();
        while (elapsed < flipDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / flipDuration;
            
            if (progress < 0.5f)
            {
                float t = progress * 2f;
                rectTransform.localEulerAngles = Vector3.Lerp(startRotation, midRotation, t);
            }
            else
            {
                if (!hasSwapped)
                {
                    SetCardState(showFront);
                    hasSwapped = true;
                }
                
                float t = (progress - 0.5f) * 2f;
                rectTransform.localEulerAngles = Vector3.Lerp(midRotation, Vector3.zero, t);
            }
            
            yield return null;
        }
        
        rectTransform.localEulerAngles = Vector3.zero;
        SetCardState(showFront);
        IsRevealed = showFront;
    }
    
    private void SetCardState(bool showFront)
    {
        cardBack.gameObject.SetActive(!showFront);
        cardFront.transform.parent.gameObject.SetActive(showFront);
        IsRevealed = showFront;
    }
    
    public IEnumerator PlayMatchAnimation()
    {
        Vector3 originalScale = transform.localScale;
        Vector3 targetScale = originalScale * 1.2f;
        float duration = 0.2f;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            yield return null;
        }
        
        elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            transform.localScale = Vector3.Lerp(targetScale, originalScale, t);
            yield return null;
        }
        
        transform.localScale = originalScale;
    }
}
