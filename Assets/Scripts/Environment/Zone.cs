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

    public int GetMadnessDegree()
    {
        return _madnessDegree;
    }

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
