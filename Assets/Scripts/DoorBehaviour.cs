using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorBehaviour : MonoBehaviour
{
    [SerializeField] Transform KUB;
    [SerializeField] Transform Player;
    public static Coroutine movement;
    private bool hasDebuged;
    private float timeDebug;
    private int latestRot;

    void OnTriggerEnter(Collider collisionInfo)
    {
        if (movement != null || !collisionInfo.gameObject.TryGetComponent<PlayerControl>(out var pc) || hasDebuged) {hasDebuged = true; return;}
        if (CameraControl._changing || !CameraControl.currentlyOrthographic) {StartCoroutine(debugCameraControl()); return;}
        if (Player.position.x < 6 && Player.position.x > -6)
        {
            if (Player.position.y > 0)
            {
                movement = StartCoroutine(DoorUp());
                latestRot = 0;
            }
            else 
            {
                movement = StartCoroutine(DoorDown());
                latestRot = 1;
            }
        }
        else 
        {
            if (Player.position.x > 0)
            {
                movement = StartCoroutine(DoorRight());
                latestRot = 2;
            }
            else 
            {
                movement = StartCoroutine(DoorLeft());
                latestRot = 3;
            }
        }
    }

    IEnumerator debugCameraControl()
    {
        while (CameraControl._changing || !CameraControl.currentlyOrthographic)
        {
            yield return null;
        }
        if (Player.position.x < 6 && Player.position.x > -6)
        {
            if (Player.position.y > 0)
            {
                movement = StartCoroutine(DoorUp());
                latestRot = 0;
            }
            else 
            {
                movement = StartCoroutine(DoorDown());
                latestRot = 1;
            }
        }
        else 
        {
            if (Player.position.x > 0)
            {
                movement = StartCoroutine(DoorRight());
                latestRot = 2;
            }
            else 
            {
                movement = StartCoroutine(DoorLeft());
                latestRot = 3;
            }
        }
    }

    void OnTriggerStay(Collider col)
    {
        timeDebug += Time.deltaTime;
        if (timeDebug > 0.05f && !hasDebuged && latestRot == 0)
        {
            StartCoroutine(DoorDown());
            hasDebuged = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        timeDebug = 0;
        hasDebuged = false;
        Player.position = new Vector3(Player.position.x, Player.position.y, -10.5f);
    }

    IEnumerator DoorUp()
    {
        gameObject.GetComponent<Collider>().enabled = false;
        var timeEllapsed = 0f;
        GameManager.Instance.pause = true;
        while (timeEllapsed < 1f)
        {
            KUB.Rotate(90 * Vector3.left * Time.unscaledDeltaTime, Space.World);
            timeEllapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        KUB.eulerAngles = new Vector3(Mathf.RoundToInt(KUB.eulerAngles.x/90)*90,Mathf.RoundToInt(KUB.eulerAngles.y/90)*90,Mathf.RoundToInt(KUB.eulerAngles.z/90)*90);
        gameObject.GetComponent<Collider>().enabled = true;
        GameManager.Instance.pause = false;
        Player.position = new Vector3(Player.position.x, -10.49f, -10.5f);
        yield return new WaitForSeconds(0.025f);
        movement = null;
    }

    IEnumerator DoorDown()
    {
        gameObject.GetComponent<Collider>().enabled = false;
        var timeEllapsed = 0f;
        GameManager.Instance.pause = true;
        while (timeEllapsed < 1f)
        {
            KUB.Rotate(-90 * Vector3.left * Time.unscaledDeltaTime, Space.World);
            timeEllapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        KUB.eulerAngles = new Vector3(Mathf.RoundToInt(KUB.eulerAngles.x/90)*90,Mathf.RoundToInt(KUB.eulerAngles.y/90)*90,Mathf.RoundToInt(KUB.eulerAngles.z/90)*90);
        gameObject.GetComponent<Collider>().enabled = true;
        GameManager.Instance.pause = false;
        Player.position = new Vector3(Player.position.x, 10.49f, -10.5f);
        yield return new WaitForSeconds(0.025f);
        movement = null;
    }

    IEnumerator DoorLeft()
    {
        gameObject.GetComponent<Collider>().enabled = false;
        var timeEllapsed = 0f;
        GameManager.Instance.pause = true;
        while (timeEllapsed < 1f)
        {
            KUB.Rotate(-90 * Vector3.up * Time.unscaledDeltaTime, Space.World);
            timeEllapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        KUB.eulerAngles = new Vector3(Mathf.RoundToInt(KUB.eulerAngles.x/90)*90,Mathf.RoundToInt(KUB.eulerAngles.y/90)*90,Mathf.RoundToInt(KUB.eulerAngles.z/90)*90);
        gameObject.GetComponent<Collider>().enabled = true;
        GameManager.Instance.pause = false;
        Player.position = new Vector3(10.49f, Player.position.y, -10.5f);
        yield return new WaitForSeconds(0.025f);
        movement = null;
    }

    IEnumerator DoorRight()
    {
        gameObject.GetComponent<Collider>().enabled = false;
        var timeEllapsed = 0f;
        GameManager.Instance.pause = true;
        while (timeEllapsed < 1f)
        {
            KUB.Rotate(90 * Vector3.up * Time.unscaledDeltaTime, Space.World);
            timeEllapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        KUB.eulerAngles = new Vector3(Mathf.RoundToInt(KUB.eulerAngles.x/90)*90,Mathf.RoundToInt(KUB.eulerAngles.y/90)*90,Mathf.RoundToInt(KUB.eulerAngles.z/90)*90);
        gameObject.GetComponent<Collider>().enabled = true;
        GameManager.Instance.pause = false;
        Player.position = new Vector3(-10.49f, Player.position.y, -10.5f);
        yield return new WaitForSeconds(0.025f);
        movement = null;
    }
}
