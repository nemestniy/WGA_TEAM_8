using System.Collections.Generic;
using UnityEngine;

public class StateBlinking : StateMachineBehaviour
{
    [Header("On cost:")]
    [SerializeField]
    private bool _isBlinkingOnCost;
    [SerializeField]
    private float _blinkDurationOnCost;
    [SerializeField]
    private List<Lamp.Period> _periodsOnCost;
    [SerializeField]
    private AnimationCurve _fadingCurveOnCost;
    
    [Header("On low energy:")]
    [SerializeField] 
    private float _energyLimit;
    [SerializeField]
    private float _blinkDurationOnLow;
    [SerializeField]
    private List<Lamp.Period> _periodsOnLow;
    [SerializeField]
    private AnimationCurve _fadingCurveOnLow;
    
    private bool _blinkedOnCost;
    private float _prevEnLvl;
    private static readonly int EnergyLvl = Animator.StringToHash("EnergyLvl");


    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _blinkedOnCost = false;
        _prevEnLvl = animator.GetFloat(EnergyLvl);
    }


    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //blinking on low energy
        if (animator.GetFloat(EnergyLvl) < _energyLimit && _energyLimit <= _prevEnLvl)
        {
            animator.GetComponent<Lamp>().BlinkLight(_fadingCurveOnLow, _blinkDurationOnLow,_periodsOnLow);
        }
        
        //blinking on cost
        if (_isBlinkingOnCost && animator.GetFloat(EnergyLvl) != _prevEnLvl && !_blinkedOnCost)
        {
            animator.GetComponent<Lamp>().BlinkLight(_fadingCurveOnCost, _blinkDurationOnCost,_periodsOnCost);
            _blinkedOnCost = true;
        }
        
        _prevEnLvl = animator.GetFloat(EnergyLvl);
    }
}
