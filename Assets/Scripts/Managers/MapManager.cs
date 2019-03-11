﻿using System.Collections.Generic;
using UnityEngine;


public class MapManager : MonoBehaviour, Manager
{
    public BackgroundController Background;
    public delegate void MapManagerEvents();

    public event MapManagerEvents HexagonGenerated;
    public event MapManagerEvents ZoneCreated;
    public event MapManagerEvents ObjectGenerated;

    private HexagonsGenerator _hexagonsGenerator;
    private ZoneCreator _zoneCreator;
    private ObjectsGenerator _objectsGenerator;

    private void Awake()
    {
        _hexagonsGenerator = GetComponent<HexagonsGenerator>();
        _zoneCreator = GetComponent<ZoneCreator>();
        _objectsGenerator = GetComponent<ObjectsGenerator>();
        Background = FindObjectOfType<BackgroundController>();
    }

    public void StartManager()
    {
        _hexagonsGenerator.GenerateMap();
//        HexagonGenerated?.Invoke();
//
//        _zoneCreator.CreateZones();
//        ZoneCreated?.Invoke();
//
//        _objectsGenerator.GenerateObjects();
//        ObjectGenerated?.Invoke();
    }

    public void PauseManager()
    {
        
    }

    public void ResumeManager()
    {
        
    }
}
