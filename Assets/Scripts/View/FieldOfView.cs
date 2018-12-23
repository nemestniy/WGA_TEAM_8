﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
	private const int DEG_IN_CIRCLE = 360;
	
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

	[HideInInspector]
	public List<Transform> _visibleEnemies;

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
	private MeshFilter _obstacleMaskMeshFilter;
	private Mesh _obstacleMaskMesh; //Obstacles vision mesh

	private void Start()
	{
		_viewMesh = new Mesh {name = "View Mesh"};
		_viewMeshFilter.mesh = _viewMesh;
		
		_obstacleMaskMesh = new Mesh{name = "Obstacles Mask Mesh"};
		_obstacleMaskMeshFilter.mesh = _obstacleMaskMesh;
	}

	private void LateUpdate()
	{
		DrawFieldOfView();
		DrawObstaclesViewMask();
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
	}

	private void GetFarViewPoints(List<Vector3> viewPoints) //add in list all view point from vision cone
	{
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
	}
	
	private void GetCloseViewPoints(List<Vector3> viewPoints) //add in list all view point from close vision
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
	
	private void DrawObstaclesViewMask() //drawing the mesh representing obstacles view mask
	{
		List<Vector3> obstaclesMaskPoints = new List<Vector3>();
		GetFarMaskPoints(obstaclesMaskPoints);
		GetCloseMaskPoints(obstaclesMaskPoints);
		
		int vertexCount = obstaclesMaskPoints.Count + 1; //number of vertices for drawing mesh
		Vector3[] vertices = new Vector3[vertexCount]; //all vertex 
		int[] triangles = new int[(vertexCount-2)*3]; //numbers of vertex for each triangle in the mash in one row
		
		vertices[0] = Vector2.zero; //since the mash is players child
		for (int i = 0; i < vertexCount - 1; i++)
		{
			//by adding forward * _maskCutawayDst we making the vision longer, which creates visible edges on obstacles 
			vertices[i + 1] = transform.InverseTransformPoint(obstaclesMaskPoints[i]);

			if (i < vertexCount - 2)
			{
				triangles [i * 3] = 0; 
				triangles [i * 3 + 1] = i + 1;
				triangles [i * 3 + 2] = i + 2;
			}
		}
		
		_obstacleMaskMesh.Clear();
		_obstacleMaskMesh.vertices = vertices;
		_obstacleMaskMesh.triangles = triangles;
		_obstacleMaskMesh.RecalculateNormals();
	}

	private void GetFarMaskPoints(List<Vector3> maskPoints)
	{
		int stepCount = Mathf.RoundToInt(_viewAngle * _meshResolution);
		float stepAngleSize = _viewAngle / stepCount; //how many degrees will by in each step
		for (int i = 0; i <= stepCount; i++)
		{
			float angle = -transform.eulerAngles.z - _viewAngle / 2 + stepAngleSize * i; //defining current angle
			Vector2 dir = DirFromAngle(angle, true);
			maskPoints.Add((Vector2)transform.position + dir * _farViewRadius);
		}
	}
	
	private void GetCloseMaskPoints(List<Vector3> maskPoints)
	{
		int stepCount = Mathf.RoundToInt((DEG_IN_CIRCLE -_viewAngle) * _meshResolution);
		float stepAngleSize = (DEG_IN_CIRCLE -_viewAngle) / stepCount; //how many degrees will by in each step
		for (int i = 0; i <= stepCount; i++)
		{
			float angle = -transform.eulerAngles.z + _viewAngle / 2 + stepAngleSize * i; //defining current angle
			Vector2 dir = DirFromAngle(angle, true);
			maskPoints.Add((Vector2)transform.position + dir * _closeViewRadius);
		}
	}
	

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

	private ViewCastInfo ViewCast(float globalAngle, float viewRadius) //cast ray and collect info about hit
	{
		Vector2 dir = DirFromAngle(globalAngle, true);
		RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, viewRadius, _obstacleMask); //cast ray
		
		if (hit) //if ray hit an obstacle
		{
			return new ViewCastInfo (true, hit.point, hit.distance, globalAngle);
		} 
		else //if ray did not hit an obstacle
		{
			return new ViewCastInfo (false, transform.position + (Vector3)dir * viewRadius, viewRadius, globalAngle);
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
