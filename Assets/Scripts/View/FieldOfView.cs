using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


public class FieldOfView : MonoBehaviour
{
	[SerializeField]
	private bool _isMain; //true for main light and false for back light
	
	[Header("")]
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
    [HideInInspector]
	public Mesh _viewMesh; //Vision mesh
	
	private float _currentChangingTime;
	private Energy _energy;

	private List<Lamp.LampMode> _lampModes;
	
	private int _prevMode;
	
	private float _changingState = 1; //[0,1] shows how close mode to it's final state; 0 - start to change mode, 1 - not changing


	public int СurrentMode { private set; get; }

	private EnemyManager _enemyManager;

	//light values
    [HideInInspector] public float _currentViewRadius;
	[HideInInspector] public float _currentSpotLightRadius;
	[HideInInspector] public float _currentViewAngle;
	[HideInInspector] public float _currentSpotLightAngle;
	[HideInInspector] public float _currentIntensity;
	[HideInInspector] public float _currentIntensityMult = 1;
	[HideInInspector] public float _currentLightHeight;
	[HideInInspector] public float _currentCoordinateY;
	[HideInInspector] public Color _currentLightColor;

	private PolygonCollider2D _visionCollider;
	
	private void Start()
	{
		_spotLight = GetComponentInChildren<Light>();
		_viewMeshFilter = GetComponent<MeshFilter>();
		_viewMesh = new Mesh {name = "View Mesh"};
		_viewMeshFilter.mesh = _viewMesh;
		_energy =  GetComponentInParent<Energy>();        

		_enemyManager = EnemyManager.Instance;
		_lampModes = Player.Instance.transform.GetChild(0).GetComponent<Lamp>()._lampModes;

		_visionCollider = GetComponent<PolygonCollider2D>();
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
	}

    private void UpdateEnemiesVisibleEnemies()
    {
        Debug.Log("UpdateVisibleEnemies");
        _enemyManager.visibleEnemiesList = FindVisibleEnemies(_currentViewRadius, _currentViewAngle);
        Debug.Log("Visible enemies - " + _enemyManager.visibleEnemiesList.Count);
    }

    private void ComputeCurrentLightValues(float energyLvl, LampModeParametrs prevMode, LampModeParametrs currentMode, float changingState)
	{
		//computing current values of player's lamp, affecting radius by current energy level
		_currentViewRadius = energyLvl * Mathf.Lerp(prevMode.viewRadius, currentMode.viewRadius, changingState);
		_currentSpotLightRadius = energyLvl * Mathf.Lerp(prevMode.spotLightRadius, currentMode.spotLightRadius, changingState);
		_currentViewAngle = Mathf.Lerp( prevMode.viewAngle, currentMode.viewAngle, changingState);
		_currentSpotLightAngle = Mathf.Lerp( prevMode.spotLightAngle, currentMode.spotLightAngle, changingState);
		_currentIntensity = Mathf.Lerp(prevMode.intensity, currentMode.intensity, changingState) * _currentIntensityMult;
		_currentLightHeight = Mathf.Lerp(prevMode.lightHeight, currentMode.lightHeight, changingState);
		_currentLightColor = Color.Lerp(prevMode.lightColor, currentMode.lightColor, changingState);
		_currentCoordinateY = Mathf.Lerp(prevMode.coordinateY, currentMode.coordinateY, changingState);
		
		if(_updatingVisColHasStarted)//TODO: исправить костыль
			UpdateVisionCollider();
	}

    public void SetLightMode(int newMode, int prevMode, float changingState)
	{
		СurrentMode = newMode;
		_prevMode = prevMode;
		_changingState = changingState;
	}

    private bool _updatingVisColHasStarted;
    public void StartUpdatingVisCol()
    {
	    _updatingVisColHasStarted = true;
    }
    
    private void UpdateVisionCollider()
    {
	    float range = _currentViewRadius + _edgeOffset;
	    float angle = _currentViewAngle;
	    angle = DegreeToRadian(angle);
	    
	    Vector2[] points = new Vector2[3];
	    points[0] = Vector2.zero;
	    points[1] = new Vector2(range * Mathf.Tan(angle / 2), range);
	    points[2] = new Vector2(-1 * range * Mathf.Tan(angle / 2), range);
	    _visionCollider.points = points;
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

    public float SkipRadius = 0.0f;

    [HideInInspector]
    public Vector3[] vertices { get; private set; }
    [HideInInspector]
    public int[] triangles { get; private set; }
    [HideInInspector]
    public Color[] colors { get; private set; }
    private void DrawFieldOfView(float viewRadius, float viewAngle) //drawing the mesh representing field of view
	{
		List<Vector3> viewPoints = new List<Vector3>();
		GetViewPoints(viewPoints, -transform.eulerAngles.z, viewRadius, viewAngle); //get view points
		
		
		int vertexCount = viewPoints.Count; //number of vertices for drawing mesh
		vertices = new Vector3[vertexCount * 2]; //all vertex 
		triangles = new int[(vertexCount-1)*6]; //numbers of vertex for each triangle in the mash in one row
	    colors = new Color[vertexCount * 2];                
		for (int i = 0; i < vertexCount - 1; i++)
        {
            
			vertices[i + vertexCount] = transform.InverseTransformPoint(viewPoints[i]);
			vertices[i] = SkipRadius * vertices[i + vertexCount].normalized;
            var c = Math.Min(1, Math.Min(i, vertexCount - i - 2) / _meshResolution / 20);
            colors[i] = new Color(0, 0, 0, Mathf.Pow(1 -c, 2));
            colors[i + vertexCount] = new Color(0, 0,0 , vertices[i + vertexCount].magnitude / viewRadius);

            if (i < vertexCount - 2)
			{
				triangles [i * 6 + 0] = i; 
				triangles [i * 6 + 1] = i + vertexCount;
				triangles [i * 6 + 2] = i + vertexCount + 1;
            	triangles [i * 6 + 3] = i;
            	triangles [i * 6 + 4] = i + vertexCount + 1;
            	triangles [i * 6 + 5] = i + 1;
            }
		}
	
		_viewMesh.Clear();
		_viewMesh.vertices = vertices;
		_viewMesh.triangles = triangles;
        _viewMesh.colors = colors;
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
	
	private float DegreeToRadian(double angle)
	{
		return (float)(Math.PI * angle / 180.0);
	}
}
