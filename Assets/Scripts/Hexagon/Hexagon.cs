using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

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
        Debug.Log("Color was changed");
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

        Debug.Log("Wall was changed");
    }

    //Disable randoms walls
    public void WallOff()
    {
        if (_walls != null)
        {

            var number = Random.Range(0, _walls.Length);
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
            place = new Vector2(Random.Range(-_radius, _radius) + transform.position.x, Random.Range(-_radius, _radius) + transform.position.y);
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
    public List<GameObject> ReturnFreeNeighbours()
    {
        Debug.Log("Enter method ReturnFreeNeighbours()");
        List<GameObject> freeNeighbours = new List<GameObject>();
        Vector2 origin = GetComponent<Transform>().position;

        List<Transform> neighbours = new List<Transform>(ReturnNeighbors());
        foreach (Transform neighbour in neighbours)
        {
            Vector2 target = neighbour.position;

            RaycastHit2D hit = Physics2D.Linecast(origin, target, 1 << LayerMask.NameToLayer("Obstacle"));
            //Debug.Log("target: " + target + ", origin: " + origin);
            //Debug.Log(hit.collider.gameObject.tag);
            if (hit.collider == null) 
            {
                Debug.Log("Added" + neighbour.position);
                freeNeighbours.Add(neighbour.gameObject);
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
}
