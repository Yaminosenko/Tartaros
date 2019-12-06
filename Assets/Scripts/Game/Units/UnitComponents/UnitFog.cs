﻿using FogOfWar;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class UnitFog : UnitComponent
{
    #region Enum
    public enum Type
    {
        Viewer = 0,
        Coverable = 1
    }
    #endregion

    #region Fields
    [Header("Viewer Settings")]
    [SerializeField] private SpriteRenderer _fogOfWarVision = null;
    [Header("Coverable Settings")]
    [SerializeField] private FOWCoverable _coverable = new FOWCoverable();

    private Type _type;
    #endregion

    #region Properties
    public bool IsCover { get => _type == Type.Viewer ? false : _coverable.IsCover; }
    public FOWCoverable Coverable { get => _coverable; }
    #endregion

    #region Methods
    #region MonoBehaviour Callbacks
    void Start()
    {
        SetTypeField();
        RegisterToManager();
    }

    void OnDestroy()
    {
        RemoveFromFOWManager();
    }
    #endregion

    #region Private methods
    void RemoveFromFOWManager()
    {
        switch (_type)
        {
            case Type.Viewer:
                FOWManager.Instance?.RemoveViewer(this);
                break;

            case Type.Coverable:
                FOWManager.Instance?.RemoveCoverable(this);
                break;
        }
    }

    void SetTypeField()
    {
        if (UnitManager.Team == Team.Sparta)
        {
            _type = Type.Viewer;
        }
        else
        {
            _type = Type.Coverable;
        }
    }

    void RegisterToManager()
    {
        switch (_type)
        {
            case Type.Viewer:
                FOWManager.Instance.AddViewer(this);

                _fogOfWarVision.gameObject.SetActive(true);
                _fogOfWarVision.transform.localScale = Vector3.one * UnitManager.Data.ViewRadius * 2;
                break;

            case Type.Coverable:
                FOWManager.Instance.AddCoverable(this);

                _fogOfWarVision.gameObject.SetActive(false);
                break;
        }
    }
    #endregion
    #endregion
}