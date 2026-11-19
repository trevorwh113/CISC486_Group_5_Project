using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private bool isRestarting = false;
    private readonly StringBuilder code = new StringBuilder();
    private int codeLength;
    private bool finalDoorUnlocked;
    private DoorCodeUI codeUI;
    private ServerInteraction finalServer;

    public bool FinalDoorUnlocked => finalDoorUnlocked;
    public int CodeLength => codeLength;
    public bool AllCodeDigitsCollected => code.Length == codeLength;
    public bool IsFinalServer(ServerInteraction server) => finalServer != null && server == finalServer;

    private void Awake()
    {
        Instance = this;
        DoorKeypad keypad = FindAnyObjectByType<DoorKeypad>();
        finalServer = keypad != null ? keypad.FinalServer : null;
        codeLength = Mathf.Max(0, FindObjectsByType<ServerInteraction>().Length - 1);
    }

    private void Start()
    {
        codeUI = FindAnyObjectByType<DoorCodeUI>();
        if (codeUI != null)
            RefreshCodeUI();
        else
        {
            Debug.LogError("Add a DoorCodeUI component with a password text reference to GameUI");
        }
    }

    void OnEnable()
    {
        GameEvents.guardReachedPlayer.AddListener(RestartLevel);
    }

    void OnDisable()
    {
        GameEvents.guardReachedPlayer.RemoveListener(RestartLevel);
    }

    public void CollectCodeDigit()
    {
        if (code.Length >= codeLength)
            return;

        code.Append(Random.Range(0, 10));
        RefreshCodeUI();
    }

    public bool TryUnlockFinalDoor(string enteredCode)
    {
        if (finalDoorUnlocked || codeLength == 0 || !AllCodeDigitsCollected || enteredCode != code.ToString())
            return false;

        finalDoorUnlocked = true;
        return true;
    }

    private void RefreshCodeUI()
    {
        if (codeUI != null)
            codeUI.RefreshCode(code.ToString().PadRight(codeLength, '_'));
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
