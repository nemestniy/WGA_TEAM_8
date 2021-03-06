﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HexagonsGenerator : MonoBehaviour
{
    [SerializeField] private int _mapSize;

    [SerializeField] private int _wallsForDelete;

    [SerializeField] private Hexagon _startHexagon;
    

    public delegate void HexagonEvents();
    public event HexagonEvents HexagonsIsCreate;
    public event HexagonEvents MapIsCreate;

    private List<Wall> _closingMapWalls = new List<Wall>();

//    void Start()
//    {
//        GenerateMap();
//    }

    public void GenerateMap()
    {
        GenerateHexagons(_mapSize);
        FindNeighbors();
        CloseMap();
        //DestroyAllExcessWalls();
        //DisableAllWalls();
        //ClearWalls(_wallsForDelete);
        //ActivateColliders();

        InitializeHexArray();
        Debug.Log("Hex array is created");
        HexagonsIsCreate();
        Debug.Log("Objects are created");
        MapIsCreate();
        Debug.Log("Map is created");
    }

    public void InitializeHexArray()
    {
        hexagonsArraylist = new LinkedList<GameObject>(GameObject.FindGameObjectsWithTag("Hexagon"));
    }
    
    [SerializeField] [ShowOnly]
    private LinkedList<GameObject> hexagonsArraylist;
    public GameObject GetHexagonByPoint(Vector3 point)
    {
        
        float distanceX = float.MaxValue;
        GameObject hexagon = null;
        
        
        foreach (GameObject hex in hexagonsArraylist)
        {
            float distance = Vector3.Distance(hex.transform.position, point);

            if (distance < hex.GetComponent<Hexagon>().InternalRadius) //if position is inside internal radius of the hex
            {
                //insert hex to start of the list to cache it
                hexagonsArraylist.Remove(hex);//insert hex to start of the list to cache it
                hexagonsArraylist.AddLast(hex);
                return hex;
            }
            
            if (distanceX > distance)
            {
                distanceX = distance;
                hexagon = hex;
            }
        }

        //insert hex to start of the list to cache it
        hexagonsArraylist.Remove(hexagon);
        hexagonsArraylist.AddLast(hexagon);

        return hexagon;
    }

    private void CloseMap()
    {
        var _hexObjects = GameObject.FindGameObjectsWithTag("Hexagon");
        foreach (GameObject hexObject in _hexObjects)
        {
            var walls = hexObject.GetComponent<Hexagon>().ActivateBorderWalls();
            if (walls.Count >= 1)
                foreach (Wall wall in walls)
                    _closingMapWalls.Add(wall);
        }
    }

    public List<Wall> GetClosingMapWalls()
    {
        return _closingMapWalls;
    }

    public void DisableAllWalls()
    {
        var walls = GetWallsWithoutBorder();
        foreach(Wall wall in walls)
        {
            wall.Disable();
        }
    }

    private IEnumerable<Wall> GetWallsWithoutBorder()
    {
        return FindObjectsOfType<Wall>().Where(h => !h.IsBorder());
    }

    public void ActivateColliders()
    {
        var _hexObjects = GameObject.FindGameObjectsWithTag("Hexagon");
        foreach (GameObject hexObject in _hexObjects)
        {
            hexObject.GetComponent<Hexagon>().ActivateCollider();
        }
    }

    private void GenerateHexagons(int mapSize)
    {
        for (int i = 0; i < mapSize; i++)
        {
            var _hexObjects = GameObject.FindGameObjectsWithTag("Hexagon");
            foreach (GameObject hexObject in _hexObjects)
            {
                hexObject.GetComponent<Hexagon>().GenerateHexagonLayer();
            }
        }

        //List<Hexagon> lastLayer = new List<Hexagon>();
        //for(int i = 0; i < mapSize; i++)
        //{
        //    var layer = new List<Hexagon>();
        //    if(i == 0)
        //    {
        //        for(var j = 0; j < 6; j++)
        //        {
        //            var hexagon = Instantiate(_startHexagon.gameObject,
        //                GetDirection(j) * (Mathf.Sqrt(3) * _startHexagon.GetRadius()) + (Vector2)_startHexagon.transform.position,
        //                Quaternion.identity) as GameObject;
        //            if (transform.parent != null)
        //                hexagon.transform.parent = transform.parent;
        //            layer.Add(hexagon.GetComponent<Hexagon>());
        //        }
        //        lastLayer = layer;
        //        continue;
        //    }

        //    foreach(var hex in lastLayer)
        //    {
        //        for(var j = 0; j < 6; j++)
        //        {
        //            var direction = GetDirection(j);
        //            var hit = Physics2D.RaycastAll(hex.transform.position, direction, 20);
        //            if(hit != null)
        //            {
        //                if (hit.FirstOrDefault(h => h.transform.tag == "Hexagon").transform != null)
        //                    continue;

        //                var hexagon = Instantiate(_startHexagon.gameObject,
        //                direction * (Mathf.Sqrt(3) * hex.GetRadius()) + (Vector2)hex.transform.position,
        //                Quaternion.identity) as GameObject;
        //                if (transform.parent != null)
        //                    hexagon.transform.parent = transform.parent;
        //                layer.Add(hexagon.GetComponent<Hexagon>());
        //            }
        //        }
        //    }
        //}
    }

    private Vector2 GetDirection(int i)
    {
        switch (i)
        {
            case 0:
                return new Vector2(0, 1);
            case 1:
                return new Vector2(0.87f, 0.5f).normalized;
            case 2:
                return new Vector2(0.87f, -0.5f).normalized;
            case 3:
                return Vector2.down;
            case 4:
                return new Vector2(-0.87f, -0.5f).normalized;
            case 5:
                return new Vector2(-0.87f, 0.5f).normalized;
        }

        return Vector2.zero;
    }

    private void FindNeighbors()
    {
        var _hexObjects = GameObject.FindGameObjectsWithTag("Hexagon");
        foreach (GameObject hexObject in _hexObjects)
        {
            hexObject.GetComponent<Hexagon>().GetNeighbors();
        }
    }

    private void ClearWalls(int wallsOnHexagon)
    {
        var _hexObjects = GameObject.FindGameObjectsWithTag("Hexagon");
        foreach (GameObject hexObject in _hexObjects)
        {
            for (int i = 0; i < wallsOnHexagon; i++)
            {
                hexObject.GetComponent<Hexagon>().WallOff();
            }
        }
    }

    public void DestroyAllExcessWalls()
    {
        var _hexObjects = GameObject.FindGameObjectsWithTag("Hexagon");
        foreach (GameObject hexObject in _hexObjects)
        {
            hexObject.GetComponent<Hexagon>().DestroyExcessWalls();
        }
    }

    public void ClearMap()
    {
        var _hexObjects = GameObject.FindGameObjectsWithTag("Hexagon");
        foreach (GameObject hexObject in _hexObjects)
        {
            Destroy(hexObject);
        }
    }

    public void GenerateStartHexagon()
    {
        Instantiate(_startHexagon, Vector2.zero, Quaternion.identity);
    }
}
