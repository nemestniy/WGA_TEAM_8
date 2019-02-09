using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hexagon : MonoBehaviour
{
    private Wall[] _walls;
    private List<Vector2> _directions;
    private float _radius;
    private PolygonCollider2D _collider;

    [SerializeField] private List<Transform> _neighbors = null;

    //Initialization of all components
    private void Awake()
    {
        //Getting all walls on current hexagon
        _walls = transform.GetComponentsInChildren<Wall>();

        //Getting radius of hexagon
        var sprite = GetComponent<SpriteRenderer>().sprite;
        _radius = (sprite.rect.width * transform.localScale.x) / (2 * sprite.pixelsPerUnit);

        //Getting control of hexagonCollider
        _collider = GetComponent<PolygonCollider2D>();
        if (_collider)
            _collider.enabled = false;
    }

    public void ActivateCollider()
    {
        if(_collider)
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


    //Disable randoms walls
    public void WallOff()
    {
        if(_walls != null)
        {

            var number = Random.Range(0, _walls.Length);
            var neighbor = _walls[number].GetNeighbor(_radius * Mathf.Sqrt(3));

            if (_walls[number].IsActive())
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
                if (underWall != null && wall.IsActive() && underWall.tag == "Wall")
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
            Collider2D[] colliders = Physics2D.OverlapCircleAll(place, distance);
            foreach (Collider2D collider in colliders)
                if (collider.tag == "Furniture")
                {
                    //If there are one or more colliders with tag furniture, finding new random position for furniture
                    check = true;
                    attemptsCount++;
                }
        }
        //doing while check = true and number of attempts haven`t big number
        while (check && attemptsCount < attempts);

        //if number of attempts isn`t big number, instantiate new object
        if(attemptsCount < attempts)
            Instantiate(furnitureObject, place, Quaternion.identity);
    }

    public Wall[] GetWalls()
    {
        return _walls;
    }
}
