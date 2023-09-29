using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GroundHeightData
{
    public float PosX { get; private set; }
    public float Height { get; private set; }
    public GroundHeightData(float posX, float height)
    {
        PosX = posX;
        Height = height;
    }
}
