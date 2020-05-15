﻿using LeonidasLegacy.MapCellEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityResourcesGeneration : EntityComponent
{
    #region Fields
    private float _nextTimeGenerationTick = 0; // Time.time + Entity.Data.GenerationTick
    private ResourcesWrapper _resourcesPerTick;

    private bool _onEnableFirstFrame = true;
    private bool _resourceProductionEnable = true;
    #endregion

    #region Fields
    public bool EnableResourceProduction
    {
        get => _resourceProductionEnable;
        set
        {
            _resourceProductionEnable = value;
            CalculateNextTimeGenerationTick();
        }
    }

    #endregion

    #region Methods
    #region MonoBehaviour Callbacks
    void Start()
    {
        // disable this component if entity can't create resources
        if (!Entity.Data.CanCreateResources)
        {
            enabled = false;
        }
        else
        {
            CalculateResourcesPerTick();
            CalculateNextTimeGenerationTick();
        }
    }

    void Update()
    {
        TryCreateResources();
    }

    void OnEnable()
    {
        // because of multiscene
        // OnEnable Entity can't find GameManager
        this.ExecuteAfterFrame(SubscribeToEvents);

        // don't throw warning the first frame
        if (!_onEnableFirstFrame && !Entity.Data.CanCreateResources)
        {
            Debug.LogWarningFormat("Entity Resources Generation : You shouldn't enable this component because this entity can't generate resources.");
        }

        _onEnableFirstFrame = true;
    }

    void OnDisable()
    {
        UnsubscribeToEvents();
    }

    void OnDrawGizmos()
    {
        if (Entity != null && Entity.Data != null && Entity.Data.CanCreateResources)
            UnityEditor.Handles.DrawWireDisc(transform.position, transform.up, Entity.Data.RadiusToReachCells);
    }

    void OnGUI()
    {
        DrawGUI_ResourcesPerTick();
    }
    #endregion

    #region Events subscription
    private void SubscribeToEvents()
    {
        // listen to building game manager, to recalculate resoruces    
        GameManager.Instance.OnTileTerrainChanged += OnTileTerrainChanged;
    }

    private void UnsubscribeToEvents()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnTileTerrainChanged -= OnTileTerrainChanged;
        }
    }
    #endregion

    #region Events handlers
    private void OnTileTerrainChanged(Vector2Int coords, GameObject gameObjectAtCoords)
    {
        CalculateResourcesPerTick();
    }
    #endregion

    #region GUI Draw

    private void DrawGUI_ResourcesPerTick()
    {
        if (Camera.main == null)
        {
            Debug.LogErrorFormat("Entity Resources Generation : Camera.main is null. Can't draw resources per tick.");
            return;
        }

        // TODO: Draw wood X food X gold X above building
        Vector2 guiPosition = Camera.main.WorldToScreenPoint(transform.position);

        // The WorldToScreenPoint functions return and integer starting from 0,0
        // at the BOTTOM LEFT of the screen.
        // Because of this, the y-value is flipped.
        // So to solve the problem, substract the screen height.
        guiPosition.y = Screen.height - guiPosition.y;

        Rect labelRect = new Rect(guiPosition.x, guiPosition.y, 300, 50);
        GUI.Label(labelRect, _resourcesPerTick.ToString(true));
    }
    #endregion

    #region Create resources 
    void CalculateNextTimeGenerationTick()
    {
        _nextTimeGenerationTick = Time.time + Entity.Data.GenerationTick;
    }

    void TryCreateResources()
    {
        if (_resourceProductionEnable && _nextTimeGenerationTick <= Time.time)
        {
            CalculateNextTimeGenerationTick();
            CreateResources();
        }
    }

    void CreateResources()
    {
        GameManager.Instance.Resources += _resourcesPerTick;
    }
    #endregion

    #region Calculate Resources Per Tick Methods
    public void CalculateResourcesPerTick()
    {
        ResourcesWrapper resourcesPerTick = new ResourcesWrapper(0, 0, 0);

        foreach (var kvp in Entity.Data.ResourcesPerCell)
        {
            int cellCount = GetCellsInGenerationRadius(kvp.Key);
            resourcesPerTick += kvp.Value * cellCount;
        }

        _resourcesPerTick = resourcesPerTick;
    }

    int GetCellsInGenerationRadius(CellType cellType)
    {
        return GameManager.Instance.MapCells.GetCells_WorldPosition(transform.position.x, transform.position.z, Entity.Data.RadiusToReachCells, cellType).Length;
    }
    #endregion
    #endregion
}
