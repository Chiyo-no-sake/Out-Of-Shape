using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LivingEntity : WorldEntity
{

    [SerializeField] protected int health = 1;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TakeHit(int damage){

      health -= damage;

      if(health <= 0)
        DestroySelf();

    }

    public abstract void DestroySelf();

}
