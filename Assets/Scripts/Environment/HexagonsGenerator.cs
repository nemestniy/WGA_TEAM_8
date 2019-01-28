using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexagonsGenerator : MonoBehaviour
{
    [SerializeField] private int _mapSize;

    [SerializeField] private float _distanceBetweenHexagons;

    [SerializeField] private int _wallsForDelete;

    [SerializeField] private GameObject _startHexagon;

    // Start is called before the first frame update
    void Start()
    {
        GenerateMap();
    }

    public void GenerateMap()
    {
        GenerateHexagons(_mapSize);
        ClearWalls(_wallsForDelete);
    }

    private void GenerateHexagons(int mapSize)
    {
        for (int i = 0; i < mapSize; i++)
        {
            var _hexObjects = GameObject.FindGameObjectsWithTag("Hexagon");
            foreach (GameObject hexObject in _hexObjects)
            {
                hexObject.GetComponent<Hexagon>().GenerateHexagonLayer(_distanceBetweenHexagons);
            }
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
