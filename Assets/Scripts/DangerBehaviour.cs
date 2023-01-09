using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DangerBehaviour : MonoBehaviour
{
    void OnTriggerEnter(Collider collisionInfo)
    {
        if (collisionInfo.gameObject.tag == "Player")
        {
            collisionInfo.gameObject.transform.parent.TryGetComponent<PlayerControl>(out var pc);
            pc.Die();
        }
    }
}
