using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Controls music behaviour during different phases of gameplay.
 * Uses a public, static member variable to grant access from any script.
 */
public class MusicManager : MonoBehaviour
{
    [Header("Default Music")]
    [Tooltip("The audio component corresponding to the default music.")]
    [SerializeField] private AudioSource defaultSource;

    [Header("Chase Music")]
    [Tooltip("The audio component corresponding to the chase music.")]
    [SerializeField] private AudioSource chaseSource;

    [Header("Lockdown Music")]
    [Tooltip("The audio component corresponding to the lockdown music.")]
    [SerializeField] private AudioSource lockdownSource;

    [Header("Victory Music")]
    [Tooltip("The audio component corresponding to the win screen music.")]
    [SerializeField] private AudioSource victorySource;
    [Tooltip("The audio sting that plays immediately when winning.")]
    [SerializeField] private AudioClip victorySting;

    [Header("Transitions")]
    [Tooltip("Fade in an out durations in seconds.")]
    [SerializeField] private float fadeTime;

    /** 
      * Indicates whether lockdown mode has been triggered to
      * avoid playing chase music during lockdown.
      */
    bool isLockdown = false;

    /**
     * Indicates how many guards are chasing the player, to avoid
     * playing the chase sound multiple times if multiple guards
     * are alerted and to avoid it stopping prematurely if one guard
     * looses interest.
     */
    int numChasing = 0;

    /**
     * Caches the max valumes for each audio source as set in the editor,
     * to remember them while fading music in and out.
     */
    private Dictionary<String, float> maxVolumes = new Dictionary<String, float>();

    /** The public static instance of this script for global access  */
    public static MusicManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;

        // Play the default music by default
        defaultSource.Play();

        // Save the original volumes
        maxVolumes.Add("default", defaultSource.volume);
        maxVolumes.Add("chase", chaseSource.volume);
        maxVolumes.Add("lockdown", lockdownSource.volume);
        maxVolumes.Add("victory", victorySource.volume);
    }

    /**
     * When chase is started, play the chase music,
     * except during lockdown which using a different sound.
     */ 
    public void playChaseMusic()
    {
        numChasing++;
        if (numChasing == 1 && !isLockdown)
        {
            StopAllCoroutines();
            StartCoroutine(fadeMusicOut(defaultSource, "default", true));
            StartCoroutine(fadeMusicIn(chaseSource, "chase"));
        }
    }

    /**
     * When chase is stopped, stop the chase audio.
     */
    public void stopChaseMusic()
    {
        numChasing--;
        if (numChasing <= 0)
        {
            numChasing = 0;
            StopAllCoroutines();
            StartCoroutine(fadeMusicOut(chaseSource, "chase"));
            StartCoroutine(fadeMusicIn(defaultSource, "default", true));
        }
    }

    /**
     * Disable all other music playing and start the lockdown
     * music during the lockdown sequence, except the ambient sound.
     */
    public void playLockdownMusic()
    {
        if (!isLockdown)
        {
            isLockdown = true;
            stopChaseMusic();
            StartCoroutine(fadeMusicIn(lockdownSource, "lockdown"));
        }
    }

    /**
     * Victory always comes after a lockdown, so stop just that
     * on and start the victory music.
     */
    public void playVictoryMusic()
    {
        isLockdown = false;
        StopAllCoroutines();
        defaultSource.Stop();
        lockdownSource.Stop();

        victorySource.PlayOneShot(victorySting);
        victorySource.Play();
    }

    /**
     * Fade music in for the given AudioSource over fadeTime, starting from 0 to up to
     * the max volume. Unless unpause is specified, plays the audio from the beginning.
     */
    private IEnumerator fadeMusicIn(AudioSource source, String key, bool unpause=false)
    {
        float maxVol = maxVolumes[key];

        // Set the current volume to 0
        source.volume = 0;

        // Resume or start the music
        if (unpause)
        {
            source.UnPause();
        }
        else
        {
            source.Play();
        }

        // Incrementally increase its volume until the max is reached
        while (source.volume < maxVol)
        {
            source.volume += maxVol * Time.deltaTime / fadeTime;
            yield return null;
        }

        source.volume = maxVol;
    }

    /**
     * Fade music out for the given AudioSource over fadeTime, starting from the max
     * volume down to 0. Unless paused is specified, fully stops the audio.
     */
    private IEnumerator fadeMusicOut(AudioSource source, String key, bool pause=false)
    {
        float maxVol = maxVolumes[key];

        // Incrementally reduce the current volume to 0
        while (source.volume > 0)
        {
            source.volume -= maxVol * Time.deltaTime / fadeTime;
            yield return null;
            Debug.Log("Fading out music: " + key);
        }

        Debug.Log("Finished fading out music: " + key);

        // Pause or stop the music
        if (pause)
        {
            source.Pause();
        }
        else
        {
            source.Stop();
        }

        source.volume = maxVol;
    }
}
