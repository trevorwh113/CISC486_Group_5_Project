using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/** Keeps the code beside the score and draws the door keypad on an overlay. */
public class DoorCodeUI : MonoBehaviour
{
    [Tooltip("Password text saved under GameUI so its position can be edited in the prefab.")]
    [SerializeField] private TMP_Text codeText;
    private GameObject uiRoot;
    private GameObject keypadPanel;
    private TMP_Text enteredText;
    private TMP_Text messageText;
    private TMP_Text doorPrompt;

    private void OnEnable()
    {
        GameEvents.playerWon.AddListener(HidePassword);
    }

    private void OnDisable()
    {
        GameEvents.playerWon.RemoveListener(HidePassword);
    }

    private void HidePassword()
    {
        if (codeText != null)
            codeText.enabled = false;
    }

    public void Initialize(Action<int> appendDigit, Action submit, Action close, Action clear)
    {
        if (codeText == null)
        {
            Debug.LogError("Assign the password text on the DoorCodeUI component.", this);
            return;
        }
        TMP_FontAsset font = codeText.font;
        //create the door code canvas and its children
        uiRoot = new GameObject("Door code canvas", typeof(RectTransform), typeof(Canvas),
            typeof(CanvasScaler), typeof(GraphicRaycaster));
        Canvas canvas = uiRoot.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10;
        CanvasScaler scaler = uiRoot.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        doorPrompt = Text(canvas.transform, "Door keypad prompt", font, 28);
        RectTransform prompt = doorPrompt.rectTransform;
        prompt.anchorMin = prompt.anchorMax = new Vector2(0.5f, 0);
        prompt.pivot = new Vector2(0.5f, 0);
        prompt.anchoredPosition = new Vector2(0, 80);
        prompt.sizeDelta = new Vector2(500, 50);
        doorPrompt.text = "Press E to use door keypad";

        RectTransform keypad = Panel(canvas.transform, "Door keypad UI", new Vector2(0.5f, 0.5f),
            Vector2.zero, new Vector2(320, 430), new Color(0.08f, 0.12f, 0.17f, 0.97f));
        keypadPanel = keypad.gameObject;
        TMP_Text title = Text(keypad, "Title", font, 28);
        Place(title.rectTransform, new Vector2(0, 175), new Vector2(290, 42));
        title.text = "FINAL DOOR";
        enteredText = Text(keypad, "Entered code", font, 32);
        Place(enteredText.rectTransform, new Vector2(0, 125), new Vector2(290, 42));
        messageText = Text(keypad, "Message", font, 20);
        Place(messageText.rectTransform, new Vector2(0, 85), new Vector2(290, 32));

        // number buttons
        Button(keypad, "1", font, new Vector2(-90, 30),
            new Vector2(78, 46), () => appendDigit(1));
        Button(keypad, "2", font, new Vector2(0, 30),
            new Vector2(78, 46), () => appendDigit(2));
        Button(keypad, "3", font, new Vector2(90, 30),
            new Vector2(78, 46), () => appendDigit(3));
        Button(keypad, "4", font, new Vector2(-90, -26),
            new Vector2(78, 46), () => appendDigit(4));
        Button(keypad, "5", font, new Vector2(0, -26),
            new Vector2(78, 46), () => appendDigit(5));
        Button(keypad, "6", font, new Vector2(90, -26),
            new Vector2(78, 46), () => appendDigit(6));
        Button(keypad, "7", font, new Vector2(-90, -82),
            new Vector2(78, 46), () => appendDigit(7));
        Button(keypad, "8", font, new Vector2(0, -82),
            new Vector2(78, 46), () => appendDigit(8));
        Button(keypad, "9", font, new Vector2(90, -82),
            new Vector2(78, 46), () => appendDigit(9));
        // bottom row buttons
        Button(keypad, "Clear", font, new Vector2(-90, -138), new Vector2(78, 46), clear);
        Button(keypad, "0", font, new Vector2(0, -138), new Vector2(78, 46), () => appendDigit(0));
        Button(keypad, "Enter", font, new Vector2(90, -138), new Vector2(78, 46), submit);
        Button(keypad, "Close", font, new Vector2(0, -188), new Vector2(110, 34), close);
        keypadPanel.SetActive(false);
    }

    private void OnDestroy()
    {
        if (uiRoot != null) Destroy(uiRoot);
    }

    public void RefreshCode(string discovered)
    {
        if (codeText == null) return;
        codeText.text = "DOOR CODE  " + discovered;
    }

    public void RefreshKeypad(string entered, string message, bool open, bool inRange)
    {
        if (uiRoot == null) return;
        enteredText.text = entered;
        messageText.text = message;
        doorPrompt.gameObject.SetActive(inRange && !open);
        keypadPanel.SetActive(open);
    }

    private static RectTransform Panel(Transform parent, string name, Vector2 anchor,
        Vector2 position, Vector2 size, Color color)
    {
        GameObject obj = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        obj.transform.SetParent(parent, false);
        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = anchor;
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        Image image = obj.GetComponent<Image>();
        image.color = color;
        image.raycastTarget = false;
        return rect;
    }

    private static TMP_Text Text(Transform parent, string name, TMP_FontAsset font, float size)
    {
        GameObject obj = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        obj.transform.SetParent(parent, false);
        TextMeshProUGUI text = obj.GetComponent<TextMeshProUGUI>();
        text.font = font;
        text.fontSize = size;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.white;
        text.raycastTarget = false;
        return text;
    }

    private static void Button(Transform parent, string label, TMP_FontAsset font,
        Vector2 position, Vector2 size, Action action)
    {
        RectTransform rect = Panel(parent, label, new Vector2(0.5f, 0.5f), position,
            size, new Color(0.2f, 0.34f, 0.43f, 1f));
        Image image = rect.GetComponent<Image>();
        image.raycastTarget = true;
        UnityEngine.UI.Button button = rect.gameObject.AddComponent<UnityEngine.UI.Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(() => action());
        TMP_Text caption = Text(rect, "Label", font, 22);
        Stretch(caption.rectTransform);
        caption.text = label;
    }

    private static void Place(RectTransform rect, Vector2 position, Vector2 size)
    {
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
    }

    private static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }
}
