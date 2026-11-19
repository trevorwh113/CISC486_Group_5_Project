using UnityEngine;
using UnityEngine.InputSystem;

/** Attach this to the door that blocks the final server's room. */
public class DoorKeypad : MonoBehaviour
{
    [Tooltip("The server inside the locked room. This server does not reveal a code digit.")]
    [SerializeField] private ServerInteraction finalServer;
    [Tooltip("Optional spot on the door where the player must stand to use the keypad.")]
    [SerializeField] private Transform interactionPoint;
    [Tooltip("How close the player must be to the keypad to press E.")]
    [SerializeField] private float interactionDistance = 2.5f;
    [Tooltip("Object to hide when unlocked. Leave empty to hide this door object.")]
    [SerializeField] private GameObject doorToOpen;

    private Transform player;
    private bool playerInRange;
    private bool keypadOpen;
    private string enteredCode = "";
    private string keypadMessage = "";
    private DoorCodeUI codeUI;
    private static readonly Key[] DigitKeys =
    {
        Key.Digit0, Key.Digit1, Key.Digit2, Key.Digit3, Key.Digit4,
        Key.Digit5, Key.Digit6, Key.Digit7, Key.Digit8, Key.Digit9
    };
    private static readonly Key[] NumpadKeys =
    {
        Key.Numpad0, Key.Numpad1, Key.Numpad2, Key.Numpad3, Key.Numpad4,
        Key.Numpad5, Key.Numpad6, Key.Numpad7, Key.Numpad8, Key.Numpad9
    };

    public ServerInteraction FinalServer => finalServer;

    private void Start()
    {
        if (finalServer == null)
            Debug.LogError("Assign the final server on the DoorKeypad component.", this);

        codeUI = FindAnyObjectByType<DoorCodeUI>();
        if (codeUI != null)
        {
            codeUI.Initialize(AppendDigit, SubmitCode, CloseKeypad, ClearEntry);
            RefreshKeypadUI();
        }
        else
            Debug.LogError("Add a DoorCodeUI component to GameUI for the door keypad.", this);
    }

    private void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.FinalDoorUnlocked)
            return;

        if (player == null)
        {
            GameObject playerObject = GameObject.FindWithTag("Player");
            if (playerObject != null) player = playerObject.transform;
        }

        Vector3 keypadPosition = interactionPoint != null ? interactionPoint.position : transform.position;
        bool inRange = player != null &&
            (player.position - keypadPosition).sqrMagnitude <= interactionDistance * interactionDistance;
        if (inRange != playerInRange)
        {
            playerInRange = inRange;
            if (!inRange) CloseKeypad();
            RefreshKeypadUI();
        }

        if (inRange && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
            OpenKeypad();

        if (!keypadOpen || Keyboard.current == null)
            return;

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            CloseKeypad();
            return;
        }

        if (Keyboard.current.backspaceKey.wasPressedThisFrame && enteredCode.Length > 0)
            enteredCode = enteredCode.Substring(0, enteredCode.Length - 1);

        if (Keyboard.current.enterKey.wasPressedThisFrame || Keyboard.current.numpadEnterKey.wasPressedThisFrame)
            SubmitCode();

        for (int digit = 0; digit <= 9; digit++)
        {
            if ((Keyboard.current[DigitKeys[digit]].wasPressedThisFrame ||
                 Keyboard.current[NumpadKeys[digit]].wasPressedThisFrame) &&
                enteredCode.Length < GameManager.Instance.CodeLength)
                enteredCode += digit;
        }
        RefreshKeypadUI();
    }

    private void OpenKeypad()
    {
        if (!playerInRange || GameManager.Instance.FinalDoorUnlocked)
            return;

        enteredCode = "";
        keypadMessage = "";
        keypadOpen = true;
        RefreshKeypadUI();
    }

    private void CloseKeypad()
    {
        keypadOpen = false;
        RefreshKeypadUI();
    }

    private void SubmitCode()
    {
        if (GameManager.Instance.TryUnlockFinalDoor(enteredCode))
        {
            CloseKeypad();
            OpenDoor();
            return;
        }

        keypadMessage = GameManager.Instance.AllCodeDigitsCollected ? "Incorrect code" : "Find the remaining servers";
        enteredCode = "";
        RefreshKeypadUI();
    }

    private void RefreshKeypadUI()
    {
        if (codeUI == null || GameManager.Instance == null)
            return;

        codeUI.RefreshKeypad(enteredCode.PadRight(GameManager.Instance.CodeLength, '_'),
            keypadMessage, keypadOpen, playerInRange && !GameManager.Instance.FinalDoorUnlocked);
    }

    private void AppendDigit(int digit)
    {
        if (GameManager.Instance != null && enteredCode.Length < GameManager.Instance.CodeLength)
            enteredCode += digit;
        RefreshKeypadUI();
    }

    private void ClearEntry()
    {
        enteredCode = "";
        RefreshKeypadUI();
    }

    private void OpenDoor()
    {
        GameObject target = doorToOpen != null ? doorToOpen : gameObject;
        target.SetActive(false);
    }

    private void OnDisable()
    {
        playerInRange = false;
        CloseKeypad();
    }

    private void OnValidate()
    {
        interactionDistance = Mathf.Max(0.1f, interactionDistance);
    }
}
