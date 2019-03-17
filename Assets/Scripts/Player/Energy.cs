using UnityEngine;

public class Energy : MonoBehaviour
{
    [SerializeField]
    private float _startingEnergy = 100;
    [ShowOnly, SerializeField] 
    private float _currentEnergy;

    [SerializeField] private bool _alwaysFull;

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
        if (_currentEnergy - amount >= 0 && !_alwaysFull)
        {
            _currentEnergy -= amount;
        }
    }
}

