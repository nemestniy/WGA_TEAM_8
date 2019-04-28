using System.Collections.Generic;
using UnityEngine;

public class StepsSound : MonoBehaviour
{
    [SerializeField]
    private List<AudioClip> _stepsRock;
    
    [SerializeField]
    private List<AudioClip> _stepsWater;
    
    [SerializeField]
    private List<AudioClip> _stepsSand;
    
    [SerializeField]
    private float volLowRange = .5f;
    [SerializeField]
    private float volHighRange = 1.0f;

    private AudioSource _audioSource;
    private MapManager _mapManager;
    private System.Random rnd;
        
    private void Awake()
    {
        _mapManager = MapManager.Instance;
        rnd = new System.Random();
        _audioSource = GetComponent<AudioSource>();
    }

    public void PlayStep()
    {
        float vol = Random.Range (volLowRange, volHighRange);
        _audioSource.PlayOneShot(GetStepFromBiom(_stepsRock,_stepsWater, _stepsSand), vol);
    }

    private AudioClip GetStepFromBiom(List<AudioClip> stepsRock, List<AudioClip> stepsWater, List<AudioClip> stepsSand)
    {
        switch (_mapManager.Background.GetBiomeByPosition(transform.position))
        {
            case BackgroundController.Biome.Water:
                return GetRandomSound(stepsWater);
            case BackgroundController.Biome.Sandy:
                return GetRandomSound(stepsSand);
            case BackgroundController.Biome.Rocky:
                return GetRandomSound(stepsRock);
        }

        return null;
    }

    private AudioClip GetRandomSound(List<AudioClip> stepsList)
    {
        int r = rnd.Next(stepsList.Count);
        return stepsList[r];
    }
    
}
