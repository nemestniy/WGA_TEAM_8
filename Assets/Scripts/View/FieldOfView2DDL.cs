using System;
using System.Collections.Generic;
using DynamicLight2D;
using UnityEngine;


public class FieldOfView2DDL : MonoBehaviour, IFieldOfView
{
	[SerializeField]
	private bool _isMain; //true for main light and false for back light
	
	[Header("Masks:")]
	[SerializeField]
	private LayerMask _enemyMask;
	[SerializeField]
	private LayerMask _obstacleMask;
	
	private Light _spotLight;
	private float _currentChangingTime;
	private Energy _energy;
	private List<Lamp.LampMode> _lampModes;
	private int _prevMode;
	private float _changingState = 1; //[0,1] shows how close mode to it's final state; 0 - start to change mode, 1 - not changing


	public int СurrentMode { private set; get; }

	private EnemyManager _enemyManager;
	private DynamicLight _visionArea;

	//light values
	[HideInInspector] public float _currentViewRadius;
	[HideInInspector] public float _currentSpotLightRadius;
	[HideInInspector] public float _currentViewAngle;
	[HideInInspector] public float _currentSpotLightAngle;
	[HideInInspector] public float _currentIntensity;
	[HideInInspector] public float _currentLightHeight;
	[HideInInspector] public float _currentCoordinateY;
	[HideInInspector] public Color _currentLightColor;
	
	public float CurrentIntensityMult { get; set; }
	public PolygonCollider2D VisionCollider { private set; get; }
	
	
	private void Start()
	{
		CurrentIntensityMult = 1;
		_spotLight = GetComponentInChildren<Light>();
		_energy =  GetComponentInParent<Energy>();        

		_enemyManager = EnemyManager.Instance;
		_lampModes = Player.Instance.transform.GetChild(0).GetComponent<Lamp>()._lampModes;

		VisionCollider = GetComponent<PolygonCollider2D>();
		_visionArea = GetComponent<DynamicLight>();
	}
	
	private void LateUpdate()
	{
		if (_isMain) //compute depending on is it main light or back light
		{
			ComputeCurrentLightValues(_energy.CurrentEnergyLvl,
				_lampModes[_prevMode].mainLight,
				_lampModes[СurrentMode].mainLight,
				_changingState);
		}
		else
		{
			ComputeCurrentLightValues(_energy.CurrentEnergyLvl,
				_lampModes[_prevMode].backLight,
				_lampModes[СurrentMode].backLight,
				_changingState);
		}
		
		//updating light values
		var position = transform.localPosition;
		transform.localPosition = new Vector3(position.x,_currentCoordinateY ,position.z);
		DrawFieldOfView(_currentViewRadius, _currentViewAngle);
		DrawSpotLight(_currentSpotLightRadius, _currentSpotLightAngle, _currentIntensity, _currentLightHeight, _currentLightColor);
	
		UpdateEnemiesVisibleEnemies();
		UpdateVisionCollider();
	}

    private void UpdateEnemiesVisibleEnemies()
    {
        _enemyManager.visibleEnemiesList = FindVisibleEnemies(_currentViewRadius, _currentViewAngle);
    }

    private void ComputeCurrentLightValues(float energyLvl, LampModeParametrs prevMode, LampModeParametrs currentMode, float changingState)
	{
		//computing current values of player's lamp, affecting radius by current energy level
		_currentViewRadius = energyLvl * Mathf.Lerp(prevMode.viewRadius, currentMode.viewRadius, changingState);
		_currentSpotLightRadius = energyLvl * Mathf.Lerp(prevMode.spotLightRadius, currentMode.spotLightRadius, changingState);
		_currentViewAngle = Mathf.Lerp( prevMode.viewAngle, currentMode.viewAngle, changingState);
		_currentSpotLightAngle = Mathf.Lerp( prevMode.spotLightAngle, currentMode.spotLightAngle, changingState);
		_currentIntensity = Mathf.Lerp(prevMode.intensity, currentMode.intensity, changingState) * CurrentIntensityMult;
		_currentLightHeight = Mathf.Lerp(prevMode.lightHeight, currentMode.lightHeight, changingState);
		_currentLightColor = Color.Lerp(prevMode.lightColor, currentMode.lightColor, changingState);
		_currentCoordinateY = Mathf.Lerp(prevMode.coordinateY, currentMode.coordinateY, changingState);
	}

    public void SetLightMode(int newMode, int prevMode, float changingState)
	{
		СurrentMode = newMode;
		_prevMode = prevMode;
		_changingState = changingState;
	}
    
    private void UpdateVisionCollider()
    {
	    float range = _currentViewRadius;
	    float angle = _currentViewAngle;
	    angle = DegreeToRadian(angle);
	    
	    Vector2[] points = new Vector2[3];
	    points[0] = Vector2.zero;
	    points[1] = new Vector2(range * Mathf.Tan(angle / 2), range);
	    points[2] = new Vector2(-1 * range * Mathf.Tan(angle / 2), range);
	    VisionCollider.points = points;
    }
	
	private void DrawSpotLight(float radius, float angel, float intensity, float height, Color color)
	{
		if (_spotLight == null)
			return;
		_spotLight.range = radius;
		_spotLight.spotAngle = angel;
		_spotLight.intensity = intensity;
		_spotLight.transform.localPosition = new Vector3(0, 0, height);
		_spotLight.color = color;
	}
	
	private List<Transform> FindVisibleEnemies(float viewRadius, float viewAngle)
	{
        
		List<Transform> visibleEnemies = new List<Transform>();
		Collider2D[] enemiesInViewRadius = Physics2D.OverlapCircleAll(transform.position, viewRadius, _enemyMask); //find all enemies in our view radius
        
		foreach (var enemyInView in enemiesInViewRadius)
		{
			Transform enemy = enemyInView.transform;
			Vector2 dirToEnemy = (enemy.position - transform.position).normalized; //find direction to the enemy
			if (Vector2.Angle(transform.up, dirToEnemy) < viewAngle / 2) //check if it is in our view angle
			{
				float dstToEnemy = Vector2.Distance(transform.position, enemy.position); //find distance to the enemy
				if (!Physics2D.Raycast(transform.position, dirToEnemy, dstToEnemy, _obstacleMask)) //check is it is not covered by an obstacle
				{
					visibleEnemies.Add(enemy);
				}
			}
		}
		return visibleEnemies;
	}
    
    private void DrawFieldOfView(float viewRadius, float viewAngle) //drawing the mesh representing field of view
    {
	    _visionArea.LightRadius = viewRadius;
	    _visionArea.RangeAngle = viewAngle;
    }
	
	private float DegreeToRadian(double angle)
	{
		return (float)(Math.PI * angle / 180.0);
	}
}
