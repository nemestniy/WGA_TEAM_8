using UnityEngine;

public class AudioPauser : MonoBehaviour
{
    [SerializeField]
    private AudioClip _sound;

    private AudioSource _audioSource;
    
    public void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.clip = _sound;
        
        AudioManager.OnAudioStart += StartSound;
        AudioManager.OnAudioPause += PauseSound;
        AudioManager.OnAudioResume += ResumeSound;
        
        if(!AudioManager.Instance.Paused)
            _audioSource.Play();
    }
    
    private void StartSound()
    {
        _audioSource.Play();
    }
    

    private void PauseSound()
    {
        _audioSource.Pause();
    }

    private void ResumeSound()
    {
        _audioSource.UnPause();
    }
}
