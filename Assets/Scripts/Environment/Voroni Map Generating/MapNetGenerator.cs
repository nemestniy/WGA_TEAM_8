using System;
using System.Collections.Generic;
using TriangleNet;
using TriangleNet.Geometry;
using TriangleNet.Meshing.Algorithm;
using TriangleNet.Voronoi;
using UnityEngine;
using Random = UnityEngine.Random;

public class MapNetGenerator
{
    public List<Vector2> TilesPoints = new List<Vector2>();
    public TriangleNetMesh TriangleMesh;
    public VoronoiBase VoronoiDiagram;
    
    public MapNetGenerator(int tilesCount, int mapWidth, int mapHeight)
    {
        TilesPoints = SetTilesPositions(tilesCount, mapWidth, mapHeight);
        
        //generate Delaunay triangulation
        TriangleMesh = (TriangleNetMesh)new Dwyer().Triangulate(ToTriangleNetVertices(TilesPoints), new Configuration());
        
        //generate Voronoi diagram
        VoronoiDiagram = new StandardVoronoi(TriangleMesh);
    }
    

    private List<Vector2> SetTilesPositions(int tilesCount, int mapWidth, int mapHeight)
    {
        List<Vector2> tilesPoints = new List<Vector2>();
        
        for (int i = 0; i < tilesCount; i++)
        {
            tilesPoints.Add(new Vector2(Random.Range(-mapWidth / 2, mapWidth / 2), Random.Range(-mapHeight / 2, mapHeight / 2)));
        }
        
        //add map corners
        tilesPoints.Add(new Vector2(mapWidth / 2, mapHeight / 2));
        tilesPoints.Add(new Vector2(mapWidth / 2, -mapHeight / 2));
        tilesPoints.Add(new Vector2(-mapWidth / 2, mapHeight / 2));
        tilesPoints.Add(new Vector2(-mapWidth / 2, -mapHeight / 2));

        return tilesPoints;
    }
    
    private List<Vertex> ToTriangleNetVertices(List<Vector2> points)
    {
        List<Vertex> vertices = new List<Vertex>();
        foreach (var vec in points)
        {
            vertices.Add(new Vertex(vec.x, vec.y));
        }
        return vertices;
    }
}
