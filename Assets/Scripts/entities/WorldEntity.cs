using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldEntity : MonoBehaviour
{
    [SerializeField] protected GameObject currentPlanet = null;
    [SerializeField] private int teamNumber = 0;

    public int GetTeamNumber()
    {
        return teamNumber;
    
    }

    public GameObject GetCurrentPlanet()
    {
        return currentPlanet;
    }


}
