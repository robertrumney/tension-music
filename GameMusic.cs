using UnityEngine;
using System.Collections;

public class GameMusic : MonoBehaviour
{
    #region VARS AND REFS
    public static GameMusic instance;

    public AudioSource MusicNone;
    public AudioSource MusicDanger;
    public AudioSource MusicAction;

    public AudioClip DeathMusic;

    public float MaxVolume = 1;
    public float MusicFadeSpeed = 5;

    private enum MusicState { None, Action, Danger }
    private MusicState currentState = MusicState.None;

    private bool dead = false;
    #endregion

    #region INIT
    private void Awake()
    {
        instance = this;

        // Initialize volumes
        MusicNone.volume = 0;
        MusicDanger.volume = 0;
        MusicAction.volume = 0;

        MaxVolume = PlayerPrefs.GetFloat("GameMusicVolume", 0.5f);
    }

    private void Start()
    {
        SetMusicGroups();
        SwitchState(MusicState.Action); // Default to action state
    }

    private void SetMusicGroups()
    {
        var audioGroup = Game.instance.audioSource.outputAudioMixerGroup;
        MusicNone.outputAudioMixerGroup = audioGroup;
        MusicDanger.outputAudioMixerGroup = audioGroup;
        MusicAction.outputAudioMixerGroup = audioGroup;
    }
    #endregion

    #region STATE MANAGEMENT
    public void TriggerDanger()
    {
        if (currentState != MusicState.Danger && !dead)
        {
            SwitchState(MusicState.Danger);
        }
    }

    public void Death()
    {
        if (!dead)
        {
            StopAllMusic();
            PlayClip(MusicDanger, DeathMusic, 0.33f, loop: false);
            dead = true;
        }
    }

    private void SwitchState(MusicState newState)
    {
        if (currentState == newState) return;

        StopAllMusic();

        currentState = newState;
        switch (newState)
        {
            case MusicState.None:
                break;
            case MusicState.Action:
                PlayClip(MusicAction, loop: true);
                break;
            case MusicState.Danger:
                PlayClip(MusicDanger, loop: true);
                break;
        }
    }

    private void PlayClip(AudioSource source, AudioClip clip = null, float volume = -1, bool loop = true)
    {
        if (clip != null)
            source.clip = clip;
        source.volume = volume >= 0 ? volume : MaxVolume;
        source.loop = loop;
        source.Play();
    }

    private void StopAllMusic()
    {
        MusicNone.Stop();
        MusicDanger.Stop();
        MusicAction.Stop();
    }
    #endregion

    #region UTILITIES
    public void SetMaxVolume(float x)
    {
        MaxVolume = x;
        if (currentState != MusicState.None)
        {
            GetCurrentSource().volume = MaxVolume;
        }
    }

    private AudioSource GetCurrentSource()
    {
        switch (currentState)
        {
            case MusicState.Action:
                return MusicAction;
            case MusicState.Danger:
                return MusicDanger;
            default:
                return null;
        }
    }
    #endregion
}
