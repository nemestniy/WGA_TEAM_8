using System.Collections.Generic;
using System.Security.Cryptography;
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
    [SerializeField]
    private bool _constantEnergy;

    private bool _isOnMinEnergy;
    private List<EnergyAffecter> _energyAffecters = new List<EnergyAffecter>();
    


    public float CurrentEnergyLvl => _currentEnergy / _maxEnergy;
    
    public delegate void OnRanoutOfEnergyAction();
    public static event OnRanoutOfEnergyAction OnRanoutOfEnergy; // событие для вызова катсцены смерти игрока когда его съели

    private void Awake()
    {
        _currentEnergy = _startingEnergy;
    }

    private void Update()
    {

        foreach (var affecter in _energyAffecters)
        {
            float frameChange;
            if (affecter.timeToAffect == 0)
            {
                frameChange = affecter.amount; //to change energy lvl instantly
            }
            else
            {
                frameChange = affecter.amount * (Time.deltaTime / affecter.timeToAffect); //compute amount of energy lvl changing for this frame
                affecter.timeLeft -= Time.deltaTime;
            }
            _currentEnergy = Mathf.Clamp(_currentEnergy + frameChange, _minEnergy, _maxEnergy); //apply energy changing in for this frame
        }

        _energyAffecters.RemoveAll(e => e.timeLeft <= 0); //delete all consumed energy changers 

        if (_currentEnergy == _minEnergy)
        {
            if (!_isOnMinEnergy)
            {
                OnRanoutOfEnergy?.Invoke();
                _isOnMinEnergy = true;
            }
        }
        else
        {
            _isOnMinEnergy = false;
        }
    }

    public void ChangeEnergyLvl(float amount, float timeToChange = 0)
    {
        if (_constantEnergy)
            return;
        
        _energyAffecters.Add(new EnergyAffecter(amount, timeToChange));
    }
    
    public class EnergyAffecter
    {
        public float amount;
        public float timeToAffect;
        public float timeLeft;

        public EnergyAffecter(float amount, float timeToAffect)
        {
            this.amount = amount;
            this.timeToAffect = timeToAffect;
            this.timeLeft = timeToAffect;
        }
    }
}

