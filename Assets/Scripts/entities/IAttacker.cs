using UnityEngine;
using System.Collections;

public interface IAttacker
{
    void Attack();
    void OnHit(LivingEntity other);
    void OnKill(LivingEntity other);

    bool IsHostileTo(WorldEntity other);
}