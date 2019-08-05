using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;


public class DungeonFiller : MonoBehaviour
{
    [SerializeField] private GameObject _wallSample;
    [SerializeField] private float _wallLengthCoef;
    [SerializeField] private int _tilesCount;
    [SerializeField] private int _mapWidth;
    [SerializeField] private int _mapHeight;
    [SerializeField] private int _pathLength;
    private Dictionary<int, List<UnorderedPair<int>>> _edgeLinkage;
    private HashSet<UnorderedPair<int>> _usedLinks;
    private HashSet<int> _alreadyUsed;
    private List<WallParameters> _walls = new List<WallParameters>();
    public MapNetGenerator MapNetGenerator { get; protected set; }

    private struct UnorderedPair<T>
    {
        public T Elem1, Elem2;

        public UnorderedPair(T elem1, T elem2)
        {
            Elem1 = elem1;
            Elem2 = elem2;
        }

        public bool Equals(UnorderedPair<T> other)
        {
            return (Elem1.Equals(other.Elem1) && Elem2.Equals(other.Elem2)) ||
                   (Elem1.Equals(other.Elem2) && Elem2.Equals(other.Elem1));
        }

        public override bool Equals(object obj)
        {
            return (obj is UnorderedPair<T>) && Equals((UnorderedPair<T>) obj);
        }
    }

    private IEnumerable<int> _availableNodes => _alreadyUsed.Where(v =>
        _edgeLinkage[v].Any(a => !_alreadyUsed.Contains(a.Elem1) || !_alreadyUsed.Contains(a.Elem2)));
    private List<UnorderedPair<int>> _links = new List<UnorderedPair<int>>();

    public Dictionary<int, int> NodeLevels = new Dictionary<int, int>();
    public int[] MainPath;

    void Start()
    {
        Random random = new Random(1234);

        MapNetGenerator = new MapNetGenerator(_tilesCount, _mapHeight, _mapWidth);
        _edgeLinkage = new Dictionary<int, List<UnorderedPair<int>>>();
        foreach (var edge in MapNetGenerator.TriangleMesh.Edges)
        {
            var p = new UnorderedPair<int>(edge.P0, edge.P1);
            if (!_edgeLinkage.ContainsKey(edge.P0)) _edgeLinkage[edge.P0] = new List<UnorderedPair<int>>();
            if (!_edgeLinkage.ContainsKey(edge.P1)) _edgeLinkage[edge.P1] = new List<UnorderedPair<int>>();
            _edgeLinkage[edge.P0].Add(p);
            _edgeLinkage[edge.P1].Add(p);
        }

        _usedLinks = new HashSet<UnorderedPair<int>>();

        int startIndex = random.Next(_edgeLinkage.Count - 1);
        int currentIndex = startIndex;
        NodeLevels = new Dictionary<int, int>();
        _alreadyUsed = new HashSet<int>();
        MainPath = new int[_pathLength];
        for (int i = 0; i < _pathLength; i++)
        {
            MainPath[i] = currentIndex;
            _alreadyUsed.Add(currentIndex);
            if (!_edgeLinkage[currentIndex].Any())
            {
                if (i == 0)
                {
                    startIndex = random.Next(_edgeLinkage.Count - 1);
                    currentIndex = startIndex;
                }
                else
                {
                    currentIndex = MainPath[i - 1];
                    i -= 2;
                }

                continue;
            }
            var tmp = _edgeLinkage[currentIndex].Where(a => !_usedLinks.Contains(a) && !(_alreadyUsed.Contains(a.Elem1) && _alreadyUsed.Contains(a.Elem2))).ToList();
            if (!tmp.Any())
            {
                currentIndex = MainPath[i - 1];
                i -= 2;
                continue;
            }
            int pi = random.Next(tmp.Count() - 1);
            currentIndex = tmp[pi].Elem1 == currentIndex ? tmp[pi].Elem2 : tmp[pi].Elem1;
            _usedLinks.Add(tmp[pi]);
        }
        _alreadyUsed.Clear();



        for (int i = 0; i < MainPath.Length; i++)
        {
            NodeLevels[MainPath[i]] = i;
            _alreadyUsed.Add(MainPath[i]);
            if (i > 0)
            {
                _links.Add(new UnorderedPair<int>(MainPath[i - 1], MainPath[i]));
            }
        }
        var an = _availableNodes.ToList();
        while ((an = _availableNodes.ToList()).Any())
        {
            
            int i = random.Next(an.Count() - 1);
            int oldIndex = an.ElementAt(i);

            var tmp = _edgeLinkage[oldIndex]
                .Where(a => !_alreadyUsed.Contains(a.Elem1) || !_alreadyUsed.Contains(a.Elem2)).ToList();
            int pathIndex = random.Next(tmp.Count() - 1);
            var v = tmp.ElementAt(pathIndex);
            int newIndex = (v.Elem1 == oldIndex ? v.Elem2 : v.Elem1);
            NodeLevels[newIndex] = NodeLevels[oldIndex] + 1;
            _alreadyUsed.Add(newIndex);
            _links.Add(new UnorderedPair<int>(oldIndex, newIndex));
        }


        SetWalls();
    }

    private void SetWalls()
    {
        foreach (var face in MapNetGenerator.VoronoiDiagram.Faces)//iterate through all faces of Voronoi diagramm
        {
            var firstEdge = face.Edge;
            if (firstEdge == null)
                continue;

            var edge = firstEdge;
            do //iterate through all edges of the face
            {
                if (Mathf.Abs(NodeLevels[edge.Face.ID] - NodeLevels[edge.Twin.Face.ID]) > 1)
                {
                    DrawWall(new Vector2(edge.Origin.X, edge.Origin.Y),
                        new Vector2(edge.Twin.Origin.X, edge.Twin.Origin.Y));
                }

                edge = edge.Next;
            } while (edge != null && edge != firstEdge);
        }
    }
    
    private struct WallParameters
    {
        public Vector2 startPoint;
        public Vector2 finishPoint;
    }


    private void DrawWall(Vector2 start, Vector2 finish)
    {
        WallParameters wall = new WallParameters();
        wall.startPoint = start;
        wall.finishPoint = finish;
        _walls.Add(wall);
        
        Vector2 wallPosition = new Vector2((start.x + finish.x) / 2, (start.y + finish.y) / 2);
        float rotationInDegrees = Mathf.Rad2Deg * Mathf.Atan2(start.x - finish.x, start.y - finish.y);
        Quaternion wallRotation = Quaternion.Euler(0, 0, -rotationInDegrees);
        float wallLength = new Vector2(start.x - finish.x,start.y - finish.y).magnitude * _wallLengthCoef;

        GameObject newWall = Instantiate(_wallSample, wallPosition, wallRotation, transform);
        var localScale = newWall.transform.localScale;
        localScale = new Vector3(localScale.x,wallLength, localScale.z);
        newWall.transform.localScale = localScale;
    }
    
    private void OnDrawGizmos() //visualisation
    {
        //show map bounds
//        Gizmos.color = Color.green;
//        Gizmos.DrawLine(new Vector2(-_mapWidth / 2, _mapHeight / 2), new Vector2(_mapWidth / 2, _mapHeight / 2));
//        Gizmos.DrawLine(new Vector2(-_mapWidth / 2, -_mapHeight / 2), new Vector2(_mapWidth / 2, -_mapHeight / 2));
//        Gizmos.DrawLine(new Vector2(-_mapWidth / 2, _mapHeight / 2), new Vector2(-_mapWidth / 2, -_mapHeight / 2));
//        Gizmos.DrawLine(new Vector2(_mapWidth / 2, _mapHeight / 2), new Vector2(_mapWidth / 2, -_mapHeight / 2));

        if (MapNetGenerator == null)
            return;
        int max = NodeLevels.Values.Max();
        
        //show tiles positions
//        var vertices = MapNetGenerator.TriangleMesh.Vertices.Select(a => new Vector2(a.X, a.Y)).ToList();
//        for (int i = 0; i < vertices.Count(); i++)
//        {
//            Gizmos.color = NodeLevels.ContainsKey(i) ? new Color((float)NodeLevels[i] / max, 0, 0) : Color.magenta;
//            Gizmos.DrawSphere(vertices[i], 0.7f);
//        }

        
//        Gizmos.color = Color.red;
//        foreach (var edge in _links)
//        {
//            var edgeStart = vertices[edge.Elem1];
//            var edgeFinish = vertices[edge.Elem2];
//
//            Gizmos.DrawLine(edgeStart, edgeFinish);
//        }

        //show Voronoi
        Color с = Color.cyan;
        с.a = 0.1f;
        Gizmos.color = с;
        foreach (var edge in MapNetGenerator.VoronoiDiagram.Edges)
        {
            var edgeStart = new Vector2(MapNetGenerator.VoronoiDiagram.Vertices[edge.P0].X, MapNetGenerator.VoronoiDiagram.Vertices[edge.P0].Y);
            var edgeFinish = new Vector2(MapNetGenerator.VoronoiDiagram.Vertices[edge.P1].X, MapNetGenerator.VoronoiDiagram.Vertices[edge.P1].Y);
            Gizmos.DrawLine(edgeStart, edgeFinish);
        }
        

        //show Voronoi vertices
        Color сol = Color.cyan;
        сol.a = 0.1f;
        Gizmos.color = сol;
        foreach (var vertex in MapNetGenerator.VoronoiDiagram.Vertices)
        {
            Gizmos.DrawSphere(new Vector2(vertex.X, vertex.Y), 0.25f);
        }

        //show main path
//        Gizmos.color = Color.magenta;
//        for (int i = 1; i < MainPath.Length; i++)
//        {
//            var edge = new UnorderedPair<int>(MainPath[i - 1], MainPath[i]);
//            var edgeStart = vertices[edge.Elem1];
//            var edgeFinish = vertices[edge.Elem2];
//            Gizmos.DrawLine(edgeStart, edgeFinish);
//            Gizmos.DrawLine(edgeStart, edgeFinish);
//        }

        //show walls
        Gizmos.color = Color.cyan;
        foreach (var wall in _walls)
        {
            Gizmos.DrawLine(wall.startPoint, wall.finishPoint);
        }
    }
}
