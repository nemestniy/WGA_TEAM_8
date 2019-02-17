using System.Collections.Generic;
using UnityEngine;

public class ZoneCreator : MonoBehaviour
{
    [SerializeField] private int _numberOfZone;
    [SerializeField] private int _firstLimit;

    private HexagonsGenerator _hexagonsGenerator;

    public List<Zone> _zones;

    private int _recursionController;

    private void Awake()
    {
        _hexagonsGenerator = GetComponent<HexagonsGenerator>();
        _hexagonsGenerator.MapIsCreate += CreateZones;
    }

    private void CreateZones()
    {
        var Hexagons = GameObject.FindGameObjectsWithTag("Hexagon");

        _zones = new List<Zone>();

        for(int i = 0; i < _numberOfZone; i++)
        {
            Color newColor = new Color(Random.value, Random.value, Random.value, 1);
            Zone newZone = new Zone(newColor);

            GenerateZone(GetRandomHexagon(Hexagons), newZone);
        
        }

        FillClearHexagons();
    }

    private Hexagon GetRandomNeighbor(List<Transform> neighborsTransforms, Color currentZone)
    {
        //Debug.Log("Gettig random hex");
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
        if (startHexagon != null)
        {
            if (!newZone.Contains(startHexagon))
                newZone.AddHexagon(startHexagon);

            var neighborsTransforms = startHexagon.ReturnNeighbors();
            foreach (Transform neighborTransform in neighborsTransforms)
            {
                Hexagon neighbor = neighborTransform.GetComponent<Hexagon>();
                if (!newZone.Contains(neighbor) && neighbor.GetZone() == null)
                {
                    newZone.AddHexagon(neighbor);
                    if(newZone.GetHexagons().Count <= _firstLimit)
                        GenerateZone(neighbor, newZone);
                }
            }
            if (!_zones.Contains(newZone))
                _zones.Add(newZone);
        }
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

