using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("Sound Effects")]
    [Tooltip("The audio source component attached to this object.")]
    [SerializeField] private AudioSource audioSource;

    public void LoadScene(string sceneName) {
        // Resume time before leaving a paused victory screen.
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }

    public void QuitGame() {
        Application.Quit();
    }
}
