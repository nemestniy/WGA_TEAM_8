using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class LampModeParametrs : ScriptableObject
{
    public float viewRadius;
    public float spotLightRadius;
    [UnityEngine.Range(0, 360)]
    public float viewAngle;
    [UnityEngine.Range(0, 360)]
    public float spotLightAngle;
    public float intensity;
    public float lightHeight;
    public float coordinateY;
    public Color lightColor;
}
