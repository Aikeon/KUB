using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazy : MonoBehaviour
{
    Coroutine rotation;
    bool needReset;
    // Start is called before the first frame update
   void OnCollisionEnter(Collision collisionInfo)
    {
        if (collisionInfo.collider.TryGetComponent<PlayerControl>(out var pc) && rotation == null && !needReset) //TODO problem of localScale vs falling axis
        {
            var not_reversed = Vector3.right == transform.right;
            if (pc.transform.position.x - transform.position.x >= (not_reversed ? transform.localScale.x/2 : transform.localScale.y/2)) rotation = StartCoroutine(RotateKUB(-90));
            if (pc.transform.position.x - transform.position.x <= -(not_reversed ? transform.localScale.x/2 : transform.localScale.y/2)) rotation = StartCoroutine(RotateKUB(90));
            if (pc.transform.position.y - pc.transform.localScale.y/2f - transform.position.y >= (not_reversed ? transform.localScale.y/2 : transform.localScale.x/2)) rotation = StartCoroutine(RotateKUB(180));
        }
    }

    void OnCollisionExit(Collision collisionInfo)
    {
        if (collisionInfo.collider.TryGetComponent<PlayerControl>(out var pc)) needReset = false;
    }

    IEnumerator RotateKUB(float degrees)
    {
        needReset = true;
        gameObject.GetComponent<Collider>().enabled = false;
        var timeEllapsed = 0f;
        GameManager.Instance.Player.newVelocity = Vector3.zero;
        GameManager.Instance.pause = true;
        var kubT = GameManager.Instance.KUB.transform;
        while (timeEllapsed < 1f)
        {
            kubT.Rotate(degrees * Vector3.forward * Time.unscaledDeltaTime, Space.World);
            timeEllapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        kubT.eulerAngles = new Vector3(Mathf.RoundToInt(kubT.eulerAngles.x/90)*90,Mathf.RoundToInt(kubT.eulerAngles.y/90)*90,Mathf.RoundToInt(kubT.eulerAngles.z/90)*90);
        GameManager.Instance.pause = false;
        gameObject.GetComponent<Collider>().enabled = true;
        yield return new WaitForSeconds(0.025f);
        rotation = null;
    }
}
