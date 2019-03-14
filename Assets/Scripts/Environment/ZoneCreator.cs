using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ZoneCreator : MonoBehaviour
{
    public int MaxZoneSize = 12;
    //[SerializeField] private int _numberOfZone;
    //[SerializeField] private int _firstLimit;

    private HexagonsGenerator _hexagonsGenerator;

    public List<Zone> _zones;

    private int _recursionController;
    private IEnumerable<Hexagon> _generatedHexagons;

    private void Awake()
    {
        _hexagonsGenerator = GetComponent<HexagonsGenerator>();
        _hexagonsGenerator.MapIsCreate += CreateZones;
    }

    private void CreateZones()
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
            Zone newZone = new Zone(newColor);

                GenerateZone(GetRandomEdgeHex(hexObjects), newZone);

        }

        FillClearHexagons();
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

        for (int i = 0; i <= MaxZoneSize; i++)
        {
            var neighborHexesWitoutZone = ReturnFreeHexNeighbors(lastHex).ToList();
            newZone.AddHexagons(neighborHexesWitoutZone);
            i += neighborHexesWitoutZone.Count;
            lastHex = newZone.GetHexagons().FirstOrDefault(h => h.ReturnNeighbors().Any(n => n.GetComponent<Hexagon>().GetZone() == null));
        }
    }

    private IEnumerable<Hexagon> ReturnFreeHexNeighbors( Hexagon hex)
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

