using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionStateBehaviour : StateMachineBehaviour
{
    [SerializeField]
    private int _prevModeNum;
    [SerializeField]
    private int _nextModeNum;
    
    [SerializeField]
    private float _timeToChange = 1;
    
    [Header("Audio:")]
    [SerializeField]
    private AudioClip _stateSound;
    
    private float _currentTime;
    private AudioSource _lampsAudioSource;
    private static readonly int HasChanged = Animator.StringToHash("HasChanged");

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _currentTime = 0;
        
        _lampsAudioSource = animator.GetComponent<AudioSource>();
        _lampsAudioSource.clip = _stateSound;
        
        AudioManager.OnAudioStart += StartSound;
        AudioManager.OnAudioPause += PauseSound;
        AudioManager.OnAudioResume += ResumeSound;
        
        if(!AudioManager.Instance.Paused)
            _lampsAudioSource.Play();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        if (_currentTime < _timeToChange)
        {
            _currentTime += Time.deltaTime;
        }
        else
        {
            animator.SetTrigger(HasChanged);
        }
        Player.Instance.transform.GetChild(0).GetComponent<Lamp>().SetLightMode(_nextModeNum, _prevModeNum,_currentTime / _timeToChange); 
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
