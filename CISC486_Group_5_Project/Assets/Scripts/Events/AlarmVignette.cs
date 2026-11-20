using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class AlarmVignette : MonoBehaviour
{
    [SerializeField] private float flashDuration = 0.45f;
    [SerializeField, Range(0f, 1f)] private float maximumAlpha = 0.8f;

    private Image vignetteImage;
    private Coroutine flashRoutine;
    private Texture2D vignetteTexture;
    private Sprite vignetteSprite;

    private void Awake()
    {
        vignetteImage = GetComponent<Image>();
        vignetteImage.raycastTarget = false;
        vignetteImage.color = new Color(1f, 0f, 0f, 0f);
        CreateVignetteSprite();
    }

    private void OnEnable()
    {
        GameEvents.allServersScored.AddListener(Flash);
    }

    private void OnDisable()
    {
        GameEvents.allServersScored.RemoveListener(Flash);
    }

    private void Flash()
    {
        if (flashRoutine != null)
            StopCoroutine(flashRoutine);

        flashRoutine = StartCoroutine(FlashVignette());
    }

    private IEnumerator FlashVignette()
    {
        float elapsed = 0f;

        while (true)
        {
            elapsed = Mathf.Repeat(elapsed + Time.unscaledDeltaTime, flashDuration);
            float pulse = Mathf.Sin((elapsed / flashDuration) * Mathf.PI);
            vignetteImage.color = new Color(1f, 0f, 0f, maximumAlpha * pulse);
            yield return null;
        }
    }

    private void CreateVignetteSprite()
    {
        const int textureSize = 256;
        vignetteTexture = new Texture2D(textureSize, textureSize, TextureFormat.RGBA32, false);
        vignetteTexture.wrapMode = TextureWrapMode.Clamp;

        for (int y = 0; y < textureSize; y++)
        {
            for (int x = 0; x < textureSize; x++)
            {
                float normalizedX = Mathf.Abs((x / (textureSize - 1f)) * 2f - 1f);
                float normalizedY = Mathf.Abs((y / (textureSize - 1f)) * 2f - 1f);
                float edgeDistance = Mathf.Max(normalizedX, normalizedY);
                float alpha = Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(0.675f, 1f, edgeDistance));
                vignetteTexture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
            }
        }

        vignetteTexture.Apply();
        vignetteSprite = Sprite.Create(
            vignetteTexture,
            new Rect(0f, 0f, textureSize, textureSize),
            new Vector2(0.5f, 0.5f));
        vignetteImage.sprite = vignetteSprite;
    }

    private void OnDestroy()
    {
        Destroy(vignetteSprite);
        Destroy(vignetteTexture);
    }
}
