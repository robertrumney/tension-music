using UnityEngine;
using System.Collections;

public class GameMusic : MonoBehaviour
{
    #region VARS AND REFS
    public static GameMusic instance;

    public AudioSource Music1;
    public AudioSource Music2;
    public AudioClip DeathMusic;

    [System.NonSerialized]
    public bool ingozi = false;

    public float MaxVolume = 1;
    public float musicFadeSpeed = 5;

    private float countDown;

    private bool tension = false;
    private bool dead = false;
    #endregion

    #region INIT
    private void Awake()
    {
        instance = this;

        Music1.volume = 0;

        AudioListener.volume = 0;

        MaxVolume = PlayerPrefs.GetFloat("GameMusicVolume");
    }

    private void Start()
    {
        Music1.outputAudioMixerGroup = Game.instance.audioSource.outputAudioMixerGroup;
        Music2.outputAudioMixerGroup = Game.instance.audioSource.outputAudioMixerGroup;

        if (Music1.enabled)
            Music1.Play();

        Music1.volume = 0;
    }
    #endregion

    #region METHODS

    public void SenseTension()
    {
        StartCoroutine(Tension());
    }

    public void ZeroTension()
    {
        StartCoroutine(CalmTension());
    }

    private IEnumerator Tension()
    {
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

                if (ShopKeeping.instance)
                    ShopKeeping.instance.Recover();

                yield break;
            }

            yield return null;
        }
    }
    #endregion
}
