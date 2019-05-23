using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AudioManager : MonoBehaviour, Manager
{
    
    [SerializeField]
    private List<AudioClip> _backgroundMusic;

    [SerializeField] private List<SoundEventPair> _soundEvents;
    //    [SerializeField] private List<SoundStatePair> _soundStates;

    private CoreSoundtrack _coreSoundtrackManager;
    
    private AudioSource _audioSource;
    public bool Paused{ get; private set; }

    public bool IsLoaded { get; private set; }
    
    public static event Action OnAudioStart;
    public static event Action OnAudioPause;
    public static event Action OnAudioResume;

    #region Singletone
    public static AudioManager Instance { get; private set; }
    public AudioManager() : base()
    {
        Instance = this;
    }
    #endregion

    void Start()
    {
        _coreSoundtrackManager = GetComponent<CoreSoundtrack>();
    }

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        Paused = true;
    }
    
    public void TriggerSoundEvent(string audioEventName)
    {
        if (Paused)
            return;
        
        IEnumerable<SoundEventPair> calledSoundEvents = _soundEvents.Where(se => se.gameEvent.Equals(audioEventName));
        foreach (var soundEvent in calledSoundEvents)
        {
            _audioSource.PlayOneShot(soundEvent.audioEvent.sound, soundEvent.audioEvent.volume);
        }
    }
    
    
    private IEnumerator PlayBackgroundMusic(List<AudioClip> audioClips)
    {
        while (true)
        {
            if (audioClips.Count == 0) //in case of audioClips list is empty
                yield return null;
            
            foreach (var clip in audioClips)
            {
                if(clip.length == 0) //in case of clips length is 0
                    yield return null;
                
                _audioSource.clip = clip;
                _audioSource.Play();
                
                float timePassed = 0;
                while (timePassed < _audioSource.clip.length)
                {
                    if (!Paused)
                    {
                        timePassed += Time.deltaTime;
                    }
                    yield return null;
                }
            }
        }
    }

    public void StartManager()
    {
        StartCoroutine(PlayBackgroundMusic(_backgroundMusic));
        IsLoaded = true;
        Paused = false;
        OnAudioStart?.Invoke();
        _coreSoundtrackManager.PlayAll();
    }

    public void PauseManager()
    {
        _audioSource.Pause();
        Paused = true;
        OnAudioPause?.Invoke();
        _coreSoundtrackManager.Mute();
    }
    
    public void ResumeManager()
    {
        _audioSource.UnPause();
        Paused = false;
        OnAudioResume?.Invoke();
        _coreSoundtrackManager.Resume();
    }
    
    [Serializable]
    public struct SoundEventPair
    {
        public string gameEvent;
        public AudioEvent audioEvent;
    }
    
    [Serializable]
    public struct AudioEvent
    {
        public AudioClip sound;
        public float volume;
    }
    
    [Serializable]
    public struct SoundStatePair
    {
        public string gameState;
        public AudioEvent audioEvent;
    }
    
    [Serializable]
    public struct AudioState
    {
        public AudioClip sound;
        public float volume;
    }
}
