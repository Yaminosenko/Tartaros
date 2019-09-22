﻿using System;

public enum EntityType
{
    // units
    Alexios = 0,
    Kassandra = 1,

    // building
    House = 1000,
    Barracks = 1001
}

public enum Building
{
    House = EntityType.House,
    Barracks = EntityType.Barracks
}

public enum Unit
{
    Alexios = EntityType.Alexios,
    Kassandra = EntityType.Kassandra
}

public static class EntityTypeExtension
{
    public static Unit? IsUnitType(this EntityType e)
    {
        foreach (Unit item in Enum.GetValues(typeof(Unit)))
        {
            if ((int)item == (int)e)
            {
                return item;
            }
        }

        return null;
    }

    public static Building? IsBuildingType(this EntityType e)
    {
        foreach (Building item in Enum.GetValues(typeof(Building)))
        {
            if ((int)item == (int)e)
            {
                return item;
            }
        }

        return null;
    }
}