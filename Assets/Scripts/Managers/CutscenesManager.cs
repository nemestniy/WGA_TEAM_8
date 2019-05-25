using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class CutscenesManager : MonoBehaviour
{
    private AudioSource _audioSource;
    private VideoPlayer _videoPlayer;
    private GameObject _imageGO;
    private Image _image;
    private GameObject addUI = null;
    private Animator _gameProcessAnimator;
    private static readonly int Next = Animator.StringToHash("Next");

    #region Singletone
    public static CutscenesManager Instance { get; private set; }
    public CutscenesManager() : base()
    {
        Instance = this;
    }
    #endregion
    
    private void Awake()
    {
        _imageGO = transform.GetChild(0).gameObject;
        _image = _imageGO.GetComponent<Image>();
        _audioSource = GetComponent<AudioSource>();
        _videoPlayer = GetComponent<VideoPlayer>();
    }

    private void Start()
    {
        if(SceneManager.GetActiveScene().name == "ReleaseScene")//TODO:КАСТЫЫЫЫЫЫЫль
            _gameProcessAnimator = GameManager.Instance.GetComponent<Animator>();
    }

    public void ShowVideo(VideoClip video)
    {
        _videoPlayer.clip = video;
        _videoPlayer.Play();
        _videoPlayer.loopPointReached += GoToNextGameState;
    }

    public IEnumerator ShowFrames(Cutscene cutscene)
    {
        if(addUI != null) Destroy(addUI);
        
        _audioSource.clip = cutscene.sound;
        _imageGO.SetActive(true);
        _audioSource.Play();
        
        if (cutscene.additionalUI)
        {
            addUI =  Instantiate(cutscene.additionalUI);
        }
        
        foreach (var frame in cutscene.frames)
        {
            float timeLeft = frame.secondsToChange;
            _image.sprite = frame.image;
            
            while (!((frame.canChangeWithClick && Input.GetKey(KeyCode.Mouse0))|| timeLeft < 0))
            {
                timeLeft -= Time.deltaTime;
                yield return null;
            }
        }
        _imageGO.SetActive(false);
        _audioSource.Stop();


        if (SceneManager.GetActiveScene().name == "ReleaseScene") //TODO:КАСТЫЫЫЫЫЫЫль
        {
            if (addUI != null)
            {
                Destroy(addUI);
            }
            else
            {
                _gameProcessAnimator.SetTrigger(Next);
            }
        }
    }

    private void GoToNextGameState(VideoPlayer vp)
    {
        vp.clip = null;
        _gameProcessAnimator.SetTrigger(Next);
    }
}

