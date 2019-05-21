using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateEnergy : StateMachineBehaviour
{
    
    [SerializeField]
    private float _energyCost;
    [SerializeField]
    private float _costDelay;
    
    private float _timePast = 0;
    private Energy _playersEnergy;
    private Lamp _playersLamp;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _playersEnergy = Player.Instance.GetComponent<Energy>();
        _playersLamp = Player.Instance.transform.GetChild(0).GetComponent<Lamp>();
        _timePast = 0;
    }


    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(_energyCost == 0)
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
}
