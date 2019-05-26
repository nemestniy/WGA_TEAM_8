using UnityEngine;
using System.Collections.Generic;
using System;
using Pathfinding;

public class EnemyDeepWaterer : MonoBehaviour, IEnemy
{
    public Player player => Player.Instance;
    public AstarPath path;

    [SerializeField] [ShowOnly] private State state;
    private EnemySavedState savedState;

    public float speed;
    public float _normalAnimationSpeed;
    
    public float maneurAngle;
    public float speedIncreaseCoefficient;

    public float distanceToPlayer;
    public float eventHorizon;// расстояние на котором враг уже не будет пытаться увернуться от луча фонаря, а просто пойдет напролом (як горизонт событий черной дыры)

    [HideInInspector] public delegate void EnemyEvents();
    [HideInInspector] public event EnemyEvents Player_death;

    public AIDestinationSetter destinationSetter;
    public AIPath aiPath;

    public float time;
    public bool inLight;
    [HideInInspector] public bool escapePointCreated;
    [HideInInspector] public bool maneurPointCreated;

    [SerializeField]
    private bool _isDeadly;
    
    private Animator animator;
    private static readonly int IsRunning = Animator.StringToHash("IsRunning");
    private static readonly int GotDamage = Animator.StringToHash("GotDamage");

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    
    

    public void Start()
    {
        //player = Player.Instance;
        destinationSetter = GetComponent<AIDestinationSetter>();
        aiPath = GetComponent<AIPath>();
        inLight = false;
        escapePointCreated = false;
        maneurPointCreated = false;

        state = State.Waiting;
    }

    private void Update()
    {
        switch (state) // Действия моба в зависимости от состояния
        {
            case State.Moving:
                Move();
                break;
            case State.Hunting:
                //
                break;
            case State.Escaping:
                if(Application.loadedLevelName == "TutorialTestScene")
                {
                    Escape(Tutorial.Instance.escapeObject);
                }
                else
                {
                    Escape(); 
                }
                break;
            case State.Maneuring:
                Maneur();
                break;
            case State.Waiting:
                GetComponent<AIPath>().canMove = false;
                StopAnimation();
                break;
        }
        distanceToPlayer = Vector2.Distance(player.transform.position, transform.position);
        
    }

    

    public delegate void OnTriggerAction();
    public static event OnTriggerAction OnTrigger; // событие для вызова катсцены смерти игрока когда его съели


    private void OnCollisionEnter2D(Collision2D other) 
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            other.transform.GetChild(0).GetComponent<Animator>().SetTrigger(GotDamage);
            AudioManager.Instance.TriggerSoundEvent("DW attack");
            if (_isDeadly)
            {
                OnTrigger?.Invoke();
            }
        }

        if (other.transform.tag == "Player")
            state = State.Escaping;
    }

    public void Escape() // Сбежать в ужасе. Состояние - Escaping
    {
        if (!escapePointCreated)
        {
            float r = UnityEngine.Random.Range(50f, 70f);

            float Y = transform.position.y - player.transform.position.y;
            float X = transform.position.x - player.transform.position.x;

            var direction = new Vector2(X, Y).normalized;

            GraphNode escapePoint = AstarPath.active.GetNearest(direction * r).node;

            if (escapePoint.Walkable)
            {
                GameObject escapeObject = new GameObject("EscapePoint");
                escapeObject.transform.position = (Vector3)escapePoint.position;
                destinationSetter.target = escapeObject.transform;
                aiPath.maxSpeed = 20f;
                escapePointCreated = true; 
            }
        }
        
    }

    public void Escape(GameObject escapeObject) // Сбежать в ужасе. Состояние - Escaping
    {
        destinationSetter.target = escapeObject.transform;
        aiPath.maxSpeed = 20f;
        escapePointCreated = true;
    }

    private void Maneur() // Маневрирование(уклонение) от луча. Состояние - Frying
    {
        if(!maneurPointCreated)// Доделать проверку на свободное место для маневра
        {
            float r = UnityEngine.Random.Range(5f, 10f);
            float side = UnityEngine.Random.Range(-10, 10) > 0 ? -1 : 1;
            Debug.LogWarning("side = " + side);
            float alpha = (player.transform.eulerAngles.z + maneurAngle * side) * Mathf.PI / 180;

            float Y = player.transform.position.y - (Mathf.Sin(alpha) * r);
            float X = player.transform.position.x + (Mathf.Cos(alpha) * r);

            GraphNode maneurPoint = AstarPath.active.GetNearest(new Vector2(X, Y)).node;

            if (maneurPoint.Walkable)
            {
                GameObject maneurObject = new GameObject("ManeurPoint");
                maneurObject.transform.position = (Vector3)maneurPoint.position;
                destinationSetter.target = maneurObject.transform;
                aiPath.maxSpeed = speed * speedIncreaseCoefficient;
                maneurPointCreated = true;
            }
        }
    }

    public void StartAnimation()
    {
        if(animator != null)
            animator.SetBool(IsRunning, true);
    }

    public void StopAnimation()
    {
        if(animator != null)
            animator.SetBool(IsRunning, false);
    }

    private void Move() // Движение по вычисленному пути до игрока. Состояние - Moving
    {
        //player = FindObjectOfType<Player>();
        StartAnimation();
        aiPath.maxSpeed = speed;

        destinationSetter.target = player.transform;
        GetComponent<AIPath>().canMove = true;
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public AIDestinationSetter GetDestinationSetter()
    {
        return destinationSetter;
    }

    public void SetState(State state)
    {
        this.state = state;
    }

    public EnemyType GetEnemyType()
    {
        return EnemyType.DeepWaterer;
    }

    public void SetOnLight()
    {
        inLight = true;
    }

    public void SetOutOfLight()
    {
        inLight = false;
    }

    public State GetState()
    {
        return state;
    }

    public void SaveState()
    {
        savedState = new EnemySavedState(state, destinationSetter.target, transform.position);
    }

    public void RestoreState()
    {
        throw new NotImplementedException();
    }

    public void Pause()
    {
//        aiPath.canMove = false;
        state = State.Waiting;
        StopAnimation();
    }

    public void Resume()
    {
//        aiPath.canMove = true;
        state = State.Moving;
        StartAnimation();
    }
}
