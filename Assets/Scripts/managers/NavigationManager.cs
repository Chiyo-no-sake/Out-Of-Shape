using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationManager : IManager
{
    private SphericalNavMesh navMesh;
    public void Init()
    {
        navMesh = UnityEngine.GameObject.FindGameObjectWithTag("Ground").GetComponent<SphericalNavMesh>();
        navMesh.ComputeMeshData();
    }

    public bool IsReady()
    {
        return navMesh.IsUpdatedCorrectly();
    }
}
