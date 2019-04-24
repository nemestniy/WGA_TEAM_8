using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWalkBehaviour : StateMachineBehaviour
{
    [SerializeField] private AudioClip _stoneSteps;
    [SerializeField] private AudioClip _sendSteps;
    [SerializeField] private AudioClip _waterSteps;

    private AudioSource _playersAudioSource;
    private GameManager _gameManager;
    
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _playersAudioSource = Player.Instance.GetComponent<AudioSource>();
        _gameManager = GameManager.Instance;
    }
    
    private void OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        switch (_gameManager.CurrentBiome)
        {
            case BackgroundController.Biome.Water:
                _playersAudioSource.clip = _waterSteps;
                break;
            case BackgroundController.Biome.Sandy:
                _playersAudioSource.clip = _sendSteps;
                break;
            case BackgroundController.Biome.Rocky:
                _playersAudioSource.clip = _stoneSteps;
                break;
        }
        _playersAudioSource.Play();
    }
}
