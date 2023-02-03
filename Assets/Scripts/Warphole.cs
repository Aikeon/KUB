using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warphole : MonoBehaviour
{
    Coroutine rotation;
    public bool needReset;
    [SerializeField] Warphole otherSide;

    void OnCollisionEnter(Collision collisionInfo)
    {
        if (collisionInfo.collider.TryGetComponent<PlayerControl>(out var pc) && rotation == null && !needReset)
        {
            StartCoroutine(GoToOtherSide());
        }
    }

    void OnCollisionExit(Collision collisionInfo)
    {
        if (collisionInfo.collider.TryGetComponent<PlayerControl>(out var pc) && rotation != null)
        {
            needReset = false;
            otherSide.needReset = false;
        }
    }

    IEnumerator GoToOtherSide()
    {
        //Anim falling in
        needReset = true;
        otherSide.needReset = true;
        gameObject.GetComponent<Collider>().enabled = false;
        var timeEllapsed = 0f;
        GameManager.Instance.pause = true;
        var kubT = GameManager.Instance.KUB.transform;
        while (timeEllapsed < 1f)
        {
            kubT.Rotate(180 * Vector3.up * Time.unscaledDeltaTime, Space.World);
            timeEllapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        //Anim falling out
        kubT.eulerAngles = new Vector3(Mathf.RoundToInt(kubT.eulerAngles.x/90)*90,Mathf.RoundToInt(kubT.eulerAngles.y/90)*90,Mathf.RoundToInt(kubT.eulerAngles.z/90)*90);
        GameManager.Instance.pause = false;
        GameManager.Instance.Player.transform.position = otherSide.transform.position;
        gameObject.GetComponent<Collider>().enabled = true;
        yield return new WaitForSeconds(0.025f);
        rotation = null;
    }
}
