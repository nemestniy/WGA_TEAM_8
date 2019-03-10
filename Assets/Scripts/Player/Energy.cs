using UnityEngine;

public class Energy : MonoBehaviour
{
    [SerializeField]
    private float _startingEnergy = 100;

    [ShowOnly, SerializeField] 
    private float _currentEnergy;

    public float CurrentEnergy
    {
        get => _currentEnergy;
    }

    private void Awake()
    {
        _currentEnergy = _startingEnergy;
    }

    public void TakeAwayEnergy(float amount)
    {
        _currentEnergy -= amount;
    }
}

