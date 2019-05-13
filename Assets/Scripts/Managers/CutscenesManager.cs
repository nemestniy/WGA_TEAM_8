using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CutscenesManager : MonoBehaviour
{
    [SerializeField]
    private List<Cutscene> _cutsceens;
    
    private AudioSource _audioSource;
    private GameObject _imageGO;
    private Image _image;
    private Cutscene _currentCutscene;
    
    public static CutscenesManager Instance { get; private set; }
    public CutscenesManager() : base()
    {
        Instance = this;
    }
    
    private void Awake()
    {
        _imageGO = transform.GetChild(0).gameObject;
        _image = _imageGO.GetComponent<Image>();
        _audioSource = GetComponent<AudioSource>();
    }
    
    public IEnumerator Show(string cutsceneName, Action whatToDoNext)
    {
        _currentCutscene = _cutsceens.Find(c => c.cutsceneName.Equals(cutsceneName)); //looking for the desired cutscene

        _audioSource.clip = _currentCutscene.sound;
        _imageGO.SetActive(true);
        _audioSource.Play();
        
        foreach (var frame in _currentCutscene.frames)
        {
            float timeLeft = frame.secondsToChange;
            _image.sprite = frame.image;
            
            while (!((frame.canChangeWithClick && Input.GetKey(KeyCode.Mouse0))|| timeLeft < 0))
            {
                timeLeft -= Time.deltaTime;
                yield return null;
            }
        }
        
        _audioSource.Stop();
        _imageGO.SetActive(false);

        whatToDoNext?.Invoke();
    }
}

