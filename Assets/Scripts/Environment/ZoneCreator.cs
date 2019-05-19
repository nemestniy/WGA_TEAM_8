using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ZoneCreator : MonoBehaviour
{
    public int MaxZoneSize = 12;
    [Range(0, 10000)]
    public int RandomSeed = 1000;

    private HexagonsGenerator _hexagonsGenerator;

    private List<Zone> _zones;

    private int _recursionController;
    private IEnumerable<Hexagon> _generatedHexagons;
 
    private void Awake()
    {
        _hexagonsGenerator = GetComponent<HexagonsGenerator>();
        Random.InitState(RandomSeed);
    }

    public List<Zone> GetZones()
    {
        return _zones;
    }

    public void CreateZones()
    {
        var Hexagons = GameObject.FindGameObjectsWithTag("Hexagon");
        var hexObjects = FindObjectsOfType<Hexagon>();

        _generatedHexagons = FindObjectsOfType<Hexagon>();

        //Debug.Log(Hexagons.Length);
        var zoneCount = Hexagons.Length / MaxZoneSize;
        //Debug.Log($"Zone count {zoneCount}");

        _zones = new List<Zone>();

        for(int i = 0; i < zoneCount; i++)
        {
            Color newColor = new Color(Random.value, Random.value, Random.value, 1);
            int madnessDegree = 0;
            if (i > 0)
                madnessDegree++;
            if (i == zoneCount - 1)
                madnessDegree = zoneCount * 2;
            Zone newZone = new Zone(newColor, madnessDegree);

            GenerateZone(GetRandomEdgeHex(hexObjects), newZone);
        }

        FillClearHexagons();
        GenerateWalls();

        _hexagonsGenerator.DestroyAllExcessWalls();
        _hexagonsGenerator.DisableAllWalls();
        //_hexagonsGenerator.ActivateColliders();
        ActivateRandomWalls();
    }

    private void GenerateWalls()
    {
        //foreach (Wall wall in Resources.FindObjectsOfTypeAll(typeof(Wall)))
        //{
        //    wall.Enable();
        //}

        foreach (var zone in _zones)
        {
            List<Wall> borderWalls = new List<Wall>();
            var borderHexes = GetBorderHexes(zone);
            foreach (var hex in borderHexes)
            {
                //var borderWall = hex.GetWalls().FirstOrDefault(w => hex.GetWallNeighbor(w) != null && hex.GetWallNeighbor(w).GetZone() != zone);
                foreach(var wall in hex.GetWalls())
                {
                    if (!wall.IsActive())
                    {
                        wall.Enable();
                        var wallNeighbor = hex.GetWallNeighbor(wall);
                        if (wallNeighbor != null)
                        {
                            if (wallNeighbor.GetZone() == zone)
                                wall.Disable();
                            else
                            {
                                wall.SetBorder();
                                borderWalls.Add(wall);
                            }
                        }
                    }
                    else
                    {
                        var wallNeighbor = hex.GetWallNeighbor(wall);
                        if (wallNeighbor != null)
                        {
                            if (wallNeighbor.GetZone() != zone)
                            {
                                wall.SetBorder();
                                //wall.ChangeColor(zone.GetColor());
                                borderWalls.Add(wall);
                            }
                        }
                    }
                    //var wallNeighbor = hex.GetWallNeighbor(wall);
                    //if (wallNeighbor != null)
                    //{
                    //    if (wallNeighbor.GetZone() == zone)
                    //    {
                    //        wall.Disable();
                    //    }
                    //    else
                    //    {
                    //        wall.SetBorder();
                    //    }
                    //}
                    //else
                    //{
                    //    wall.Enable();
                    //    wall.SetBorder();
                    //}
                }
                //if (borderWall != null)
                //{
                //    borderWall.SetBorder();
                //}
            }
            MakePass(borderWalls);
            /*var activeWall = _borderWalls[Random.Range(1, _borderWalls.Count - 1)];
            activeWall.Disable();
            var wallUnderActiveWall = activeWall.GetWallUnderMe();
            if (wallUnderActiveWall != null)
            {
                wallUnderActiveWall.gameObject.SetActive(false);
                Destroy(wallUnderActiveWall.gameObject);
            }*/



        }
        //foreach (var hex in FindObjectsOfType<Hexagon>())
        //{

        //    if (hex.GetWalls().Where(w => w.IsActive()).Count() == 6)
        //    {
        //        var randNum = Random.Range(0, 5);
        //        var randWall = hex.GetWalls().ToList()[randNum];
        //        randWall.Disable();
        //    }

        //    hex.ActivateBorderWalls();
        //}
    }

    private void MakePass(List<Wall> walls)
    {
        var borderZones = new List<Zone>();
        if (walls != null)
        {
            foreach (Wall wall in walls)
            {
                var neighborHex = wall.GetHexagonNeighbor();
                if (neighborHex != null)
                {
                    var zone = neighborHex.GetZone();
                    if (!borderZones.Contains(zone))
                        borderZones.Add(zone);
                }
            }

            foreach (Zone zone in borderZones)
            {
                var borderWall = walls.FirstOrDefault(h => h.GetNeighborZone() == zone);
                if (borderWall != null)
                    OpenPass(borderWall);
            }
        }
    }

    private void OpenPass(Wall activeWall)
    {
        Debug.Log("OpenPass");
        activeWall.Disable();
        var wallUnderActiveWall = activeWall.GetWallUnderMe();
        if (wallUnderActiveWall != null)
        {
            wallUnderActiveWall.gameObject.SetActive(false);
            Destroy(wallUnderActiveWall.gameObject);
        }
    }

    private Hexagon GetRandomNeighbor(List<Transform> neighborsTransforms, Color currentZone)
    {
        Hexagon randomNeighbor = neighborsTransforms[Random.Range(0, neighborsTransforms.Count-1)].GetComponent<Hexagon>();
        if (randomNeighbor.GetZoneColor() != currentZone && _recursionController < neighborsTransforms.Count)
        {
            _recursionController++;
            return GetRandomNeighbor(neighborsTransforms, currentZone);
        }
        if (_recursionController >= neighborsTransforms.Count)
            return null;
        return randomNeighbor;
    }

    private IEnumerable<Hexagon> GetBorderHexes(Zone zone)
    {
        var zoneHexes = FindObjectsOfType<Hexagon>().Where(h => h.GetZone() == zone);
        var borderHexes = zoneHexes.Where(h => h.ReturnNeighborsHex().Any(n => n.GetZone() != zone));
        return borderHexes;
    }

    private IEnumerable<Hexagon> GetInternalHexas(Zone zone)
    {
        var zoneHexes = FindObjectsOfType<Hexagon>().Where(h => h.GetZone() == zone);
        var borderHexes = zoneHexes.Where(h => h.ReturnNeighborsHex().All(n => n.GetZone() == zone));
        return borderHexes;
    }

    private Hexagon GetFirstFreeNeighbor(List<Transform> neighborsTransforms, Zone currentZone)
    {
        foreach(Transform hexTransform in neighborsTransforms)
        {
            Hexagon currentHex = hexTransform.GetComponent<Hexagon>();
            if (currentZone.Contains(currentHex))
                return currentHex;
        }
        return null;
    }

    private void GenerateZone(Hexagon startHexagon, Zone newZone) 
    {
        if (startHexagon == null)
            return;

        if (!_zones.Contains(newZone))
            _zones.Add(newZone);

        var lastHex = startHexagon;

        for (int i = 0; i < MaxZoneSize;)
        {
            var neighborHexesWitoutZone = ReturnFreeHexNeighbors(lastHex).ToList();
            if (i + neighborHexesWitoutZone.Count > MaxZoneSize)
            {
                for (var h = 0; h < MaxZoneSize - i; h++)
                {
                    newZone.AddHexagon(neighborHexesWitoutZone[h]);
                    i++;
                }
            }
            else
            {
                newZone.AddHexagons(neighborHexesWitoutZone);
                i += neighborHexesWitoutZone.Count;
            }
            lastHex = newZone.GetHexagons().FirstOrDefault(h => h.ReturnNeighbors().Any(n => n.GetComponent<Hexagon>().GetZone() == null));
        }
    }

    private void ActivateRandomWalls()
    {
        foreach(Zone zone in _zones)
        {
            zone.GenerateWalls();  
        }
    }

    private IEnumerable<Hexagon> ReturnFreeHexNeighbors(Hexagon hex)
    {
        return _generatedHexagons.Where(h => h.ReturnNeighbors().Contains(hex.transform) && h.GetZone() == null);
    }
    private Hexagon GetRandomHexagonNearAnyZone(IEnumerable<Hexagon> hexagons)
    {
        return hexagons.FirstOrDefault(h =>h.GetZone() != null && h.ReturnNeighbors().Any(hh => hh.GetComponent<Hexagon>().GetZone() == null));
    }

    private Hexagon GetRandomEdgeHex(IEnumerable<Hexagon> hexagons)
    {
        var edgeHexagons = hexagons.Where(h => h.ReturnNeighbors().Count < 6 && h.GetZone() == null).ToArray();
        return edgeHexagons[Random.Range(0, edgeHexagons.Length - 1)];
    }

    private Hexagon GetRandomEdgeHexNearAnyZone(IEnumerable<Hexagon> hexagons)
    {
        var edgeHexagons = hexagons.Where(h => h.ReturnNeighbors().Count < 6 && h.GetZone() == null && h.ReturnNeighbors().Any(n=>n.GetComponent<Hexagon>().GetZone() != null)).ToArray();
        return edgeHexagons[Random.Range(0, edgeHexagons.Length - 1)];
    }

    private Hexagon GetRandomHexagon(GameObject[] hexagons)
    {
        int hexagonNumber = 0;
        Hexagon currentHexagon = hexagons[hexagonNumber].GetComponent<Hexagon>();
        while (currentHexagon.GetZoneColor() != Color.clear)
        {
            hexagonNumber++;
            if (hexagonNumber >= (hexagons.Length - 1))
                return null;
            currentHexagon = hexagons[hexagonNumber].GetComponent<Hexagon>();
        }
        return currentHexagon;
    }

    private void FillClearHexagons()
    {
        foreach (Zone currentZone in _zones)
        {
            var hexagons = currentZone.GetFreeNeighbors();
            //Debug.Log(hexagons.Count);
            while (hexagons.Count >= 1)
            {
                currentZone.AddHexagons(hexagons);
                hexagons = currentZone.GetFreeNeighbors();
            }

        }
    }
}