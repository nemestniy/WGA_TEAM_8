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

    [SerializeField] private State state;

    public float speed;

    public delegate void OnTriggerAction();
    public static event OnTriggerAction OnTrigger;

    [HideInInspector] public delegate void EnemyEvents();
    [HideInInspector] public event EnemyEvents Player_death;

    public void Start()
    {
        player = Player.Instance;
        destinationSetter = GetComponent<AIDestinationSetter>();
        aiPath = GetComponent<AIPath>();
        inLight = false;
        GetComponent<AIPath>().canMove = false;
    }

    private void Update()
    {
        switch (state) // Действия моба в зависимости от состояния
        {
            case State.Waiting:
                GetComponent<AIPath>().canMove = false;
                break;
            case State.Moving:
                GetComponent<AIPath>().canMove = true;
                break;
            case State.Hunting:
                
                break;
            case State.Escaping:
                
                break;
            case State.Maneuring:
                
                break;
        }
        distanceToPlayer = Vector2.Distance(player.transform.position, transform.position);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            OnTrigger?.Invoke();
        }
    }

    public AIDestinationSetter GetDestinationSetter()
    {
        return destinationSetter;
    }

    public EnemyType GetEnemyType()
    {
        return EnemyType.Statue;
    }

    public State GetState()
    {
        return state;
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public void SetOnLight()
    {
        inLight = true;
    }

    public void SetOutOfLight()
    {
        inLight = false;
    }

    public void SetState(State state)
    {
        Debug.Log("Setting state");
        this.state = state;
    }
    
}

