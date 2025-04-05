using System;
using UnityEngine;

public class SonarEvents
{

    public static event Action OnSonarActivated;


    public static void SonarActivated()
    {
        OnSonarActivated?.Invoke();
    }



}
