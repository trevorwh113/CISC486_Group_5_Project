using TMPro;
using UnityEngine;

/**
 * Stores the player's score and keeps the score UI synchronized.
 */
public class ScoreManager : MonoBehaviour
{
    /** Shared scene instance used by interactable objects */
    public static ScoreManager Instance;

    [Tooltip("The UI text used to display the current score.")]
    [SerializeField] private TMP_Text scoreText;

    /** The player's current score */
    private int score;
    /** The number of active servers that must be scored in this level */
    private int totalServers;
    /** The number of servers that have already awarded their point */
    private int scoredServers;
    /** Prevents the completion event from being invoked more than once */
    private bool completionEventInvoked;

    private void Awake()
    {
        // Register this scene's manager and initialize its display.
        Instance = this;
        UpdateDisplay();
    }

    private void Start()
    {
        // Build the level objective from every active server in the loaded scene.
        totalServers = FindObjectsByType<ServerInteraction>().Length;
    }

    /**
     * Increases the score by the supplied amount.
     */
    public void AddScore(int amount)
    {
        score += amount;
        UpdateDisplay();
    }

    /**
     * Records one completed server and triggers lockdown after the final server.
     */
    public void RegisterServerScored()
    {
        scoredServers++;
        AddScore(1);

        if (!completionEventInvoked &&
            totalServers > 0 &&
            scoredServers >= totalServers)
        {
            completionEventInvoked = true;
            GameEvents.allServersScored.Invoke();
        }
    }

    /**
     * Refreshes the UI to match the stored score.
     */
    private void UpdateDisplay()
    {
        scoreText.text = "Score: " + score;
    }
}