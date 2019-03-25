using UnityEngine;

public class Energy : MonoBehaviour
{
    [SerializeField]
    private float _startingEnergy = 100;
    [SerializeField]
    private float _maxEnergy = 100;
    [SerializeField]
    private float _minEnergy = 0;
    [ShowOnly, SerializeField] 
    private float _currentEnergy;

    [SerializeField] private bool _constantEnergy;

    public float CurrentEnergy => _currentEnergy;

    private void Awake()
    {
        _currentEnergy = _startingEnergy;
    }

    public void ChangeEnergyLvl(float amount)
    {
        if (!_constantEnergy)
        {
            _currentEnergy = Mathf.Clamp(_currentEnergy + amount, _minEnergy, _maxEnergy);
        }
    }
}

