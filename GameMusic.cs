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
            Music1.volume += Time.deltaTime * 0.1f;

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
        while (Music1.volume > 0)
        {
            Music1.volume -= Time.deltaTime * 0.1f;

            if (Music1.volume == 0.0)
            {
                tensionTarget = 0;

                tension = false;
                yield break;
            }

            yield return null;
        }
    }

    public void Danger()
    {
        countDown = 12;

        if (!danger && !dead)
        {
            if (ShopKeeping.instance)
                ShopKeeping.instance.Scare();

            StartCoroutine(FadeToDanger());
            danger = true;
        }
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

    public void SetMaxVolume(float x)
    {
        MaxVolume = x;

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
        Invoke(nameof(DoForceChill), 1);
    }

    private void DoForceChill()
    {
        countDown = 0;
    }

    private IEnumerator FadeToDanger()
    {
        while (true)
        {
            Music1.volume -= Time.deltaTime;
            Music2.volume += Time.deltaTime;

            if (Music2.volume >= MaxVolume)
            {
                Music2.volume = MaxVolume;
                Music1.volume = 0;

                tensionTarget = 1;

                StartCoroutine(CountDown());
                yield break;
            }

            yield return null;
        }
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
