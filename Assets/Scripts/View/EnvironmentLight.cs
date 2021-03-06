﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


public class EnvironmentLight : MonoBehaviour
{
	private const int DegInCircle = 360;

	[SerializeField]
	public float _viewRadius = 20;

	[SerializeField]
	private LayerMask _obstacleMask;

	[Header("Optimisation:")]
	[SerializeField]
	private float _meshResolution = 4; //how many rays will be casted per 1 degree
	[SerializeField]
	private int _edgeResolveIterations = 10; //how many steps we spend to find an edge of an obstacle
	[FormerlySerializedAs("_shadowsCornersIsVisible")] [SerializeField] 
	private bool _shadowsCornersAreVisible = true;
	[SerializeField]
	private float _edgeDistanceThreshold = 0; //what distance between 2 hit points will be considered as hitting 2 different objects

	[Header("")]
	[SerializeField]
	private float _edgeOffset = 0.5f;
	[SerializeField]
	private MeshFilter _viewMeshFilter;

	private Mesh _viewMesh; //Vision mesh

	private void Start()
	{
		_viewMesh = new Mesh {name = "View Mesh"};
		_viewMeshFilter.mesh = _viewMesh;
		DrawFieldOfView(_viewRadius, DegInCircle);
		Hexagon.OnWallsChange += RedrawView;
	}

	private void RedrawView()
	{
		DrawFieldOfView(_viewRadius, DegInCircle);
	}

	private void DrawFieldOfView(float farViewRadius, float viewAngle) //drawing the mesh representing field of view
	{
		List<Vector3> viewPoints = new List<Vector3>();
		GetViewPoints(viewPoints, -transform.eulerAngles.z, farViewRadius, viewAngle); //get far view points

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

	private void GetViewPoints(List<Vector3> viewPoints, float directionAngle, float viewRadius, float viewAngle) //add in list all view point from close vision
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

	private void OnDestroy()
	{
		Hexagon.OnWallsChange -= RedrawView;
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
