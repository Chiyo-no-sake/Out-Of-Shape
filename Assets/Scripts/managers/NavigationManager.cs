using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationManager : IManager
{
    private SphericalNavMesh navMesh;

    private static NavigationManager instance;

    public static NavigationManager GetInstance()
    {
        if (instance == null) instance = new NavigationManager();
        return instance;
    }

    public void Init()
    {
        navMesh = UnityEngine.GameObject.FindGameObjectWithTag("Ground").GetComponent<SphericalNavMesh>();
    }

    public bool IsReady()
    {
        return navMesh.IsUpdatedCorrectly();
    }

    public void OnSetupComplete()
    {

    }
}
