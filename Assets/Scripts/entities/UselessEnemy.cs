using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UselessEnemy : AIEnemy
{
    public override void DestroySelf()
    {
        return;
    }

    protected override bool IsTargetReached()
    {
        return false;
    }

    protected override void OnTargetReached()
    {
        return;
    }

    protected override void StepTowardsTarget()
    {
        return;
    }

}
