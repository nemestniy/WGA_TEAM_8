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

    protected void TransitionOnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _currentTime = 0;
        
        _lampsAudioSource = animator.GetComponent<AudioSource>();
        _lampsAudioSource.clip = _stateSound;
        _lampsAudioSource.Play();
    }

    protected void TransitionOnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
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
    
    protected void TransitionOnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _lampsAudioSource.Stop();
    }
}
