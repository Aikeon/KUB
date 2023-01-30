using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartLevel : MonoBehaviour
{
    public string levelName;
    // Start is called before the first frame update
    void Start()
    {
        SetAndShowName(levelName); //TODO play it after the transition
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) SetAndShowName(levelName);
    }

    public void SetAndShowName(string name)
    {
        levelName = name;
        GameManager.Instance.showLevelName(levelName);
    }
}
