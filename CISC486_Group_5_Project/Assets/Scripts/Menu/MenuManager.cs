using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("Sound Effects")]
    [Tooltip("The audio source component attached to this object.")]
    [SerializeField] private AudioSource audioSource;

    /** The sound clip to play by the pressed button */
    private AudioClip sound;

    /** The name of the scene to switch to, recieved from the button */
    private string scene;

    /** True if a button has provided a sound */
    private bool soundObtained = false;
    /** True if a button has provided a destination scene */
    private bool loadingScene = false;
    /** True if a button has asked to quit the game */
    private bool quittingGame = false;
    /** 
     * True only before any button is clicked for the first time. 
     * Prevents button spam while playing sound/loading/quitting.
     */
    private bool firstClick = true;

    public void Update()
    {
        // Guard against spamming buttons while the sound plays
        if (firstClick)
        {
            if (soundObtained && loadingScene)
            {
                // Resume time before leaving a paused victory screen.
                Time.timeScale = 1f;
                StartCoroutine("PlaySoundAndLoad");
            }
            else if (soundObtained && quittingGame)
            {
                StartCoroutine("PlaySoundAndQuit");
            }
        }
    }

    public void LoadScene(string sceneName) {
        loadingScene = true;
        scene = sceneName;
    }

    public void QuitGame() {
        quittingGame = true;
        Application.Quit();
    }

    public void PlaySoundEffect(AudioClip clip)
    {
        soundObtained = true;
        sound = clip;
    }

    public IEnumerator PlaySoundAndLoad()
    {
        // Disable functionality while playing the sound and loading
        firstClick = false;

        // Resume time before leaving a paused victory screen.
        Time.timeScale = 1f;
    
        // Wait for the sound effect to play, then change scenes
        audioSource.PlayOneShot(sound);
        yield return new WaitForSeconds(sound.length);
        
        resetMenu();
        SceneManager.LoadScene(scene);
    }

    public IEnumerator PlaySoundAndQuit()
    {
        // Disable functionality while playing the sound and loading
        firstClick = false;

        // Wait for the sound effect to play, then quit
        audioSource.PlayOneShot(sound);
        yield return new WaitForSeconds(sound.length);

        resetMenu();
        Application.Quit();
    }

    private void resetMenu()
    {
        // Reset all flags to allow the scene to start fresh
        soundObtained = false;
        loadingScene = false;
        quittingGame = false;
        firstClick = true;
    }
}
