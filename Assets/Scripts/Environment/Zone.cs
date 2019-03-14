using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Zone
{
    public int HexCount = 0;
    public Guid ZoneGuid { get; private set; }
    private List<Hexagon> hexagons;

    private Color color;

    public Zone(Color color)
    {
        ZoneGuid = Guid.NewGuid();
        this.color = color;
        hexagons = new List<Hexagon>();
    }

    public void AddHexagon(Hexagon hexagon)
    {
        hexagon.SetZone(this);
        hexagons.Add(hexagon);
        HexCount++;
    }

    public void AddHexagons(List<Hexagon> hexagons)
    {
        foreach (Hexagon hexagon in hexagons)
        {
            AddHexagon(hexagon);
        }
    }

    public List<Hexagon> GetHexagons()
    {
        return hexagons;
    }

    public Color GetColor()
    {
        return color;
    }

    public bool Contains(Hexagon hexagon)
    {
        return hexagons.Contains(hexagon);
    }

    public List<Hexagon> GetFreeNeighbors()
    {
        List<Hexagon> FreeNeighbors = new List<Hexagon>();

        Debug.Log("Zone: Hexagons count is " + hexagons.Count);
        foreach(Hexagon hexagon in hexagons)
        {
            var transformsNeighbors = hexagon.ReturnNeighbors();
            foreach(Transform transformNeighbor in transformsNeighbors)
            {
                var currentNeighbor = transformNeighbor.GetComponent<Hexagon>();
                if (currentNeighbor.GetZone() == null && !FreeNeighbors.Contains(currentNeighbor))
                {
                    FreeNeighbors.Add(currentNeighbor);
                    Debug.Log("Free hexagon added");
                }
            }
        }
        return FreeNeighbors;
    }
}
