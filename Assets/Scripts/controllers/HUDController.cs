using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{

    private Slider healthbarSlider;
    [SerializeField] private GameObject player;

    // Start is called before the first frame update
    void Start()
    {

        healthbarSlider = gameObject.GetComponentInChildren<Slider>();
        healthbarSlider.maxValue = player.GetComponent<PlayerController>().GetMaxHealth();
    }

    // Update is called once per frame
    void Update()
    {
        if(player != null)
        {

            int currentHealth = player.GetComponent<PlayerController>().GetCurrentHealth();

            healthbarSlider.value = Mathf.Lerp(healthbarSlider.value, currentHealth, Time.deltaTime * 3);
        }
    }
}
