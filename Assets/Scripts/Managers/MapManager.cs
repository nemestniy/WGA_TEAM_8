using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class MapManager : MonoBehaviour, Manager
{
    public BackgroundController Background;
    public delegate void MapManagerEvents();

    public event MapManagerEvents HexagonGenerated;
    public event MapManagerEvents ZoneCreated;
    public event MapManagerEvents ObjectGenerated;

    private HexagonsGenerator _hexagonsGenerator;
    private ZoneCreator _zoneCreator;
    private ObjectsGenerator _objectsGenerator;
    private Player _player;

    [SerializeField]
    private GameObject _exitTrigger;

    
    public bool IsLoaded { get; private set; }
    #region Singletone
    public static MapManager Instance { get; private set; }
    public MapManager() : base()
    {
        Instance = this;
    }
    #endregion
    
    
    
    private void Awake()
    {
        _hexagonsGenerator = GetComponent<HexagonsGenerator>();
        _zoneCreator = GetComponent<ZoneCreator>();
        _objectsGenerator = GetComponent<ObjectsGenerator>();
        Background = FindObjectOfType<BackgroundController>();

        _hexagonsGenerator.HexagonsIsCreate += _hexagonsGenerator_HexagonsIsCreate;
        _hexagonsGenerator.MapIsCreate += _hexagonsGenerator_MapIsCreate;
    }

    private void _hexagonsGenerator_HexagonsIsCreate()
    {
        _zoneCreator.CreateZones();
    }

    private void _player_CurrentHexagonChanged()
    {
        
        var zone = _player.GetCurrentZone();
        if (zone != null)
        {
            zone.ChangeWalls();
        }
    }

    private void _hexagonsGenerator_MapIsCreate()
    {
        _player = Player.Instance;
        _player.CurrentHexagonChanged += _player_CurrentHexagonChanged;
    }

    public void StartManager()
    {
       
        _hexagonsGenerator.GenerateMap();
        IsLoaded = true;
        //        HexagonGenerated?.Invoke();
        //
        //        _zoneCreator.CreateZones();
        //        ZoneCreated?.Invoke();
        //
        //        _objectsGenerator.GenerateObjects();
        //        ObjectGenerated?.Invoke(); 
    }

    public void PauseManager(){}
    public void ResumeManager(){}

    private void OnDestroy()
    {
        _hexagonsGenerator.MapIsCreate -= _hexagonsGenerator_MapIsCreate;
    }

    public void MakeMapPass()
    {
        var zones = _zoneCreator.GetZones();
        foreach(Zone zone in zones)
        {
            var walls = _hexagonsGenerator.GetClosingMapWalls().Where(h => h.GetZone() == zone);
            int wallCount = Random.Range(0, walls.Count());
            var wall = walls.ElementAt(wallCount);
            var wallPosition = wall.GetPosition();
            var generatePosition = new Vector3(wallPosition.x, wallPosition.y, _exitTrigger.transform.position.z);
            Instantiate(_exitTrigger, generatePosition, Quaternion.identity);
            wall.Disable();
        }
    }
}
