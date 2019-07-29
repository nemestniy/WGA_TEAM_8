using UnityEngine;

public interface IFieldOfView
{
    void SetLightMode(int newMode, int prevMode, float changingState);
    PolygonCollider2D VisionCollider { get; }
    float CurrentIntensityMult { get; set; }
}
