using UnityEngine;


public class ObjectsGenerator : MonoBehaviour
{
    [SerializeField] private GameObject[] _objects;

    [SerializeField] private float _distanceBetweenObjects;

    [SerializeField] private int _attemptsNumber;

    private HexagonsGenerator _hexagonsGenerator;

    private void Awake()
    {
        _hexagonsGenerator = GetComponent<HexagonsGenerator>();
        _hexagonsGenerator.MapIsCreate += GenerateObjects;
    }

    private void GenerateObjects()
    {
        var _hexObjects = GameObject.FindGameObjectsWithTag("Hexagon");
        foreach (GameObject hexObject in _hexObjects)
        {
            for (int i = 0; i < _objects.Length; i++) {
                hexObject.GetComponent<Hexagon>().GenerateObjects(_distanceBetweenObjects, _attemptsNumber, _objects[i]);
            }
        }
    }
}
