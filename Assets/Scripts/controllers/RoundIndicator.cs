using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundIndicator : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameObject roundGO = GameObject.FindGameObjectWithTag("roundText");
        UnityEngine.UI.Text roundText = roundGO.GetComponent<UnityEngine.UI.Text>();
        roundText.text = RoundManager.GetInstance().GetRoundNum().ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
