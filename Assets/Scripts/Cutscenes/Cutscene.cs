using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cutscene : MonoBehaviour
{
    
    [SerializeField]
    private List<Frame> _frames;
        
    private AudioSource _audioSource;
    private GameObject _imageGO;
    private Image _image;

    private bool _wasClicked;

    private void Awake()
    {
        _imageGO = transform.GetChild(0).gameObject;
        _image = _imageGO.GetComponent<Image>();
        _audioSource = GetComponent<AudioSource>();
    }

    public IEnumerator Show(Action whatToDoNext)
    {
        _imageGO.SetActive(true);
        _audioSource.Play();
        
        foreach (var frame in _frames)
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

    
    [System.Serializable]
    struct Frame
    {
        [SerializeField]
        public Sprite image;
        [SerializeField]
        public float secondsToChange;
        [SerializeField]
        public bool canChangeWithClick;
    }
}
