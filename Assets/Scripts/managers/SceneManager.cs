using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{

    private IManager __navManager;
    private IManager __roundManager;
    private IManager __audioManager;

    private void Awake()
    {

        List<IManager> managers = new List<IManager>();

        __audioManager = AudioManager.GetInstance();
        __navManager = NavigationManager.GetInstance();
        __roundManager = RoundManager.GetInstance();

        managers.Add(__navManager);
        managers.Add(__roundManager);
        managers.Add(__audioManager);

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

        foreach(IManager manager in managers)
        {
            manager.OnSetupComplete();
        }
    }

    public void ToDeathScreen()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("DeathScreen", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

}
