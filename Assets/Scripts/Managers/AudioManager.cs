using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour, Manager
{
    
    [SerializeField]
    private List<AudioClip> _audioClips;
    
    private AudioSource _audioSource;
    private bool _paused;
    
    
    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
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
        StartCoroutine(PlayBackgroundMusic(_audioClips));
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
}
