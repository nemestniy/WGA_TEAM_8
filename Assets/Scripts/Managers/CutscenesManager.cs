﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.Windows.Speech;

public class CutscenesManager : MonoBehaviour
{
    private AudioSource _audioSource;
    private VideoPlayer _videoPlayer;
    private GameObject _imageGO;
    private Image _image;
    private GameObject addUI = null;
    private Animator _gameProcessAnimator;
    private static readonly int Next = Animator.StringToHash("Next");

    public GameObject canvas;
    public GameObject textBox;

    #region Singletone
    public static CutscenesManager Instance { get; private set; }
    public CutscenesManager() : base()
    {
        Instance = this;
    }
    #endregion

    private string CharToRus(Character character)
    {
        switch (character)
        {
            case Character.Father: return "Father";
            case Character.Daughter: return "Daughter";
            case Character.Chtulhu: return "Mystery voice";
            case Character.Elder: return "The Elder";
            default: return "";
        }       
    }

    IEnumerator CutsceneDialogue(SubtitleInfo phrase)
    {
        Debug.Log($"Dialogue: {phrase}");
        canvas.active = true;
        textBox.GetComponent<Text>().text = "";        
        textBox.GetComponent<Text>().text += CharToRus(phrase.Speaker) + ": " + phrase.Subtitles;        
        yield return null;
        //Добавить произношение реплики героем
        Debug.Log("Dialogue +");
    }

    public IEnumerator CutsceneDialogue(SubtitleInfo[] dialogue, float delay)
    {
        canvas.active = true;
        float d = dialogue.Sum(a => a.Delay);        
        foreach (var phrase in dialogue)
        {            
            StartCoroutine(CutsceneDialogue(phrase));
            yield return new WaitForSeconds(d > delay - 0.05f ? phrase.Delay * ((delay - 0.05f) / d) : phrase.Delay);

        }
        canvas.active = false;
        textBox.GetComponent<Text>().text = "";
        
        yield return new WaitForSeconds(delay - dialogue.Sum(a => a.Delay));

    }

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

            StartCoroutine(CutsceneDialogue(frame.Subtitles, frame.secondsToChange));            
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

