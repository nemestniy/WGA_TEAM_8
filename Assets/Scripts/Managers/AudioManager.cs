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
    [SerializeField] private List<SoundStatePair> _soundStates;
    
    private AudioSource _audioSource;
    private bool _paused;

    public static AudioManager Instance { get; private set; }
    public bool IsLoaded { get; private set; }
    
    public AudioManager() : base()
    {
        Instance = this;
    }
    
    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }
    
    public void TriggerSoundEvent(string audioEventName)
    {
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
            foreach (var clip in audioClips)
            {
                _audioSource.clip = clip;
                _audioSource.Play();
                
                float timePassed = 0;
                while (timePassed < _audioSource.clip.length)
                {
                    if (!_paused)
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
        _paused = false;
    }

    public void PauseManager()
    {
        _audioSource.Pause();
        _paused = true;
    }
    
    public void ResumeManager()
    {
        _audioSource.UnPause();
        _paused = false;
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
