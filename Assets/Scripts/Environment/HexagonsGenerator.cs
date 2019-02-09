using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexagonsGenerator : MonoBehaviour
{
    [SerializeField] private int _mapSize;

    [SerializeField] private int _wallsForDelete;

    [SerializeField] private GameObject _startHexagon;

    public delegate void HexagonEvents();
    public event HexagonEvents MapIsCreate;

    // Start is called before the first frame update
    void Start()
    {
        GenerateMap();
    }

    public void GenerateMap()
    {
        GenerateHexagons(_mapSize);
        FindNeighbors();
        ClearWalls(_wallsForDelete);
        DestroyAllExcessWalls();
        ActivateColliders();

        MapIsCreate();
    }

    private void ActivateColliders()
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

    private void DestroyAllExcessWalls()
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
        foreach(GameObject hexObject in _hexObjects)
        {
            Destroy(hexObject);
        }
    }

    public void GenerateStartHexagon()
    {
        Instantiate(_startHexagon, Vector2.zero, Quaternion.identity);
    }
}
