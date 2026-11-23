using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("UI Canvas")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private string mainMenuScene = "MainMenu";

    [Header("Sound Effects")]
    [Tooltip("The audio source component attached to this object.")]
    [SerializeField] private AudioSource audioSource;

    [Tooltip("The audio clip to play when pausing or unpausing the menu.")]
    [SerializeField] private AudioClip sound;
    private bool isPaused;

    private void Start()
    {
        pausePanel.SetActive(false);
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Pause()
    {
        isPaused = true;
        pausePanel.SetActive(true);
        audioSource.PlayOneShot(sound);
        Time.timeScale = 0f;
    }

    public void Resume()
    {
        isPaused = false;
        pausePanel.SetActive(false);
        audioSource.PlayOneShot(sound);
        Time.timeScale = 1f;
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuScene);
    }
}
