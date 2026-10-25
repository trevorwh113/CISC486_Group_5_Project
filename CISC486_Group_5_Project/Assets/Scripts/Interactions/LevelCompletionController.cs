using UnityEngine;

/**
 * Activates the level exit after every server is scored and displays the
 * victory screen when the player reaches that exit.
 */
public class LevelCompletionController : MonoBehaviour
{
    [Tooltip("The exit object that becomes available after lockdown begins.")]
    [SerializeField] private GameObject exitObject;
    [Tooltip("The UI panel displayed after the player wins.")]
    [SerializeField] private GameObject victoryPanel;

    private void Awake()
    {
        // Reset the level state whenever this scene is loaded.
        Time.timeScale = 1f;
        exitObject.SetActive(false);
        victoryPanel.SetActive(false);
    }

    private void OnEnable()
    {
        GameEvents.allServersScored.AddListener(activateExit);
        GameEvents.playerWon.AddListener(showVictory);
    }

    private void OnDisable()
    {
        GameEvents.allServersScored.RemoveListener(activateExit);
        GameEvents.playerWon.RemoveListener(showVictory);
    }

    /**
     * Makes the escape destination available when lockdown begins.
     */
    private void activateExit()
    {
        exitObject.SetActive(true);
    }

    /**
     * Ends active gameplay and displays the victory overlay.
     */
    private void showVictory()
    {
        victoryPanel.SetActive(true);
        Time.timeScale = 0f;
    }
}
