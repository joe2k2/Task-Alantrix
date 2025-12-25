using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Clips")]
    [SerializeField] private AudioClip cardFlipSound;
    [SerializeField] private AudioClip matchSound;
    [SerializeField] private AudioClip mismatchSound;
    [SerializeField] private AudioClip gameOverSound;
    [SerializeField] private AudioClip wonSound;

    [Header("Audio Source")]
    [SerializeField] private AudioSource audioSource;

    [Header("Volume Settings")]
    [Range(0f, 1f)]
    [SerializeField] private float sfxVolume = 1f;

    private void Awake()
    {
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;
        audioSource.volume = sfxVolume;
    }
    private void OnEnable()
    {
        EventManager.playCardFlip += PlayCardFlip;
        EventManager.PlayMatch += PlayMatch;
        EventManager.PlayMismatch += PlayMismatch;
        EventManager.PlayGameOver += PlayGameOver;
        EventManager.PlayWon += PlayWon;
    }
    private void OnDisable()
    {
        EventManager.playCardFlip -= PlayCardFlip;
        EventManager.PlayMatch -= PlayMatch;
        EventManager.PlayMismatch -= PlayMismatch;
        EventManager.PlayGameOver -= PlayGameOver;
        EventManager.PlayWon -= PlayWon;
    }

    public void PlayCardFlip()
    {
        PlaySound(cardFlipSound);
    }

    public void PlayMatch()
    {
        PlaySound(matchSound);
    }

    public void PlayMismatch()
    {
        PlaySound(mismatchSound);
    }

    public void PlayGameOver()
    {
        PlaySound(gameOverSound);
    }
    public void PlayWon()
    {
        PlaySound(wonSound);
    }
    private void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip, sfxVolume);
        }
    }

    public void SetVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        audioSource.volume = sfxVolume;
    }
}
