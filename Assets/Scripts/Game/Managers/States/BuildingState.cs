﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingState : OwnerState<GameManager>
{
    #region Fields
    private GameObject _currentBuilding = null;
    #endregion

    #region Methods
    public BuildingState(GameManager owner) : base(owner) { }

    public override void OnStateEnter()
    {
        SetCurrentBuilding(Building.Barracks);
    }

    public override void Tick()
    {
        if (_currentBuilding == null)
            return;

        UpdateCurrentBuildingPosition();
    }

    public void SetCurrentBuilding(Building building)
    {
        var prefab = BuildingsRegister.Instance.GetBuildingPrefab(building);
        _currentBuilding = Object.Instantiate(prefab);
        UpdateCurrentBuildingPosition();
    }

    void UpdateCurrentBuildingPosition()
    {
        Vector3? newPosition = GameManager.Instance.Grid.GetNearestPointFromMouse();

        if (newPosition != null)
        {
            _currentBuilding.transform.position = (Vector3)newPosition;
        }
    }
    #endregion
}
