using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpResetterBehaviour : MonoBehaviour
{

    //TODO maybe a "collectible" superclass with respawnTime and animation ??

    [SerializeField] private float respawnTime;
    private MeshRenderer rend;
    private Collider col;

    void Start()
    {
        rend = GetComponent<MeshRenderer>();
        col = GetComponent<Collider>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.TryGetComponent<PlayerControl>(out var pc)) return;

        pc.RegainJump();
        //TODO collect anim
        rend.enabled = false;
        col.enabled = false;
        StartCoroutine(respawning());
    }

    IEnumerator respawning()
    {
        yield return new WaitForSeconds(respawnTime);
        //TODO repop anim
        rend.enabled = true;
        col.enabled = true;
    }
}
