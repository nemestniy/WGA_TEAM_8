using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEditorInternal;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = System.Random;


public class Hexagon : MonoBehaviour
{
    private Wall[] _walls;
    private List<Vector2> _directions;
    private float _radius;
    private PolygonCollider2D _collider;
    private SpriteRenderer _spriteRenderer;

    private GameObject _lastObject;

    private Color _zoneColor;
    private Zone _ownZone;

    [SerializeField] private List<Transform> _neighbors = null;


    private bool isVisited;

    private List<Transform> freeNeigbours = null;
    
    //public delegate void OnWallsChangeAction(); - Нафиг нам этот делегат, если есть стандартный Action мы не используем аргументов;
    
    public static event Action OnWallsChange;

    public float GetRadius()
    {
        return _radius;
    }


    //Initialization of all components
    private void Awake()
    {
        //Getting all walls on current hexagon
        _walls = transform.GetComponentsInChildren<Wall>();

        //Getting radius of hexagon
        _spriteRenderer = GetComponent<SpriteRenderer>();

        var sprite = _spriteRenderer.sprite;
        _radius = (sprite.rect.width * transform.localScale.x) / (2 * sprite.pixelsPerUnit);

        //Getting control of hexagonCollider
        _collider = GetComponent<PolygonCollider2D>();
        if (_collider)
            _collider.enabled = false;

    }

    public Color GetZoneColor()
    {
        return _zoneColor;
    }

    public Zone GetZone()
    {
        return _ownZone;
    }

    public void SetZone(Zone newZone)
    {
        _ownZone = newZone;
        _zoneColor = newZone.GetColor();

        _spriteRenderer.color = newZone.GetColor();
        //Debug.Log("Color was changed");
    }

    public void ActivateCollider()
    {
        if (_collider)
            _collider.enabled = true;
    }


    public Vector2 GetPosition()
    {
        return transform.position;
    }

    public void GenerateHexagonLayer()
    {
        if (_walls != null)
        {
            //Get wall and check neighbor
            foreach (Wall wall in _walls)
            {
                if (wall.GetNeighbor(_radius * Mathf.Sqrt(3)) == null)
                {
                    // if neighbor is no, generate new neighbor
                    var hexagon = Instantiate(gameObject,
                        wall.GetDirection() * (Mathf.Sqrt(3) * _radius) + (Vector2)transform.position,
                        Quaternion.identity) as GameObject;
                    if(transform.parent != null)
                        hexagon.transform.parent = transform.parent;
                }

            }
        }
    }

    public void GetNeighbors()
    {
        if (_walls != null)
        {
            foreach (Wall wall in _walls)
            {
                var neighbor = wall.GetNeighbor(_radius * Mathf.Sqrt(3));
                if (neighbor != null)
                {
                    _neighbors.Add(neighbor.parent);
                }

            }
        }
    }

    public List<Transform> ReturnNeighbors()
    {
        return _neighbors;
    }

    public bool NeighborContains(Transform neighbor)
    {
        var neighbors = ReturnNeighbors();
        return neighbors.Contains(neighbor);
    }

    public void ChangeWall()
    {
        Wall wallSwitched = new Wall();
        for (int i = 0; i < _walls.Length; i++)
        {
            if (_walls[i] != null)
            {
                if (_walls[i].IsActive())
                {
                    wallSwitched = _walls[i];
                    _walls[i].Disable();
                    continue;
                }
            }
        }

        for (int i = 0; i < _walls.Length; i++)
        {
            if (_walls[i] != null)
            {
                if (!_walls[i].IsActive() && _walls[i] != wallSwitched && !_walls[i].GetWallUnderMe().CompareTag("Wall"))
                {
                    _walls[i].Enable();
                    continue;
                }
            }
        }
        
        OnWallsChange?.Invoke();
        //Debug.Log("Wall was changed");
    }

    //Disable randoms walls
    public void WallOff()
    {
        if (_walls != null)
        {

            var number = UnityEngine.Random.Range(0, _walls.Length);
            var neighbor = _walls[number].GetNeighbor(_radius * Mathf.Sqrt(3));

            if (_walls[number].IsActive() && !_walls[number].IsBorder())
                _walls[number].Disable();
            else
                WallOff();
        }
    }

    //Delete excess walls
    public void DestroyExcessWalls()
    {
        if (_walls != null)
        {
            foreach (Wall wall in _walls)
            {
                //check wall under other wall
                var underWall = wall.GetWallUnderMe();
                if (underWall != null && wall.IsActive() && underWall.CompareTag("Wall"))
                    Destroy(underWall.gameObject);
            }
        }
    }

    public void GenerateObjects(float distance, int attempts, GameObject furnitureObject)
    {
        bool check;
        int attemptsCount = 0;
        Vector2 place;
        do
        {
            check = false;
            place = new Vector2(UnityEngine.Random.Range(-_radius, _radius) + transform.position.x, UnityEngine.Random.Range(-_radius, _radius) + transform.position.y);
            if (_lastObject != null && Vector2.Distance(place, _lastObject.transform.position) < distance)
            {
                //If there are one or more colliders with tag furniture, finding new random position for furniture
                check = true;
                attemptsCount++;
            }
        }
        //doing while check = true and number of attempts haven`t big number
        while (check && attemptsCount < attempts);

        //if number of attempts isn`t big number, instantiate new object
        if (attemptsCount < attempts)
            _lastObject = Instantiate(furnitureObject, place, Quaternion.identity);
    }

    [HideInInspector]
    public List<GameObject> Objects = new List<GameObject>();
    public void FillHexagon()
    {
        switch (UnityEngine.Random.Range(0, 3))
        {
            case 0: FillCentricHexagon(); break;
            case 1: FillMiddleHexagon(); break;
            case 2: FillDecorativeHexagon(); break;
        }

        ///Debug code for hexa story type
        //var s = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //s.transform.position = this.transform.position;
        //s.name = GetBiome().ToString();
        //switch (GetStoryType())
        //{
        //    case HexaStoryType.Border:
        //        s.transform.localScale = Vector3.one * 5;
        //        break;
        //    case HexaStoryType.Zone:
        //        s.transform.localScale = Vector3.one * 10;
        //        break;
        //    case HexaStoryType.Story:
        //        s.transform.localScale = Vector3.one * 15;
        //        break;
        //}
        

        FixObjects();
    }

    public enum HexaStoryType
    {
        Border,
        Zone,
        Story,
    }

    public BackgroundController.Biome GetBiome()
    {
        //return GameManager.Instance.MapManager.Background.GetBiomeByPosition(transform.position);
        int cWatr = 0, cRock = 0, cSand = 0;
        var points = Enumerable.Range(0, 6).Select(a =>
            transform.position + _radius * 0.5f * new Vector3(Mathf.Sin(a * Mathf.PI / 3), Mathf.Cos(a * Mathf.PI / 3), 0)).Select(a => GameManager.Instance.MapManager.Background.GetBiomeByPosition(a));
        cWatr = points.Count(a => a == BackgroundController.Biome.Water);
        cRock = points.Count(a => a == BackgroundController.Biome.Rocky);
        cSand = points.Count(a => a == BackgroundController.Biome.Sandy);
        switch (GameManager.Instance.MapManager.Background.GetBiomeByPosition(transform.position))
        {
            case BackgroundController.Biome.Rocky: cRock++; break;
            case BackgroundController.Biome.Water: cWatr++; break;
            case BackgroundController.Biome.Sandy: cSand++; break;
        }
        if (cWatr > cRock && cWatr > cSand) return BackgroundController.Biome.Water;
        else if (cRock > cSand) return BackgroundController.Biome.Rocky;
        else return BackgroundController.Biome.Sandy;
    }

    public HexaStoryType GetStoryType()
    {        
        if (IsBorder)
            return HexaStoryType.Border;
        if (IsZone) return HexaStoryType.Zone;
        return HexaStoryType.Story;
    }
    public bool IsBorder
    {
        get
        {
            var hexaN = _neighbors.Select(a => a.GetComponent<Hexagon>()).ToList();
            var myBiome = GetBiome();
            return (hexaN.Any(a =>
                a.GetBiome() != myBiome));
        }
    }

    public bool IsZone
    {
        get
        {
            var hexaN = _neighbors.Select(a => a.GetComponent<Hexagon>()).ToList();            
            return !IsBorder && (hexaN.Any(a =>a.IsBorder));
        }
    }

    public bool IsStory
    {
        get
        {            
            return !IsBorder && !IsZone;
        }
    }

    public void FillCentricHexagon()
    {
        AddCentralObject(3);
        AddMedianObject(1);        
    }

    public void FillMiddleHexagon()
    {
        AddMedianObject(2);
        AddMedianObject(2);
        for (int i = 0; i < UnityEngine.Random.Range(1, 4); i++) {
            AddDecorativeObject(1);
        }
    }

    public void FillDecorativeHexagon()
    {
        for (int i = 0; i < UnityEngine.Random.Range(2, 5); i++)
        {
            AddDecorativeObject(1);
        }
    }

    public void FillWalledHexagon()
    {
        
    }

    private int _maxFixIter = 16;
    public void FixObjects()
    {
        for (int i = 0; i < _maxFixIter; i++)
        {
            Objects.ForEach(FixObjectIter);
        }
    }

    public void FixObjectIter(GameObject obj)
    {
        Vector3 pos = obj.transform.position - this.transform.position;
        Vector3 res = Vector3.zero;
        var sizeMod = ObjectsGenerator.sizePrefix.ToList().IndexOf(obj.name.Substring(5, 2)) + 1;
        foreach (var o in Objects.Where(o => o != obj))
        {
            var l = (obj.transform.position - o.transform.position);
            var distMod = (o.transform.position - this.transform.position).magnitude / _radius;
            res += l / l.sqrMagnitude;
        }    
        Debug.Log(res);
        obj.transform.position += res * 2/*Mathf.Sqrt(pos.magnitude / _radius)*/ / Mathf.Pow(sizeMod, 1);
        
    }

    public void AddCentralObject(int size)
    {
        var obj = ObjectsGenerator.Instance.SpawnObjectForPlace(size,
            transform.position /*+ (Vector3)(UnityEngine.Random.insideUnitCircle * _radius * 0.1f)*/);
        obj.transform.SetParent(this.transform);
        Objects.Add(obj);
    }

    public void AddMedianObject(int size)
    {
        var v1 = UnityEngine.Random.onUnitSphere * _radius * 0.4f;
        v1.z = 0;
        v1.Normalize();
        var obj = ObjectsGenerator.Instance.SpawnObjectForPlace(size,
            transform.position + v1 /*+ (Vector3)(UnityEngine.Random.insideUnitCircle * _radius * 0.1f)*/);
        obj.transform.SetParent(this.transform);
        Objects.Add(obj);
    }

    public void AddDecorativeObject(int size)
    {
        var obj = ObjectsGenerator.Instance.SpawnObjectForPlace(size,
            transform.position + (Vector3)(UnityEngine.Random.insideUnitCircle * _radius * 0.65f));
        obj.transform.SetParent(this.transform);
        Objects.Add(obj);
    }



    public Wall[] GetWalls()
    {
        return _walls;
    }
    public void ActivateBorderWalls()
    {
        if (_walls != null)
        {
            foreach (Wall wall in _walls)
            {
                var neighbor = wall.GetNeighbor(_radius * Mathf.Sqrt(3));
                if (neighbor == null)
                {
                    wall.SetBorder();
                }

            }
        }
    }

    //---------------Часть Лехи
    public List<Hexagon> ReturnFreeNeighbours()
    {
        //Debug.Log("Enter method ReturnFreeNeighbours()");
        List<Hexagon> freeNeighbours = new List<Hexagon>();
        Vector2 origin = GetComponent<Transform>().position;

        List<Transform> neighbours = new List<Transform>(ReturnNeighbors());
        foreach (Transform neighbour in neighbours)
        {
            Vector2 target = neighbour.position;

            RaycastHit2D hit = Physics2D.Linecast(origin, target, 1 << LayerMask.NameToLayer("Obstacle"));
            ////Debug.Log("target: " + target + ", origin: " + origin);
            ////Debug.Log(hit.collider.gameObject.tag);
            if (hit.collider == null) 
            {
                //Debug.Log("Added" + neighbour.position);
                freeNeighbours.Add(neighbour.gameObject.GetComponent<Hexagon>());
            }
        }

        return freeNeighbours;
    }

    public void SetVisited()
    {
        isVisited = true;
    }

    public void SetUnvisited()
    {
        isVisited = false;
    }

    public bool IsVisited()
    {
        return isVisited;
    }

    public GameObject GenerateWell(GameObject well)
    {
        return Instantiate(well, transform);
    }
}

