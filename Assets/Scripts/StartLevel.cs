using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartLevel : MonoBehaviour
{
    public string levelName;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(showNameAtStart(levelName)); //TODO play it after the transition
    }

    IEnumerator showNameAtStart(string name)
    {
        yield return new WaitForSeconds(1);
        SetAndShowName(name);
    }

    public void SetAndShowName(string name)
    {
        levelName = name;
        GameManager.Instance.showLevelName(levelName);
    }
}
