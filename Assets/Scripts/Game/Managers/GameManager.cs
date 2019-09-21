﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    #region Fields
    [SerializeField] private SnapGrid _grid;

    private OwnerState<GameManager> _state = null;
    #endregion

    #region Properties
    public SnapGrid Grid { get => _grid; }
    public OwnerState<GameManager> State
    {
        get
        {
            return _state;
        }

        set
        {
            _state?.OnStateExit();
            _state = value;
            _state?.OnStateEnter();
        }
    }
    #endregion

    #region Methods
    #region MonoBehaviour Callbacks
    void Update()
    {
        CheckForStateChangement();

        _state?.Tick();
    }

#if UNITY_EDITOR
    void OnGUI()
    {
        Rect rect = new Rect(0, 0, 700, 800);
        string label = "Current state: " + _state;
        GUIStyle style = new GUIStyle
        {
            fontSize = 15,
        };

        GUI.Label(rect, label, style);
    }
#endif
    #endregion

    void CheckForStateChangement()
    {
        if (_state == null)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                State = new BuildingState(this);
            }
        }
    }
    #endregion
}
