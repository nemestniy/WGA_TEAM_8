using System;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
	private const int DEG_IN_CIRCLE = 360;

	[SerializeField] 
	private bool _staticCloseVision;
	[SerializeField]
	public float _closeViewRadius;
	[SerializeField]
	public float _farViewRadius;
	[SerializeField]
	[Range(0, DEG_IN_CIRCLE)]
	public float _viewAngle;
	[SerializeField]
	private LayerMask _enemyMask;
	[SerializeField]
	private LayerMask _obstacleMask;
	private int _obstacleMaskNum;

	[HideInInspector]
	public List<Transform> _visibleEnemies;

	[SerializeField]
	[Range(0,1)]
	private float _findingVisibleResolution;
	[SerializeField]
	private float _meshResolution; //how many rays will be casted per 1 degree

	[SerializeField]
	private int _edgeResolveIterations; //how many steps we spend to find an edge of an obstacle
	[SerializeField]
	private float _edgeDistanceThreshold; //what distance between 2 hit points will be considered as hitting 2 different objects
	
	[SerializeField]
	private MeshFilter _viewMeshFilter;
	private Mesh _viewMesh; //Vision mesh

	[SerializeField] 
	private LayerMask _currentCheckedObstacleMask;

	private int _visibleLayerNum;
	private ContactFilter2D _contactFilter2D;
	
//	[SerializeField]
//	private MeshFilter _obstacleMaskMeshFilter;
//	private Mesh _obstacleMaskMesh; //Obstacles vision mesh

	private HashSet<GameObject> _visibleObstacles; //contains all obstacles which are visible in this frame

	private void Start()
	{
		_viewMesh = new Mesh {name = "View Mesh"};
		_viewMeshFilter.mesh = _viewMesh;
		_visibleLayerNum = FromLayerToNum(_currentCheckedObstacleMask);
		_obstacleMaskNum = FromLayerToNum(_obstacleMask);
		_contactFilter2D.SetLayerMask(_obstacleMask);
//		_obstacleMaskMesh = new Mesh{name = "Obstacles Mask Mesh"};
//		_obstacleMaskMeshFilter.mesh = _obstacleMaskMesh;

		_visibleObstacles = new HashSet<GameObject>();
	}

	private void LateUpdate()
	{
		DrawFieldOfView();
//		DrawObstaclesViewMask();
	}

	public void FindVisibleEnemies()
	{
		_visibleEnemies.Clear();
		Collider2D[] enemiesInFarViewRadius = Physics2D.OverlapCircleAll(transform.position, _farViewRadius, _enemyMask); //find all enemies in our far view radius
		foreach (var enemyInView in enemiesInFarViewRadius)
		{
			Transform enemy = enemyInView.transform;
			Vector2 dirToEnemy = (enemy.position - transform.position).normalized; //find direction to the enemy
			if (Vector2.Angle(transform.up, dirToEnemy) < _viewAngle / 2) //check if it is in our view angle
			{
				float dstToEnemy = Vector2.Distance(transform.position, enemy.position); //find distance to the enemy
				if (!Physics2D.Raycast(transform.position, dirToEnemy, dstToEnemy, _obstacleMask)) //check is it is not covered by an obstacle
				{
					_visibleEnemies.Add(enemy);
				}
			}
		}
		
		Collider2D[] enemiesInCloseViewRadius = Physics2D.OverlapCircleAll(transform.position, _closeViewRadius, _enemyMask); //find all enemies in our close view radius
		foreach (var enemyInView in enemiesInCloseViewRadius)
		{
			Transform enemy = enemyInView.transform;
			Vector2 dirToEnemy = (enemy.position - transform.position).normalized; //find direction to the enemy
			if (Vector2.Angle(-transform.up, dirToEnemy) < (DEG_IN_CIRCLE -_viewAngle) / 2) //check if it is in our view angle
			{
				float dstToEnemy = Vector2.Distance(transform.position, enemy.position); //find distance to the enemy
				if (!Physics2D.Raycast(transform.position, dirToEnemy, dstToEnemy, _obstacleMask)) //check is it is not covered by an obstacle
				{
					_visibleEnemies.Add(enemy);
				}
			}
		}
	}

	private void DrawFieldOfView() //drawing the mesh representing field of view
	{
		List<Vector3> viewPoints = new List<Vector3>();
		GetFarViewPoints(viewPoints);
		GetCloseViewPoints(viewPoints);

		int vertexCount = viewPoints.Count + 1; //number of vertices for drawing mesh
		Vector3[] vertices = new Vector3[vertexCount]; //all vertex 
		int[] triangles = new int[(vertexCount-2)*3]; //numbers of vertex for each triangle in the mash in one row
		
		vertices[0] = Vector2.zero; //since the mash is players child
		for (int i = 0; i < vertexCount - 1; i++)
		{
			//by adding forward * _maskCutawayDst we making the vision longer, which creates visible edges on obstacles 
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

//		foreach (var visibleObstacle in _visibleObstacles) //make all seen obstacles visible
//		{
//			visibleObstacle.BecomeVisible();
//		}
//		_visibleObstacles.Clear();
	}

	private void GetFarViewPoints(List<Vector3> viewPoints) //add in list all view point from vision cone
	{
		FindVisibleObstacles();

		foreach (var visibleObstacle in _visibleObstacles)
		{
			visibleObstacle.layer = _visibleLayerNum;
		}
		
		ViewCastInfo oldViewCast = new ViewCastInfo();
		int stepCount = Mathf.RoundToInt(_viewAngle * _meshResolution);
		float stepAngleSize = _viewAngle / stepCount; //how many degrees will by in each step
		for (int i = 0; i <= stepCount; i++)
		{
			float angle = -transform.eulerAngles.z - _viewAngle / 2 + stepAngleSize * i; //defining current angle
			ViewCastInfo newViewCast = ViewCast(angle, _farViewRadius);

			if (i > 0
			    && (oldViewCast.hit != newViewCast.hit //if previous ray hits an obstacle and this ray doesn't (or vice versa)...
			        || Mathf.Abs(oldViewCast.dst - newViewCast.dst) > _edgeDistanceThreshold)) //...or _edgeDistanceThreshold is exceeded find edge point
			{
				EdgeInfo edge = FindEdge(oldViewCast, newViewCast);
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
		foreach (var visibleObstacle in _visibleObstacles)
		{
			visibleObstacle.layer = _obstacleMaskNum;
		}
		_visibleObstacles.Clear();
	}
	
	private void GetCloseViewPoints(List<Vector3> viewPoints) //add in list all view point from close vision
	{
		if (!_staticCloseVision) //if close vision is interactive
		{
			ViewCastInfo oldViewCast = new ViewCastInfo();
		
			int stepCount = Mathf.RoundToInt((DEG_IN_CIRCLE -_viewAngle) * _meshResolution);
			float stepAngleSize = (DEG_IN_CIRCLE -_viewAngle) / stepCount; //how many degrees will by in each step
			for (int i = 0; i <= stepCount; i++)
			{
				float angle = -transform.eulerAngles.z + _viewAngle / 2 + stepAngleSize * i; //defining current angle
				ViewCastInfo newViewCast = ViewCast(angle, _closeViewRadius);

				if (i > 0
				    && (oldViewCast.hit != newViewCast.hit //if previous ray hits an obstacle and this ray doesn't (or vice versa)...
				        || Mathf.Abs(oldViewCast.dst - newViewCast.dst) > _edgeDistanceThreshold)) //...or _edgeDistanceThreshold is exceeded find edge point
				{
					EdgeInfo edge = FindEdge(oldViewCast, newViewCast);
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
		else //if close vision is static
		{
			int stepCount = Mathf.RoundToInt((DEG_IN_CIRCLE -_viewAngle) * _meshResolution);
			float stepAngleSize = (DEG_IN_CIRCLE -_viewAngle) / stepCount; //how many degrees will by in each step
			for (int i = 0; i <= stepCount; i++)
			{
				float angle = -transform.eulerAngles.z + _viewAngle / 2 + stepAngleSize * i; //defining current angle
				Vector2 dir = DirFromAngle(angle, true);
				viewPoints.Add((Vector2)transform.position + dir * _closeViewRadius);
			}
		}
		
	}
	
//	private void DrawObstaclesViewMask() //drawing the mesh representing obstacles view mask
//	{
//		List<Vector3> obstaclesMaskPoints = new List<Vector3>();
//		GetFarMaskPoints(obstaclesMaskPoints);
//		GetCloseMaskPoints(obstaclesMaskPoints);
//		
//		int vertexCount = obstaclesMaskPoints.Count + 1; //number of vertices for drawing mesh
//		Vector3[] vertices = new Vector3[vertexCount]; //all vertex 
//		int[] triangles = new int[(vertexCount-2)*3]; //numbers of vertex for each triangle in the mash in one row
//		
//		vertices[0] = Vector2.zero; //since the mash is players child
//		for (int i = 0; i < vertexCount - 1; i++)
//		{
//			//by adding forward * _maskCutawayDst we making the vision longer, which creates visible edges on obstacles 
//			vertices[i + 1] = transform.InverseTransformPoint(obstaclesMaskPoints[i]);
//
//			if (i < vertexCount - 2)
//			{
//				triangles [i * 3] = 0; 
//				triangles [i * 3 + 1] = i + 1;
//				triangles [i * 3 + 2] = i + 2;
//			}
//		}
//		
//		_obstacleMaskMesh.Clear();
//		_obstacleMaskMesh.vertices = vertices;
//		_obstacleMaskMesh.triangles = triangles;
//		_obstacleMaskMesh.RecalculateNormals();
//	}

//	private void GetFarMaskPoints(List<Vector3> maskPoints)
//	{
//		int stepCount = Mathf.RoundToInt(_viewAngle * _meshResolution);
//		float stepAngleSize = _viewAngle / stepCount; //how many degrees will by in each step
//		for (int i = 0; i <= stepCount; i++)
//		{
//			float angle = -transform.eulerAngles.z - _viewAngle / 2 + stepAngleSize * i; //defining current angle
//			Vector2 dir = DirFromAngle(angle, true);
//			maskPoints.Add((Vector2)transform.position + dir * _farViewRadius);
//		}
//	}
//	
//	private void GetCloseMaskPoints(List<Vector3> maskPoints)
//	{
//		int stepCount = Mathf.RoundToInt((DEG_IN_CIRCLE -_viewAngle) * _meshResolution);
//		float stepAngleSize = (DEG_IN_CIRCLE -_viewAngle) / stepCount; //how many degrees will by in each step
//		for (int i = 0; i <= stepCount; i++)
//		{
//			float angle = -transform.eulerAngles.z + _viewAngle / 2 + stepAngleSize * i; //defining current angle
//			Vector2 dir = DirFromAngle(angle, true);
//			maskPoints.Add((Vector2)transform.position + dir * _closeViewRadius);
//		}
//	}
	

	private EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
	{
		float minAngle = minViewCast.angle;
		float maxAngle = maxViewCast.angle;
		Vector2 minPoint = Vector2.zero;
		Vector2 maxPoint = Vector2.zero;

		for (int i = 0; i < _edgeResolveIterations; i++)
		{
			float angle = (minAngle + maxAngle) / 2; //find angle between min and max
			ViewCastInfo newViewCast = ViewCast(angle, _farViewRadius); //cast ray with new angle
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

//	private ViewCastInfo ViewCast(float globalAngle, float viewRadius) //cast ray and collect info about hit
//	{
//		Vector2 dir = DirFromAngle(globalAngle, true);
//		RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, viewRadius, _obstacleMask); //cast ray
//		
//		if (hit) //if ray hit an obstacle
//		{
////			_visibleObstacles.Add(hit.collider.GetComponent<Obstacle>()); //adding all seen obstacles
//			return new ViewCastInfo (true, hit.point, hit.distance, globalAngle);
//		} 
//		else //if ray did not hit an obstacle
//		{
//			return new ViewCastInfo (false, transform.position + (Vector3)dir * viewRadius, viewRadius, globalAngle);
//		}
//	}


//	private ViewCastInfo ViewCast(float globalAngle, float viewRadius) //cast ray and collect info about hit
//	{
//		Vector2 dir = DirFromAngle(globalAngle, true);
//		RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, viewRadius, _obstacleMask); //cast ray from player position to vision border
//		
//		if (hit) //if ray hit an obstacle find the point on the opposite side of the obstacle
//		{
//			hit.collider.gameObject.layer = _visibleLayerNum;
//			RaycastHit2D reverseHit = Physics2D.Raycast(transform.position + (Vector3)dir * viewRadius, -dir, viewRadius, 1<<_visibleLayerNum); //cast ray from vision border to player position
////			if (reverseHit.collider != hit.collider && !reverseHit.collider.IsTouching(hit.collider))
////			{
////			}
//			hit.collider.gameObject.layer = _obstacleMaskNum;
//			
//			return new ViewCastInfo (true, reverseHit.point, viewRadius-reverseHit.distance, globalAngle);
//		} 
//		else //if ray did not hit an obstacle
//		{
//			return new ViewCastInfo (false, transform.position + (Vector3)dir * viewRadius, viewRadius, globalAngle);
//		}
//	}
	
	private ViewCastInfo ViewCast(float globalAngle, float viewRadius) //cast ray and collect info about hit
	{
		Vector2 dir = DirFromAngle(globalAngle, true);
		RaycastHit2D reverseHit = Physics2D.Raycast(transform.position + (Vector3)dir * viewRadius, -dir, viewRadius, 1<<_visibleLayerNum); //cast ray from player position to vision border
		
		if (reverseHit) //if ray hit an obstacle find the point on the opposite side of the obstacle
		{
			return new ViewCastInfo (true, reverseHit.point, reverseHit.distance, globalAngle);
		} 
		else //if ray did not hit an obstacle
		{
			return new ViewCastInfo (false, transform.position + (Vector3)dir * viewRadius, viewRadius, globalAngle);
		}
	}
	
	private void FindVisibleObstacles()
	{
		int stepCount = Mathf.RoundToInt(_viewAngle * _findingVisibleResolution);
		float stepAngleSize = _viewAngle / stepCount; //how many degrees will by in each step
		for (int i = 0; i <= stepCount; i++)
		{
			float angle = -transform.eulerAngles.z - _viewAngle / 2 + stepAngleSize * i; //defining current angle
			Vector2 dir = DirFromAngle(angle, true);
			RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, _farViewRadius, _obstacleMask);
			if (hit)
			{
				_visibleObstacles.Add(hit.collider.gameObject);
			}
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
}
