using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateAudio : StateMachineBehaviour
{
    [SerializeField]
    private AudioClip _stateSound;

    private AudioSource _lampsAudioSource;
    
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _lampsAudioSource = animator.GetComponent<AudioSource>();
        _lampsAudioSource.clip = _stateSound;
        
        AudioManager.OnAudioStart += StartSound;
        AudioManager.OnAudioPause += PauseSound;
        AudioManager.OnAudioResume += ResumeSound;
        
        if(!AudioManager.Instance.Paused)
            _lampsAudioSource.Play();
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _lampsAudioSource.Stop();
    }
    
    private void StartSound()
    {
        _lampsAudioSource.Play();
    }
    

    private void PauseSound()
    {
        _lampsAudioSource.Pause();
    }

    private void ResumeSound()
    {
        _lampsAudioSource.UnPause();
    }
}
