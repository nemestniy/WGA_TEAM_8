using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Pathfinding;

public interface IEnemy
{
    Transform GetTransform();
    AIDestinationSetter GetDestinationSetter();
    State GetState();
    void SetState(State state);
    EnemyType GetEnemyType();
    void SetOnLight();
    void SetOutOfLight();
    void SaveState();
    void RestoreState();
    void Pause();
    void Resume();

    
}

