
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private bool isRestarting = false;

    void OnEnable()
    {
        GameEvents.guardReachedPlayer.AddListener(RestartLevel);
    }

    void OnDisable()
    {
        GameEvents.guardReachedPlayer.RemoveListener(RestartLevel);
    }

    private void RestartLevel()
    {
        if (isRestarting)
            return;

        isRestarting = true;

        Debug.Log("Player caught! Restarting level.");

        Time.timeScale = 1f;
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }
}
