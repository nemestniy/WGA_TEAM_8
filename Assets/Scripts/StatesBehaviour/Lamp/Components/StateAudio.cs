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
        if(_lampsAudioSource)//TODO:КАСТЫЫЫЛЬ
            _lampsAudioSource.Stop();
    }
    
    private void StartSound()
    {
        if(_lampsAudioSource)//TODO:КАСТЫЫЫЛЬ
            _lampsAudioSource.Play();
    }
    

    private void PauseSound()
    {
        if(_lampsAudioSource)//TODO:КАСТЫЫЫЛЬ
            _lampsAudioSource.Pause();
    }

    private void ResumeSound()
    {
        if(_lampsAudioSource)//TODO:КАСТЫЫЫЛЬ
            _lampsAudioSource.UnPause();
    }

    private void OnDestroy()
    {
        AudioManager.OnAudioStart -= StartSound;
        AudioManager.OnAudioPause -= PauseSound;
        AudioManager.OnAudioResume -= ResumeSound;
    }
}
