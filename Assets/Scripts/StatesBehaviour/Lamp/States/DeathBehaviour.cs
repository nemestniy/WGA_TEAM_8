using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBehaviour : StateMachineBehaviour
{
    private const int CURRENT_MODE_NUM = 3;
    
    
    [SerializeField]
    private float _energyCost;
    [SerializeField]
    private float _costDelay;

    private float _timePast = 0;
    private Energy _playersEnergy;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //1 means mode is not changing
        Player.Instance.transform.GetChild(0).GetComponent<Lamp>().SetLightMode(CURRENT_MODE_NUM,CURRENT_MODE_NUM,1); 
        _playersEnergy = Player.Instance.GetComponent<Energy>();
    }
    
    private void OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        if (_timePast > _costDelay)
        {
            _playersEnergy.ChangeEnergyLvl(-_energyCost, 0); //affect by negative value of energy cost
        }
        else
        {
            _timePast += Time.deltaTime;
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    }
}
