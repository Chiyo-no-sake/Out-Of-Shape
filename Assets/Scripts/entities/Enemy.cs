using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public abstract class Enemy: LivingEntity
{
    protected abstract void StepTowardsTarget();
    protected abstract bool IsTargetReached();
    protected abstract void OnTargetReached();

    public void Update()
    {
        if (IsTargetReached())
        {
            OnTargetReached();
        }
        else
        {
            StepTowardsTarget();
        }
    }

}
