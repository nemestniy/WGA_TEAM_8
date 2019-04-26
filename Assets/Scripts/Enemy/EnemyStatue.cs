using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pathfinding;
using UnityEngine;

public class EnemyStatue : MonoBehaviour, IEnemy
{
    public Player player;
    public AstarPath path;

    public float distanceToPlayer;
    public float eventHorizon;

    public AIDestinationSetter destinationSetter;
    public AIPath aiPath;

    public float time;
    public bool inLight;

    [ShowOnly] public State state;

    public float speed;

    public void Start()
    {
        player = Player.Instance;
        destinationSetter = GetComponent<AIDestinationSetter>();
        aiPath = GetComponent<AIPath>();
        inLight = false;
    }

    private void Update()
    {
        switch (state) // Действия моба в зависимости от состояния
        {
            case State.WaySearching:
                
                break;
            case State.Moving:
                
                break;
            case State.Hunting:
                
                break;
            case State.Escaping:
                
                break;
            case State.Maneuring:
                
                break;
            case State.Paused:
                
                break;
        }
        distanceToPlayer = Vector2.Distance(player.transform.position, transform.position);
    }

    public AIDestinationSetter GetDestinationSetter()
    {
        throw new NotImplementedException();
    }

    public EnemyType GetEnemyType()
    {
        throw new NotImplementedException();
    }

    public State GetState()
    {
        throw new NotImplementedException();
    }

    public Transform GetTransform()
    {
        throw new NotImplementedException();
    }

    public void SetOnLight()
    {
        throw new NotImplementedException();
    }

    public void SetOutOfLight()
    {
        throw new NotImplementedException();
    }

    public void SetState(State state)
    {
        throw new NotImplementedException();
    }
    
}

