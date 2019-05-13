using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Cutscene : ScriptableObject
{
    public string cutsceneName;
    public AudioClip sound;
    public List<Frame> frames;
}

[System.Serializable]
public struct Frame
{
    public Sprite image;
    public float secondsToChange;
    public bool canChangeWithClick;
}