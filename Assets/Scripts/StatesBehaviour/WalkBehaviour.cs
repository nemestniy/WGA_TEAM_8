using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkBehaviour : StateMachineBehaviour
{
    [SerializeField] private AudioClip _stoneSteps;
    [SerializeField] private AudioClip _sendSteps;
    [SerializeField] private AudioClip _waterSteps;

    private AudioSource _ememysAudioSource;
    private MapManager _mapManager;
    
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _ememysAudioSource = animator.GetComponent<AudioSource>();
        _mapManager = MapManager.Instance;

//        switch (_mapManager.Background.GetBiomeByPosition(animator.GetComponent<Transform>().position))
//        {
//            case BackgroundController.Biome.Water:
//                _ememysAudioSource.clip = _waterSteps;
//                break;
//            case BackgroundController.Biome.Sandy:
//                _ememysAudioSource.clip = _sendSteps;
//                break;
//            case BackgroundController.Biome.Rocky:
//                _ememysAudioSource.clip = _stoneSteps;
//                break;
//        }
//        AudioManager.OnAudioStart += StartSound;
//        AudioManager.OnAudioPause += PauseSound;
//        AudioManager.OnAudioResume += ResumeSound;
//        
//        if(!AudioManager.Instance.Paused)
//            _ememysAudioSource.Play();
    }
    
    private void OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
//        switch (_mapManager.Background.GetBiomeByPosition(animator.GetComponent<Transform>().position))
//        {
//            case BackgroundController.Biome.Water:
//                _ememysAudioSource.clip = _waterSteps;
//                break;
//            case BackgroundController.Biome.Sandy:
//                _ememysAudioSource.clip = _sendSteps;
//                break;
//            case BackgroundController.Biome.Rocky:
//                _ememysAudioSource.clip = _stoneSteps;
//                break;
//        }
    }
    
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
//        _ememysAudioSource.Stop();
    }
    
//    private void StartSound()
//    {
//        _ememysAudioSource.Play();
//    }
//    
//
//    private void PauseSound()
//    {
//        _ememysAudioSource.Pause();
//    }
//
//    private void ResumeSound()
//    {
//        _ememysAudioSource.UnPause();
//    }
}
