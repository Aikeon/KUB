using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayCustomLevel : MonoBehaviour
{
    void Start()
    {
        GameManager.Instance.inGame = true;
        StartCoroutine(waitForPlayer());
    }

    IEnumerator waitForPlayer()
    {
        while (GameManager.Instance.Player == null)
        {
            yield return null;
        }
        LoadAction(SimpleFileBrowser.FileBrowser.Result[0]);
    }

    public void LoadAction(string path)
    {
        CustomLevelFileManager.LoadLevel(path,false);
    }
}
