using UnityEngine;


public class ObjectsGenerator : MonoBehaviour
{
    [Header("Furniture parametrs:")]
    [SerializeField] private GameObject[] _objects;

    [SerializeField] private float _distanceBetweenObjects;

    [SerializeField] private int _attemptsNumber;

    [Header("Wells parametrs:")]
    [SerializeField] private int _countWell;

    [SerializeField] private GameObject _well;

    private HexagonsGenerator _hexagonsGenerator;

    private void Awake()
    {
        _hexagonsGenerator = GetComponent<HexagonsGenerator>();
        _hexagonsGenerator.MapIsCreate += GenerateObjects;
    }

    private void GenerateObjects()
    {
        var hexObjects = GameObject.FindGameObjectsWithTag("Hexagon");
        foreach (GameObject hexObject in hexObjects)
        {
            for (int i = 0; i < _objects.Length; i++) {
                hexObject.GetComponent<Hexagon>().GenerateObjects(_distanceBetweenObjects, _attemptsNumber, _objects[i]);
            }
        }

        for(int i = 0; i < _countWell; i++)
        {
            int randomHexagonNumber = Random.Range(0, hexObjects.Length);
//            hexObjects[randomHexagonNumber].GetComponent<Hexagon>().GenerateWell(_well);
        }
    }
}
