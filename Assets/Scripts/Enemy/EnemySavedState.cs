using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

struct EnemySavedState
{
    public EnemySavedState(State state, Transform target, Vector3 position)
    {
        this.lastState = state;
        this.lastTarget = target;
        this.lastPosition = position;
    }

    public Transform lastTarget;
    public Vector3 lastPosition;
    public State lastState;
}

