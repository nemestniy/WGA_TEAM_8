using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Cutscenes
{
    public class Cutscene : MonoBehaviour
    {
        [SerializeField]
        private AudioClip _music;
        [SerializeField]
        private List<Frame> _frames;
        
        private AudioSource _audioSource;

        public void Show()
        {
            _audioSource = GetComponent<AudioSource>();
            _audioSource.clip = _music;
            _audioSource.Play();
        }

        [System.Serializable]
        struct Frame
        {
            [SerializeField]
            private Sprite image;
            [SerializeField]
            private float secondsToChange;
            [SerializeField]
            private bool canChngeWhithClick;
        }
    }
    
}
