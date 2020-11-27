using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LivingEntity : WorldEntity
{

    [SerializeField] protected int maxHealth;
    [SerializeField] protected int health;

    // Start is called before the first frame update
    protected void Start()
    {
        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {

    }

    // return true if enemy dies
    public virtual bool TakeHit(int damage){

      health -= damage;

        if (health <= 0)
        {
            DestroySelf();
            return true;
        }

        return false;
    }

    public abstract void DestroySelf();

    public int GetMaxHealth()
    {
        return maxHealth;
    }
    
    public int GetCurrentHealth()
    {
        return health;
    }

}
