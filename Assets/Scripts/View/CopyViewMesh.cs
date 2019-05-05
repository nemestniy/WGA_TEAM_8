using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyViewMesh : MonoBehaviour
{
    private MeshFilter _viewMeshFilter;
    private Mesh _viewMesh;

    public int DegResolution = 1;
    public float MaskingDistanceDeg = 5;

    private float _innerRingDistance = 0.05f;
    private float _midRingRelativeDistance = 0.75f;
     

    // Start is called before the first frame update
    void Start()
    {
        _viewMeshFilter = GetComponent<MeshFilter>();
        _viewMesh = new Mesh { name = "View Mesh" };
        _viewMeshFilter.mesh = _viewMesh;
        GetComponent<MeshRenderer>().sortingLayerName = "PostProcess";

       
        


    }

    // Update is called once per frame
    void Update()
    {


        //_viewMesh.Clear();
        //int vertexCount = 360 * DegResolution * 3;
        //Vector3[] vertices = new Vector3[vertexCount];
        //Vector3[] normals = new Vector3[vertexCount];
        ////inner ring
        //for (float angle = 0, i = 0; angle < 360; angle += 1.0f / DegResolution, i++)
        //{
        //    vertices[(int)i] = new Vector3(Mathf.Sin(Mathf.Deg2Rad * angle), Mathf.Cos(Mathf.Deg2Rad * angle));
        //}
        //for (float angle = 0, i = 0; angle < 360; angle += 1.0f / DegResolution, i++)
        //{
        //    vertices[(int)i] = new Vector3(Mathf.Sin(Mathf.Deg2Rad * angle), Mathf.Cos(Mathf.Deg2Rad * angle));
        //}
        var parent = GetComponentInParent<FieldOfView>();
        _viewMesh.vertices = parent.vertices;
        _viewMesh.triangles = parent.triangles;
        _viewMesh.colors = parent.colors;
        _viewMeshFilter.mesh = _viewMesh;
    }
}
