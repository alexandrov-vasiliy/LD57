using System;
using UnityEngine;

public class SonarEvents
{
    public static event Action<Transform, bool> OnSonarActivated;


    public static void SonarActivated(Transform pos, bool isSonarActive)
    {
        OnSonarActivated?.Invoke(pos, isSonarActive);
    }
}