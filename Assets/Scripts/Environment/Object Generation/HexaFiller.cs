using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHexaFiller
{
    bool Checkhexagon(Hexagon hex);

    bool Fill(Hexagon hex);

    int MinCountInDungeon { get; set; }
    int MaxCountInDungeon { get; set; }

    float ProbabilityWeight { get; set; }

}

public class CentricHexaFiller : IHexaFiller
{
    public bool Checkhexagon(Hexagon hex)
    {
        throw new System.NotImplementedException();
    }

    public bool Fill(Hexagon hex)
    {
        throw new System.NotImplementedException();
    }

    public int MinCountInDungeon { get => 0;
        set { }
    }
    public int MaxCountInDungeon { get => 1000;
        set { }
    }
    public float ProbabilityWeight { get => 0.33f;
        set { }
    }
}