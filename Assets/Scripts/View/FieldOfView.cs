using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


public class FieldOfView : MonoBehaviour
{
	[SerializeField]
	private List<LampMode> _lightModes;

	[Header("")]
	[SerializeField]
	private float _changingDuration = 1;
	[SerializeField]
	private float _meshResolution = 4; //how many rays will be casted per 1 degree
	[SerializeField]
	private int _edgeResolveIterations = 10; //how many steps we spend to find an edge of an obstacle
	[FormerlySerializedAs("_shadowsCornersIsVisible")] [SerializeField] 
	private bool _shadowsCornersAreVisible = true;
	[SerializeField]
	private float _edgeDistanceThreshold = 0; //what distance between 2 hit points will be considered as hitting 2 different objects
	[SerializeField]
	private float _edgeOffset = 0.5f;
	
	[Header("Masks:")]
	[SerializeField]
	private LayerMask _enemyMask;
	[SerializeField]
	private LayerMask _obstacleMask;
	
	private Light _spotLight;
	private MeshFilter _viewMeshFilter;
	private Mesh _viewMesh; //Vision mesh
	private bool _isModeChanging;
	private float _currentChangingTime;
	private Energy _energy;
	private const int DegInCircle = 360;

	private int _currentMode;
	private int _prevMode;
	private float _changingState;

	[HideInInspector] public float _currentViewRadius;
	[HideInInspector] public float _currentSpotLightRadius;
	[HideInInspector] public float _currentViewAngle;
	[HideInInspector] public float _currentSpotLightAngle;
	[HideInInspector] public float _currentIntensity;
	[HideInInspector] public float _currentLightHeight;
	[HideInInspector] public float _currentCoordinateY;
	[HideInInspector] public Color _currentLightColor;

	private void Start()
	{
		_spotLight = GetComponentInChildren<Light>();
		_viewMeshFilter = GetComponent<MeshFilter>();
		_viewMesh = new Mesh {name = "View Mesh"};
		_viewMeshFilter.mesh = _viewMesh;
		_energy =  GetComponentInParent<Energy>();
	}

	
	private void LateUpdate()
	{
		_changingState = 1; //[0,1] shows how close mode to it's final state; 0 - start to change mode, 1 - not changing
		if (_isModeChanging)
		{
			if (_currentChangingTime < _changingDuration)
			{
				_currentChangingTime += Time.deltaTime;
				_changingState = _currentChangingTime / _changingDuration;
			}
			else
			{
				_currentChangingTime = 0;
				_isModeChanging = false;
			}
		}
	
		//computing current values of player's lamp, affecting radius by current energy level
		_currentViewRadius = _energy.CurrentEnergy * 0.01f * Mathf.Lerp(_lightModes[_prevMode].viewRadius, _lightModes[_currentMode].viewRadius, _changingState);
		_currentSpotLightRadius = _energy.CurrentEnergy * 0.01f * Mathf.Lerp(_lightModes[_prevMode].spotLightRadius, _lightModes[_currentMode].spotLightRadius, _changingState);
		_currentViewAngle = Mathf.Lerp( _lightModes[_prevMode].viewAngle, _lightModes[_currentMode].viewAngle, _changingState);
		_currentSpotLightAngle = Mathf.Lerp( _lightModes[_prevMode].spotLightAngle, _lightModes[_currentMode].spotLightAngle, _changingState);
		_currentIntensity = Mathf.Lerp(_lightModes[_prevMode].intensity, _lightModes[_currentMode].intensity, _changingState);
		_currentLightHeight = Mathf.Lerp(_lightModes[_prevMode].lightHeight, _lightModes[_currentMode].lightHeight, _changingState);
		_currentLightColor = Color.Lerp(_lightModes[_prevMode].lightColor, _lightModes[_currentMode].lightColor, _changingState);

		_currentCoordinateY = Mathf.Lerp(_lightModes[_prevMode].coordinateY, _lightModes[_currentMode].coordinateY, _changingState);
		
		var position = transform.localPosition;
		transform.localPosition = new Vector3(position.x,_currentCoordinateY ,position.z);
		
		DrawFieldOfView(_currentViewRadius, _currentViewAngle);
		DrawSpotLight(_currentSpotLightRadius, _currentSpotLightAngle, _currentIntensity, _currentLightHeight, _currentLightColor);
	
		//this means that the light in combat newMode
		if (_currentMode == 1 && _changingState == 1)
		{
			List<Transform> visibleEnemies = FindVisibleEnemies(_currentViewRadius, _currentViewAngle);
			foreach (var enemy in visibleEnemies)
			{
//                enemy.GetComponent<Enemy>().state = Enemy.States.Escaping;
				EnemyManager.Instance.OnEnemyOnLight(enemy.GetComponent<Enemy>());
			}
		}
	}

	private float timePast = 0;
	private void FixedUpdate()
	{
		//light is not changing now
		if (_changingState == 1)
		{
			if (timePast > _lightModes[_currentMode].spendEnergyDelay)
			{
				_energy.TakeAwayEnergy(_lightModes[_currentMode].energyCost);
			}
			else
			{
				timePast += Time.deltaTime;
			}
		}
		else
		{
			timePast = 0;
		}
	}

	public void SetLightMode(int newMode)
	{
		if (!_isModeChanging && _currentMode != newMode) //change the light to the combat newMode
		{
			_isModeChanging = true;
			_prevMode = _currentMode;
			_currentMode = newMode;
		}
	}

	private void DrawSpotLight(float radius, float angel, float intensity, float height, Color color)
	{
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
		List<Vector3> viewPoints = new List<Vector3>();
		GetViewPoints(viewPoints, -transform.eulerAngles.z, viewRadius, viewAngle); //get view points
		
		
		int vertexCount = viewPoints.Count + 1; //number of vertices for drawing mesh
		Vector3[] vertices = new Vector3[vertexCount]; //all vertex 
		int[] triangles = new int[(vertexCount-2)*3]; //numbers of vertex for each triangle in the mash in one row
	
		vertices[0] = Vector2.zero; //since the mash is players child
		for (int i = 0; i < vertexCount - 1; i++)
		{ 
			vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);

			if (i < vertexCount - 2)
			{
				triangles [i * 3] = 0; 
				triangles [i * 3 + 1] = i + 1;
				triangles [i * 3 + 2] = i + 2;
			}
		}
	
		_viewMesh.Clear();
		_viewMesh.vertices = vertices;
		_viewMesh.triangles = triangles;
		_viewMesh.RecalculateNormals();
	}

	private void GetViewPoints(List<Vector3> viewPoints, float directionAngle, float viewRadius, float viewAngle) //add in list all view point
	{
		
		ViewCastInfo oldViewCast = new ViewCastInfo();

		int stepCount = Mathf.RoundToInt(viewAngle * _meshResolution);
		float stepAngleSize = viewAngle / stepCount; //how many degrees will by in each step
		for (int i = 0; i <= stepCount; i++)
		{
			float angle = directionAngle - viewAngle / 2 + stepAngleSize * i; //defining current angle
			ViewCastInfo newViewCast = ViewCast(angle, viewRadius);
			if (i > 0
			    && (oldViewCast.hit != newViewCast.hit //if previous ray hits an obstacle and this ray doesn't (or vice versa)...
			        || Mathf.Abs(oldViewCast.dst - newViewCast.dst) > _edgeDistanceThreshold)) //...or _edgeDistanceThreshold is exceeded find edge point
			{
				EdgeInfo edge = FindEdge(oldViewCast, newViewCast, viewRadius);
				if (edge.pointA != Vector3.zero) //add both points if their values is not default
				{
					viewPoints.Add(edge.pointA);
				}
				if (edge.pointA != Vector3.zero)
				{
					viewPoints.Add(edge.pointA);
				}
			}
	
			viewPoints.Add(newViewCast.point); //defining list of all points of vision edge
			oldViewCast = newViewCast; //saving previous ViewCastIfo
		}
	}

	private EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast, float viewRadius)
	{
		float minAngle = minViewCast.angle;
		float maxAngle = maxViewCast.angle;
		Vector2 minPoint = Vector2.zero;
		Vector2 maxPoint = Vector2.zero;

		for (int i = 0; i < _edgeResolveIterations; i++)
		{
			float angle = (minAngle + maxAngle) / 2; //find angle between min and max
			ViewCastInfo newViewCast = ViewCast(angle, viewRadius); //cast ray with new angle
			//reconfigure min or max angles
			if (newViewCast.hit == minViewCast.hit && 
			    !(Mathf.Abs(minViewCast.dst - newViewCast.dst) > _edgeDistanceThreshold))
			{
				minAngle = angle;
				minPoint = newViewCast.point;
			}
			else
			{
				maxAngle = angle;
				maxPoint = newViewCast.point;
			}
		}
	
		return new EdgeInfo(minPoint, maxPoint);
	}


	private ViewCastInfo ViewCast(float globalAngle, float viewRadius) //cast ray and collect info about hit
	{
		Vector2 dir = DirFromAngle(globalAngle, true);
		if (_shadowsCornersAreVisible)
		{
			RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, viewRadius, _obstacleMask); //cast ray

			if (hit) //if ray hit an obstacle
			{
				return new ViewCastInfo (true, hit.point + dir * _edgeOffset, hit.distance, globalAngle);
			} 
			else //if ray did not hit an obstacle
			{
				return new ViewCastInfo (false, transform.position + (Vector3)dir * (viewRadius + _edgeOffset), viewRadius, globalAngle);
			}
		}
		else
		{
			RaycastHit2D[] hits = Physics2D.LinecastAll(transform.position, transform.position + (Vector3)dir * viewRadius, _obstacleMask); //cast ray
	
			foreach (var hit in hits)
			{
				Debug.DrawLine(hit.point, hit.point + Vector2.up * 0.017f);
			}

			if (hits.Length != 0) //if ray hit an obstacle
			{
				if (Physics2D.OverlapCircle(hits[0].point + dir * _edgeOffset, 0, _obstacleMask))
				{
					return new ViewCastInfo (true, hits[0].point + dir * _edgeOffset, hits[0].distance, globalAngle);
				}
				else if(hits.Length > 1)
				{
					return new ViewCastInfo (true, hits[1].point + dir * _edgeOffset, hits[0].distance, globalAngle);
				}
			}
			return new ViewCastInfo (false, transform.position + (Vector3)dir * (viewRadius + _edgeOffset), viewRadius, globalAngle);
		}
	}

	public Vector2 DirFromAngle(float angleInDegrees, bool angleIsGlobal) //to get looking directory from angle
	{
		if (!angleIsGlobal)
		{
			angleInDegrees -= transform.eulerAngles.z;
		}
		return new Vector2(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));	//using Sin for X and Cos for Y since 0 deg is on top in Unity, not on the right
	}

	private int FromLayerToNum(LayerMask layerMask) //converting LayerMask object to number of layer
	{
		return (int)(Math.Log(layerMask.value) / Math.Log(2)); //layerMask.value returns number of layer in power of 2 (11^2=2048) so we use log2 to do vice versa
	}

	private struct ViewCastInfo //information about casting a view ray
	{
		public bool hit;
		public Vector3 point;
		public float dst;
		public float angle;

		public ViewCastInfo(bool hit, Vector2 point, float dst, float angle)
		{
			this.hit = hit;
			this.point = point;
			this.dst = dst;
			this.angle = angle;
		}
	}

	private struct EdgeInfo
	{
		public Vector3 pointA;
		public Vector3 pointB;

		public EdgeInfo(Vector3 pointA, Vector3 pointB)
		{
			this.pointA = pointA;
			this.pointB = pointB;
		}
	}
	
	[Serializable]
	public struct LampMode
	{
		public float viewRadius;
		public float spotLightRadius;
		[Range(0, DegInCircle)]
		public float viewAngle;
		[Range(0, DegInCircle)]
		public float spotLightAngle;
		public float intensity;
		public float lightHeight;
		public float coordinateY;
		public Color lightColor;
		public float energyCost;
		public float spendEnergyDelay;
	}
}
