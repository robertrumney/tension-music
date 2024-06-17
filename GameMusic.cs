using UnityEngine;
using System.Collections;

public class GameMusic : MonoBehaviour
{
    #region VARS AND REFS
    // Singleton instance of the GameMusic class
    public static GameMusic instance;

    // Audio sources for game music
    public AudioSource Music1;
    public AudioSource Music2;
    public AudioClip DeathMusic;

    // Flag to check if danger music is playing
    [System.NonSerialized]
    public bool ingozi = false;

    // Maximum volume for the music
    public float MaxVolume = 1;
    
    // Speed at which the music fades in and out
    public float musicFadeSpeed = 5;

    // Countdown timer for danger music
    private float countDown;

    // Flags to check the current state of the music
    private bool tension = false;
    private bool dead = false;
    #endregion

    #region INIT
    private void Awake()
    {
        // Set the instance to this object
        instance = this;

        // Set initial volume levels
        Music1.volume = 0;
        AudioListener.volume = 0;

        // Load the max volume from player preferences
        MaxVolume = PlayerPrefs.GetFloat("GameMusicVolume");
    }

    private void Start()
    {
        // Play the music if it is enabled
        if (Music1.enabled)
            Music1.Play();

        // Reset the volume to 0
        Music1.volume = 0;
    }
    #endregion

    #region METHODS

    // Start the tension music
    public void SenseTension()
    {
        StartCoroutine(Tension());
    }

    // Stop the tension music
    public void ZeroTension()
    {
        StartCoroutine(CalmTension());
    }

    private IEnumerator Tension()
    {
        // Gradually increase the volume of Music1 to the maximum volume
        while (Music1.volume < MaxVolume)
        {
            Music1.volume += Time.deltaTime;

            if (Music1.volume == 1.0 || Music1.volume > MaxVolume)
            {
                Music1.volume = MaxVolume;
                tension = true;
                yield break;
            }

            yield return null;
        }
    }

    private IEnumerator CalmTension()
    {
        // Gradually decrease the volume of Music1 to 0
        while (Music1.volume > 0)
        {
            Music1.volume -= Time.deltaTime;

            if (Music1.volume == 0.0)
            {
                tension = false;
                yield break;
            }

            yield return null;
        }
    }

    // Start the danger music
    public void Ingozi()
    {
        countDown = 12;

        if (!ingozi && !dead)
        {
            if (ShopKeeping.instance)
                ShopKeeping.instance.Scare();

            StartCoroutine(FadeToDanger());
            ingozi = true;
        }

        GameProgress.instance.introSwitchInterrupted = true;
    }

    // Start the death music
    public void Death()
    {
        if (!dead)
        {
            if (Music1.clip)
            {
                Music1.Stop();
                Music2.Stop();
            }

            Music1.volume = 0.33f;
            Music1.clip = DeathMusic;

            if (Music1.enabled == false)
                Music1.enabled = true;

            Music1.loop = false;
            Music1.Play();

            dead = true;
        }
    }

    // Set the maximum volume for the music
    private void SetMaxVolume(float x)
    {
        MaxVolume = x;

        if (ingozi)
        {
            Music2.volume = MaxVolume;
        }
        else
        {
            if (tension)
            {
                Music1.volume = MaxVolume;
            }
        }
    }
    #endregion

    #region COROUTINES
    private IEnumerator FadeToDanger()
    {
        // Gradually fade Music1 out and Music2 in
        while (true)
        {
            Music1.volume -= Time.deltaTime;

            if (Music2.volume < MaxVolume) 
            { 
                Music2.volume += Time.deltaTime; 
            }

            if (Music1.volume == 0)
            {
                StartCoroutine(CountDown());
                yield break;
            }

            yield return null;
        }
    }

    // Force the music to calm down
    public void ForceChill()
    {
        Invoke(nameof(DoForceChill), 1);
    }

    private void DoForceChill()
    {
        countDown = 0;
    }

    private IEnumerator CountDown()
    {
        // Countdown before fading to calm music
        while (true)
        {
            countDown--;

            if (countDown == 0)
            {
                if (!dead)
                {
                    StartCoroutine(FadeToChill());
                    yield break;
                }
            }

            yield return new WaitForSeconds(1);
        }
    }

    private IEnumerator FadeToChill()
    {
        // Gradually fade Music2 out and Music1 in
        while (true)
        {
            if (Music1.volume < MaxVolume)
            {
                Music1.volume += Time.deltaTime;
            }

            Music2.volume -= Time.deltaTime;

            if (Music2.volume == 0)
            {
                ingozi = false;
                yield break;
            }

            yield return null;
        }
    }
    #endregion
}
