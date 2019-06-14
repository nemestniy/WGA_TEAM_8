using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu]
public class Cutscene : ScriptableObject
{
    public bool isVideo;
    public VideoClip video;
    public AudioClip sound;
    public GameObject additionalUI;
    public List<Frame> frames;

}

[System.Serializable]
public struct SubtitleInfo
{
    public string Subtitles;
    public Character Speaker;
    public float Delay;
}

[System.Serializable]
public struct Frame
{
    public Sprite image;
    public float secondsToChange;
    public bool canChangeWithClick;
    public SubtitleInfo[] Subtitles;
}