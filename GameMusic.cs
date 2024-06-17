using UnityEngine;
using System.Collections;

public class GameMusic : MonoBehaviour
{
    public static GameMusic instance;

    public AudioSource Music1;
    public AudioSource Music2;
    public AudioClip DeathMusic;

    public float MaxVolume = 1;
    public float musicFadeSpeed = 5;

    private float countDown;

    private float tensionTarget = 0;

    private bool tension = false;
    private bool danger = false;
    private bool dead = false;

    private void Awake()
    {
        // Set the instance to this script
        instance = this;

        // Initialize Music1 volume
        Music1.volume = 0;

        // Set AudioListener volume to 0
        AudioListener.volume = 0;

        // Get the maximum volume setting from player preferences
        MaxVolume = PlayerPrefs.GetFloat("GameMusicVolume");
    }

    private void Start()
    {
        // Set the audio output mixer group for Music1 and Music2
        Music1.outputAudioMixerGroup = Game.instance.audioSource.outputAudioMixerGroup;
        Music2.outputAudioMixerGroup = Game.instance.audioSource.outputAudioMixerGroup;

        // Play Music1 if it is enabled
        if (Music1.enabled)
            Music1.Play();

        // Ensure Music1 starts with volume 0
        Music1.volume = 0;
    }

    public void SenseTension()
    {
        // Start the tension coroutine
        StartCoroutine(Tension());
    }

    public void ZeroTension()
    {
        // Start the calm tension coroutine
        StartCoroutine(CalmTension());
    }

    public void Danger()
    {
        // Set countdown to 12 seconds
        countDown = 12;

        // Trigger danger sequence if not already in danger or dead
        if (!danger && !dead)
        {
            if (ShopKeeping.instance)
                ShopKeeping.instance.Scare();

            // Start the danger fade coroutine
            StartCoroutine(FadeToDanger());
            danger = true;
        }
    }

    public void Death()
    {
        // Trigger death sequence if not already dead
        if (!dead)
        {
            // Stop existing music and set DeathMusic
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

    public void SetMaxVolume(float x)
    {
        // Set the maximum volume
        MaxVolume = x;

        // Adjust volume based on current state
        if (danger)
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

    public void ForceChill()
    {
        // Force chill after a delay
        Invoke(nameof(DoForceChill), 1);
    }

    private void DoForceChill()
    {
        // Reset the countdown to 0
        countDown = 0;
    }

        private IEnumerator Tension()
    {
        // Gradually increase the volume of Music1 to MaxVolume
        while (Music1.volume < MaxVolume)
        {
            Music1.volume += Time.deltaTime * 0.1f;

            // Clamp the volume to MaxVolume and set tension
            if (Music1.volume == 1.0 || Music1.volume > MaxVolume)
            {
                Music1.volume = MaxVolume;
                tensionTarget = MaxVolume;

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
            Music1.volume -= Time.deltaTime * 0.1f;

            // Set tension to false when volume reaches 0
            if (Music1.volume == 0.0)
            {
                tensionTarget = 0;

                tension = false;
                yield break;
            }

            yield return null;
        }
    }

    private IEnumerator FadeToDanger()
    {
        // Gradually decrease Music1 volume and increase Music2 volume to MaxVolume
        while (true)
        {
            Music1.volume -= Time.deltaTime;
            Music2.volume += Time.deltaTime;

            if (Music2.volume >= MaxVolume)
            {
                Music2.volume = MaxVolume;
                Music1.volume = 0;

                tensionTarget = 1;

                // Start the countdown coroutine
                StartCoroutine(CountDown());
                yield break;
            }

            yield return null;
        }
    }

    private IEnumerator CountDown()
    {
        // Countdown loop for danger state
        while (true)
        {
            countDown--;

            if (countDown == 0)
            {
                if (!dead)
                {
                    // Start the fade to chill coroutine
                    StartCoroutine(FadeToChill());
                    yield break;
                }
            }

            yield return new WaitForSeconds(1);
        }
    }

    private IEnumerator FadeToChill()
    {
        // Gradually adjust volumes to transition out of danger state
        while (true)
        {
            if (Music1.volume != tensionTarget)
            {
                Music1.volume = Mathf.MoveTowards(Music1.volume, tensionTarget, Time.deltaTime * 0.5f);
            }

            Music2.volume -= Time.deltaTime;

            if (Music2.volume == 0 && Music1.volume == tensionTarget)
            {
                danger = false;
                yield break;
            }

            yield return null;
        }
    }
}
