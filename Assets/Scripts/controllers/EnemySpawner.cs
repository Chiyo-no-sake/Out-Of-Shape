using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public void Spawn(Enemy enemy)
    {
        Instantiate(enemy);
        enemy.transform.position = transform.position;
    }

    public bool IsFree()
    {
        LayerMask layerMask = ~LayerMask.GetMask("Ground");
        return Physics.SphereCast(new Ray(transform.position, -transform.position), 1.3f, 5.0f, layerMask);
    }
}