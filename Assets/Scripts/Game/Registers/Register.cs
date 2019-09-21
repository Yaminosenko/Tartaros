﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Register<TPrefab, TEnum> : Singleton<Register<TPrefab, TEnum>> where TEnum : struct, System.Enum where TPrefab : class
{
    #region Fields
    private TPrefab[] _prefabs;
    #endregion

    #region Properties
    protected virtual TPrefab[] Prefabs { get => _prefabs; set => _prefabs = value; }
    protected virtual int DeltaIndex { get => 0; }
    #endregion

    #region Methods   
    // force array to be the size of TEnum
    void OnValidate()
    {
        int tEnumLength = Enum.GetValues(typeof(TEnum)).Length;

        // save old values
        if (Prefabs == null) Prefabs = new TPrefab[tEnumLength];
        TPrefab[] oldValues = Prefabs;

        // create a new TEnum
        Prefabs = new TPrefab[tEnumLength];

        // find copy length
        int copyLength = oldValues.Length;
        if (copyLength > Prefabs.Length)  copyLength = Prefabs.Length; 

        // then copy
        Array.Copy(oldValues, Prefabs, copyLength);
    }

    public TPrefab GetItem(TEnum itemEnum)
    {
        int index = (int)(object)itemEnum;
        index -= DeltaIndex;

        Debug.Log("Prefab length = " + Prefabs.Length + " index = " + index);
        return Prefabs[index];
    }
    #endregion
}
