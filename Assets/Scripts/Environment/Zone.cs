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
    [EnumFlags]
    public ZoneType Type;    
    public static ZoneType lastType = ZoneType.Starting;

    [Flags]
    public enum ZoneType
    {
        NotSet = 0,
        Starting = 1,
        Madness = 2,
        DeepOnes = 4,
        Statues = 8,
        All = 255,
    }

    private Color color;
    private int _madnessDegree;

    public Zone(Color color, int zoneCount)
    {
        ZoneGuid = Guid.NewGuid();
        Type = lastType;
        lastType = (ZoneType)((int)lastType * 2);
        if ((int)lastType > (0 << zoneCount))
        {
            lastType = ZoneType.Starting;
        }
        this.color = color;
        switch (Type)
        {
            case ZoneType.Starting: _madnessDegree = 0; break;
            case ZoneType.Madness: _madnessDegree = zoneCount * 2; break;
            default: _madnessDegree = 1; break;
        }
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
        int madness = 0;
        Player player = Player.Instance;
        var hexas = GetInternalHexas();
        for(int i = 0; i < hexas.Count(); i++)
        {
            var hexNumber = UnityEngine.Random.Range(0, hexas.Count());
            var hex = hexas.ElementAt(hexNumber);

            if (hex == player.GetCurrentHexagon() || player.IsVisible(hex.GetComponent<Collider2D>()))
                continue;

            var walls = hex.GetActiveWalls();
            foreach (Wall wall in walls)
            {
                if (!wall.IsBorder())
                {
                    if (wall.IsActive())
                    {
                        wall.Disable();
                        madness++;
                    }
                    else
                    {
                        wall.Enable();
                        madness++;
                    }
                }
                if (madness >= _madnessDegree) 
                    break;
            }
            
            if (madness >= _madnessDegree)
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
