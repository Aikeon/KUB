using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DangerBehaviour : MonoBehaviour
{
    void OnCollisionEnter(Collision collisionInfo)
    {
        if (collisionInfo.collider.gameObject.TryGetComponent<PlayerControl>(out var pc))
        {
            pc.Die();
        }
    }
}
