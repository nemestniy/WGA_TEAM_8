using System.Collections.Generic;
using UnityEngine;


public class Zone
{
    private List<Hexagon> hexagons;

    private Color color;

    public Zone(Color color)
    {
        this.color = color;
        hexagons = new List<Hexagon>();
    }

    public void AddHexagon(Hexagon hexagon)
    {
        hexagon.SetZone(this);
        hexagons.Add(hexagon);
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
