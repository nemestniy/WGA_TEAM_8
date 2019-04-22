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

    private float _blockTimeLeft = 0;
    

    [SerializeField] private bool _constantEnergy;

    public float CurrentEnergyLvl => _currentEnergy / _maxEnergy;
    
    public delegate void OnRanoutOfEnergyAction();
    public static event OnRanoutOfEnergyAction OnRanoutOfEnergy; // событие для вызова катсцены смерти игрока когда его съели

    private void Awake()
    {
        _currentEnergy = _startingEnergy;
    }

    private void FixedUpdate()
    {
        _blockTimeLeft =  (_blockTimeLeft - Time.deltaTime > 0) ? _blockTimeLeft - Time.deltaTime : 0; //decrease _blockTimeLeft every frame but not lower then 0
    }

    public void ChangeEnergyLvl(float amount, float blockTime)
    {
        if (!_constantEnergy && _blockTimeLeft == 0)
        {
            _currentEnergy = Mathf.Clamp(_currentEnergy + amount, _minEnergy, _maxEnergy);

            if (_currentEnergy == _minEnergy)
            {
                OnRanoutOfEnergy?.Invoke();
            }

            
            _blockTimeLeft = blockTime; //block all energy changes for blockTime
        }
    }
}

