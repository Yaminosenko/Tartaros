﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Leonidas Legacy/Resources")]
public class ResourcesWrapperDatabase : ScriptableObject
{
    [SerializeField] private ResourcesWrapper _resourcesWrapper;
    public ResourcesWrapper ResourcesWrapper { get => _resourcesWrapper; }
}
