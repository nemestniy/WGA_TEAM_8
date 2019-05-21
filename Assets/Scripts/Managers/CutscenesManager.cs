using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CutscenesManager : MonoBehaviour
{
    private AudioSource _audioSource;
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
    }

    private void Start()
    {
        _gameProcessAnimator = GameManager.Instance.GetComponent<Animator>();
    }

    public IEnumerator Show(Cutscene cutscene)
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

