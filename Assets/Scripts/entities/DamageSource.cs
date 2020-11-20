using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DamageSource : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    [SerializeField] private GameObject casterGameObject = null;
    private IAttacker caster;

    public void SetCaster(IAttacker caster)
    {
        this.caster = caster;
    }

    public IAttacker GetCaster()
    {
        return caster;
    }

    public virtual void OnHit(LivingEntity other)
    {
        IAttacker caster = this.caster;

        if(casterGameObject != null)
        {
            caster = casterGameObject.GetComponent<IAttacker>();
        }

        caster.OnHit(other);
        if(caster.IsHostileTo(other))
            if(other.TakeHit(damage)) 
                caster.OnKill(other);
    }

    public virtual void OnWallHit()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        
        LivingEntity[] entities = other.gameObject.GetComponents<LivingEntity>();
        foreach (var entity in entities)
        {
            Debug.Log("colliding with: " + other.gameObject);   
            OnHit(entity);
        }

        if (entities.Length == 0)
        {
            OnWallHit();
        }

    }
}
