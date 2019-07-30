using System.Collections;
using System.Collections.Generic;
using TriangleNet;
using TriangleNet.Voronoi;
using UnityEngine;

[ExecuteInEditMode]
public class MapVisualisator : MonoBehaviour
{
    [SerializeField] private int _tilesCount;
    [SerializeField] private int _mapWidth;
    [SerializeField] private int _mapHeight;
    
    [Header("Visualisation")]
    [SerializeField] private float _pointsRadius;
    [SerializeField] private bool _showDelaune;
    [SerializeField] private bool _showVoronoi;
    [SerializeField] private bool _showVoronoiVertices;
    
//    private List<Vector2> _tilesPoints = new List<Vector2>();
//    private TriangleNetMesh _triangleMesh;
//    private VoronoiBase _voronoiDiagram;

    private MapNetGenerator _mapNetGenerator;
    
    void Start()
    {
        _mapNetGenerator = new MapNetGenerator(_tilesCount, _mapHeight, _mapWidth);
    }
    
    private void OnDrawGizmos() //visualisation
    {
        //show map bounds
        Gizmos.color = Color.green;
        Gizmos.DrawLine(new Vector2(-_mapWidth / 2, _mapHeight / 2), new Vector2(_mapWidth / 2, _mapHeight / 2));
        Gizmos.DrawLine(new Vector2(-_mapWidth / 2, -_mapHeight / 2), new Vector2(_mapWidth / 2, -_mapHeight / 2));
        Gizmos.DrawLine(new Vector2(-_mapWidth / 2, _mapHeight / 2), new Vector2(-_mapWidth / 2, -_mapHeight / 2));
        Gizmos.DrawLine(new Vector2(_mapWidth / 2, _mapHeight / 2), new Vector2(_mapWidth / 2, -_mapHeight / 2));

        if (_mapNetGenerator == null)
            return;
        
        //show tiles positions
        Gizmos.color = Color.red;
        foreach (var point in _mapNetGenerator.TilesPoints)
        {
            Gizmos.DrawSphere(point, _pointsRadius);
        }
        
        //show triangle map
        if (_showDelaune)
        {
            Gizmos.color = Color.magenta;
            foreach (var edge in _mapNetGenerator.TriangleMesh.Edges)
            {
                var edgeStart = new Vector2(_mapNetGenerator.TriangleMesh.vertices[edge.P0].X, _mapNetGenerator.TriangleMesh.vertices[edge.P0].Y); 
                var edgeFinish = new Vector2(_mapNetGenerator.TriangleMesh.vertices[edge.P1].X, _mapNetGenerator.TriangleMesh.vertices[edge.P1].Y); 
                Gizmos.DrawLine(edgeStart, edgeFinish);
            }
        }
        
        //show Voronoi diagram
        if (_showVoronoi)
        {
            Gizmos.color = Color.red;
            foreach (var edge in _mapNetGenerator.VoronoiDiagram.Edges)
            {
                var edgeStart = new Vector2(_mapNetGenerator.VoronoiDiagram.Vertices[edge.P0].X, _mapNetGenerator.VoronoiDiagram.Vertices[edge.P0].Y); 
                var edgeFinish = new Vector2(_mapNetGenerator.VoronoiDiagram.Vertices[edge.P1].X, _mapNetGenerator.VoronoiDiagram.Vertices[edge.P1].Y); 
                Gizmos.DrawLine(edgeStart, edgeFinish);
            }
        }

        if (_showVoronoiVertices)
        {
            Gizmos.color = Color.cyan;
            foreach (var vertex in _mapNetGenerator.VoronoiDiagram.Vertices)
            {
                Gizmos.DrawSphere(new Vector2(vertex.X, vertex.Y), _pointsRadius);
            }
        }
    }
}
