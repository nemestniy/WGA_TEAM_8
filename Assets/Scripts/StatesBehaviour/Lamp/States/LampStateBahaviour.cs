﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LampStateBahaviour : StateMachineBehaviour
{
    [SerializeField]
    private int _currentModeNum;
    
    [Header("Energy:")]
    [SerializeField]
    private float _energyCost;
    [SerializeField]
    private float _costDelay;
    [Header("Audio:")]
    [SerializeField]
    private AudioClip _stateSound;

    private AudioSource _lampsAudioSource;
    private float _timePast = 0;
    private Energy _playersEnergy;
    
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //1 means mode is not changing
        Player.Instance.transform.GetChild(0).GetComponent<Lamp>().SetLightMode(_currentModeNum,_currentModeNum,1); 
        _playersEnergy = Player.Instance.GetComponent<Energy>();
        
        _lampsAudioSource = animator.GetComponent<AudioSource>();
        _lampsAudioSource.clip = _stateSound;
        
        AudioManager.OnAudioStart += StartSound;
        AudioManager.OnAudioPause += PauseSound;
        AudioManager.OnAudioResume += ResumeSound;
        
        if(!AudioManager.Instance.Paused)
            _lampsAudioSource.Play();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(_costDelay == 0)
            return;
        
        if (_timePast > _costDelay)
        {
            _playersEnergy.ChangeEnergyLvl(-_energyCost); //affect by negative value of energy cost
        }
        else
        {
            _timePast += Time.deltaTime;
        }
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
