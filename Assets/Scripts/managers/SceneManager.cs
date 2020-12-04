using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{

    private IManager __navManager;

    private void Awake()
    {

        List<IManager> managers = new List<IManager>();

        __navManager = new NavigationManager();

        managers.Add(__navManager);

        StartCoroutine("InitManagers", managers);

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator InitManagers(List<IManager> managers)
    {
        foreach (IManager manager in managers)
            manager.Init();

        yield return null;

        int managerCount = managers.Count;
        int readyCount = 0;


        while(readyCount < managerCount)
        {

            foreach (IManager manager in managers)
            {
                if (manager.IsReady())
                {
                    readyCount++;
                    Debug.Log(manager.ToString() + " is ready");
                }
            }

            yield return null;

        }


    }

}
