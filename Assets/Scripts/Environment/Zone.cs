using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Zone
{
    public int HexCount = 0;
    public Guid ZoneGuid { get; private set; }
    private List<Hexagon> hexagons;

    private Color color;
    private int _madnessDegree;

    public Zone(Color color, int madnessDegree)
    {
        ZoneGuid = Guid.NewGuid();
        this.color = color;
        _madnessDegree = madnessDegree;
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

    public IEnumerable<Hexagon> GetInternalHexas()
    {
        var borderHexes = GetHexagons().Where(h => h.ReturnNeighborsHex().All(n => n.GetZone() == this));
        return borderHexes;
    }

    public void GenerateWalls()
    {
        var hexas = GetInternalHexas();
        foreach (Hexagon hex in hexas)
        {
            var walls = hex.GetWalls();
            foreach (Wall wall in walls)
            {
                if (wall != null && !wall.IsActive())
                {
                    wall.Enable();
                    break;
                }
            }

        }
    }

    public void ChangeWalls()
    {
        Player player = Player.Instance;
        var hexas = GetInternalHexas();
        for(int j = 0; j < hexas.Count(); j++) {
            var hexRandomCount = UnityEngine.Random.Range(0, hexas.Count());
            var hex = hexas.ElementAt(hexRandomCount);
            if (hex == player.GetCurrentHexagon())
                continue;
            var walls = hex.GetActiveWalls();
            for (int i = 0; i < walls.Count(); i++)
            {
                var wallRandomCount = UnityEngine.Random.Range(0, walls.Count());
                var wall = walls.ElementAt(wallRandomCount);
                if (wall.IsActive())
                {
                    wall.Disable();
                    _madnessDegree--;
                }
                else
                {
                    wall.Enable();
                    _madnessDegree--;
                }
                if (_madnessDegree <= 0)
                    break;
            }
            if (_madnessDegree <= 0)
                break;
        }
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
