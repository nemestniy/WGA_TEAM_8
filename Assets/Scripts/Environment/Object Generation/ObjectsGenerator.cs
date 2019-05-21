using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using Random = UnityEngine.Random;

#if UNITY_EDITOR
using UnityEditor;
using System.IO;
[CustomEditor(typeof(ObjectsGenerator))]
public class ObjectsGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Load Objects"))
        {
            var list = Directory.GetFiles($"{Application.dataPath}/Prefabs/Environment/Furniture/", "*.prefab", SearchOption.AllDirectories).Select(a => a.Replace(Application.dataPath, "Assets"));

            var objList = new List<ObjectPrefabInfo>();
            foreach (var asset in list)
            {
                var obj = AssetDatabase.LoadAssetAtPath<ObjectPrefabInfo>(asset);
                if (obj != null) objList.Add(obj);
            }

            var tgt = ((ObjectsGenerator) target);
            tgt._objectPrefabs = objList./*Select(a => a.GetComponent<ObjectPrefabInfo>()).*/ToArray();


        }

        DrawDefaultInspector();         
    } 
}
#endif

public class ObjectsGenerator : MonoBehaviour
{
    [Header("Furniture parametrs:")]   

    [SerializeField] public ObjectPrefabInfo[] _objectPrefabs;

    [SerializeField] private float _distanceBetweenObjects;

    [SerializeField] private int _attemptsNumber;

    [HideInInspector] public List<IHexaFiller> hexaCreators;

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
    public const string anyBPrefix = "anyb_";
    [HideInInspector]
    public static readonly string[] biomePrefix = {waterPrefix, rockPrefix, sandPrefix};

    public GameObject GetPrefabForStats(int size, Hexagon owner, Vector3 position)
    {
        var biome = MapManager.Instance.Background.GetBiomeByPosition(position);        
        var l = _objectPrefabs.Where(a => a?.Check(biome, size, owner) ?? false).ToList();
        if (l.Count == 0)
            Debug.LogError($"Nothing to gen at {biome} of size {size}");
        if (l.Any(a => a.MinCountInDungeon - GetCountInDungeon(a.name) > 0))
            l = l.Where(a => a.MinCountInDungeon - GetCountInDungeon(a.name) > 0).ToList();
        if (l.Count == 0)
            Debug.LogError("Nothing to gen after filter");



        return l.WeightRandom(a => a.SpawnProbMultiplier).gameObject;
        
    }

    public int GetCountInDungeon(string name)
    {
        if (CountSpawns.ContainsKey(name)) return CountSpawns[name];
        return 0;
    }

    public ObjectPlaceholder PlaceholderPrefab;

    private List<ObjectPlaceholder> _listObjects = new List<ObjectPlaceholder>();
    public GameObject SpawnPlaceholderObject(Hexagon caller, int size, Vector3 position)
    {
        var obj = Instantiate(PlaceholderPrefab.gameObject, position, Quaternion.identity);
        var holder = obj.GetComponent<ObjectPlaceholder>();
        holder.Owner = caller;
        holder.Size = size;
        _listObjects.Add(holder);
        return obj;
    }

    public Dictionary<string, int> CountSpawns = new Dictionary<string, int>();


    public GameObject SpawnObject(ObjectPlaceholder placeholder)
    {        
        var prefab = GetPrefabForStats(placeholder.Size, placeholder.Owner, placeholder.transform.position);
        var obj = Instantiate(prefab, placeholder.transform.position, Quaternion.Euler(0, 0, Random.value * 360));
        if (!CountSpawns.ContainsKey(prefab.name)) CountSpawns[prefab.name] = 0;
        CountSpawns[prefab.name] = 1;
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
            hexObject.GetComponent<Hexagon>().FillHexagon();            
        }
             
        foreach (var objectPlaceholder in _listObjects)
        {
            objectPlaceholder.Compile();
        }

    }
}

static class ArrayRandom
{
    public static void Shuffle<T>(this List<T> array)
    {
        int n = array.Count;
        while (n > 1)
        {
            int k = Random.Range(0, n--);
            T temp = array[n];
            array[n] = array[k];
            array[k] = temp;
        }
    }

    public static T WeightRandom<T>(this List<T> array, Func<T, float> weight)
    {
        float w = array.Sum(weight);
        var r = Random.Range(0, w);
        foreach (var elem in array)
        {
            r -= weight(elem);
            if (r < 0) return elem;
        }
        return array.Last();
    }
}
