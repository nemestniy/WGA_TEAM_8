using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour, Manager
{
    
    [SerializeField]
    private List<AudioClip> _audioClips;
    
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
<<<<<<< HEAD

        StartCoroutine(PlayBackgroundMusic(_backgroundMusic));
=======
        
        StartCoroutine(PlayBackgroundMusic(_audioClips));
>>>>>>> parent of 4de105c... Merge branch 'master' of https://github.com/nemestniy/WGA_TEAM_8
        IsLoaded = true;
        _paused = false;
        Debug.Log("AudioManager Started");
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
