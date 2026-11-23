using UnityEngine;
using UnityEngine.UI;
public class MasterVolume : MonoBehaviour
{
    [SerializeField] Slider slider;
    void Start()
    {
        float v = PlayerPrefs.GetFloat("MasterVolume", 1f);
        slider.value = v;
        AudioListener.volume = v;
        slider.onValueChanged.AddListener(SetVolume);
    }
    void SetVolume(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat("MasterVolume", value);
    }
}
