using System.Linq;
using UnityEngine;


public class ObjectsGenerator : MonoBehaviour
{
    [Header("Furniture parametrs:")]
    [SerializeField] private GameObject[] _objects;

    [SerializeField] private float _distanceBetweenObjects;

    [SerializeField] private int _attemptsNumber;

    [Header("Wells parametrs:")]
//    [SerializeField] private int _countWell;

    [SerializeField] private GameObject _well;

    public float DistanceToWell
    {
        get
        {
            if (_well == null)
                return -1; //in case of error
            
            return Vector2.Distance(_well.gameObject.transform.position, Player.Instance.transform.position);
        }
    }
    
    [Header("Shadows parametrs:")]
    [SerializeField] public float _dynamicShadowOffset;

    private HexagonsGenerator _hexagonsGenerator;

    private void Awake()
    {
        _hexagonsGenerator = GetComponent<HexagonsGenerator>();
        _hexagonsGenerator.MapIsCreate += GenerateObjects;
        _instance = this;
    }

    private const string smallPrefix = "s_";
    private const string mediumPrefix = "m_";
    private const string largePrefix = "l_";
    [HideInInspector]
    public static readonly string[] sizePrefix = { smallPrefix, mediumPrefix, largePrefix};

    private const string waterPrefix = "watr_";
    private const string sandPrefix = "sand_";
    private const string rockPrefix = "rock_";
    [HideInInspector]
    public static readonly string[] biomePrefix = {waterPrefix, rockPrefix, sandPrefix};

    public GameObject GetPrefabForPrefix(string sizePrefix, string biomePrefix)
    {
        var list = _objects.Where(a => a.name.StartsWith(biomePrefix + sizePrefix)).ToList();
        if (list.Any())
        {
            return list.ElementAt(UnityEngine.Random.Range(0, list.Count()));
        }
        else return _objects.First();
    }

    public GameObject GetPrefabForStats(int size, BackgroundController.Biome biome)
    {
        return GetPrefabForPrefix(sizePrefix[size - 1], biomePrefix[(int) biome]);
    }

    public GameObject SpawnObjectForPlace(int size, Vector3 position)
    {
        var biome = MapManager.Instance.Background.GetBiomeByPosition(position);
        var prefab = GetPrefabForStats(size, biome);
        var obj = Instantiate(prefab, position, Quaternion.Euler(0, 0, Random.value * 360));
        return obj;
    }

    private static ObjectsGenerator _instance;
    public static ObjectsGenerator Instance
    {
        get => _instance;
    }

    private void GenerateObjects()
    {
        var hexObjects = GameObject.FindGameObjectsWithTag("Hexagon");
        foreach (GameObject hexObject in hexObjects)
        {

//            for (int i = 0; i < _objects.Length; i++) {
//                hexObject.GetComponent<Hexagon>().GenerateObjects(_distanceBetweenObjects, _attemptsNumber, _objects[i]);
//            }
            hexObject.GetComponent<Hexagon>().FillHexagon();
        }

        int randomHexagonNumber = Random.Range(0, hexObjects.Length);
        _well = hexObjects[randomHexagonNumber].GetComponent<Hexagon>().GenerateWell(_well);
//        for(int i = 0; i < _countWell; i++)
//        {
//            
//        }
    }

    private void OnDestroy()
    {
        _hexagonsGenerator.MapIsCreate -= GenerateObjects;
    }
}
