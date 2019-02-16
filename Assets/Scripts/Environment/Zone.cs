using System.Collections.Generic;
using UnityEngine;

namespace Environment
{
    public class Zone
    {
        private List<Hexagon.Hexagon> hexagons;

        private Color color;

        public Zone(Color color)
        {
            this.color = color;
            hexagons = new List<Hexagon.Hexagon>();
        }

        public void AddHexagon(Hexagon.Hexagon hexagon)
        {
            hexagon.SetZone(this);
            hexagons.Add(hexagon);
        }

        public void AddHexagons(List<Hexagon.Hexagon> hexagons)
        {
            foreach (Hexagon.Hexagon hexagon in hexagons)
            {
                AddHexagon(hexagon);
            }
        }

        public List<Hexagon.Hexagon> GetHexagons()
        {
            return hexagons;
        }

        public Color GetColor()
        {
            return color;
        }

        public bool Contains(Hexagon.Hexagon hexagon)
        {
            return hexagons.Contains(hexagon);
        }

        public List<Hexagon.Hexagon> GetFreeNeighbors()
        {
            List<Hexagon.Hexagon> FreeNeighbors = new List<Hexagon.Hexagon>();

            Debug.Log("Zone: Hexagons count is " + hexagons.Count);
            foreach(Hexagon.Hexagon hexagon in hexagons)
            {
                var transformsNeighbors = hexagon.ReturnNeighbors();
                foreach(Transform transformNeighbor in transformsNeighbors)
                {
                    var currentNeighbor = transformNeighbor.GetComponent<Hexagon.Hexagon>();
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
}
