using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hexagon : MonoBehaviour
{
    public Wall[] _walls;
    private List<Vector2> _directions;
    private float _radius;

    private void Awake()
    {
        _walls = transform.GetComponentsInChildren<Wall>();
        var sprite = GetComponent<SpriteRenderer>().sprite;
        _radius = sprite.rect.width / (2 * sprite.pixelsPerUnit);
    }

    public Vector2 GetPosition()
    {
        return transform.position;
    }

    public void GenerateHexagonLayer(float distanceBetweenHexagons)
    {
        if (_walls != null)
        {
            foreach (Wall wall in _walls)
            {
                if (wall.GetNeighbor(1.5f * distanceBetweenHexagons + (_radius * Mathf.Sqrt(3) / 2)) == null)
                {
                    var hexagon = Instantiate(gameObject,
                        wall.GetDirection() * (Mathf.Sqrt(3) * _radius + distanceBetweenHexagons) + (Vector2)transform.position,
                        Quaternion.identity) as GameObject;
                   // _neighbors.Add(hexagon.GetComponent<RaycastHit2D>());
                }

            }
        }
    }

    public void WallOff()
    {
        if(_walls != null)
        {
            var number = Random.Range(0, _walls.Length);
            Debug.Log(number);
            if (_walls[number].IsActive())
                _walls[number].Disable();
            else
                WallOff();
        }
    }
}
