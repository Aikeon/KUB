using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Glue : MonoBehaviour
{
    private float baseGravity;
    private int baseAddJumps;

    void Start() //TODO problem on build only ??? wtf
    {
        baseGravity = GameManager.Instance.Player.gravity;
        baseAddJumps = GameManager.Instance.Player.additionalJumps;   
    }

    void OnCollisionEnter(Collision collisionInfo)
    {

        if (collisionInfo.collider.TryGetComponent<PlayerControl>(out var pc))
        {
            if (!Physics.Raycast(pc.transform.position,Vector3.down,pc.transform.localScale.y + 0.02f, ~LayerMask.GetMask("Glue"), QueryTriggerInteraction.UseGlobal))
            {
                pc.gravity = 0;
                pc.additionalJumps = 10000;
            }
        }
    }

    void OnCollisionExit(Collision collisionInfo)
    {
        if (collisionInfo.collider.TryGetComponent<PlayerControl>(out var pc))
        {
            Debug.DrawRay(pc.transform.position,(pc.transform.localScale.y + 0.02f) * Vector3.up, Color.red, 1);
            if (!Physics.Raycast(pc.transform.position,Vector3.up,pc.transform.localScale.y + 0.02f, ~LayerMask.GetMask("Glue"), QueryTriggerInteraction.UseGlobal))
            {
                pc.gravity = baseGravity;
                pc.additionalJumps = baseAddJumps;
                pc.ResetJumps();
            }
        }
    }
}
