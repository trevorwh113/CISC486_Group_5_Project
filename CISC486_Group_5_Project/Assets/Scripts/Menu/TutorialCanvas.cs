using UnityEngine;
using UnityEngine.UI;

public class TutorialCanvas : MonoBehaviour
{
    [SerializeField] private Button startButton;

    private void Awake()
    {
        startButton.onClick.AddListener(StartGame);
        Time.timeScale = 0f;
    }

    private void StartGame()
    {
        Time.timeScale = 1f;
        gameObject.SetActive(false);
    }
}
