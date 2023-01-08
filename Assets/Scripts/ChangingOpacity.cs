using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class ChangingOpacity : MonoBehaviour
{

    enum Condition
    {
        isTutorial
    }

    [SerializeField] bool automaticLaunch;
    [SerializeField] Condition condition;
    [SerializeField] float baseDelay;
    [SerializeField] AnimationCurve opacityCurve;
    [SerializeField] bool looping;
    [SerializeField] float loopDelay;
    [SerializeField] Renderer rend;
    [SerializeField] TextMeshProUGUI textrend;
    bool hasLooped;
    Color baseColor;
    // Start is called before the first frame update
    void Start()
    {
        baseColor = (rend == null ) ? textrend.color : rend.material.color; 

        switch (condition)
        {
            case Condition.isTutorial : if (!GameManager.Instance.levelsUnlocked.Contains(7)) StartChange(); break;
            default : break;
        }
    }

    public void StartChange()
    {
        Debug.Log("started to change");
        StartCoroutine(coroutine());
    }

    IEnumerator coroutine()
    {
        if (!hasLooped) {hasLooped = true; yield return new WaitForSeconds(baseDelay);}
        var timeEllapsed = 0f;
        while (timeEllapsed < opacityCurve.keys[opacityCurve.keys.Length-1].time)
        {
            if (rend != null) rend.material.color = new Color(baseColor.r,baseColor.g,baseColor.b,Mathf.Lerp(0,1,opacityCurve.Evaluate(timeEllapsed)));
            else textrend.color = new Color(baseColor.r,baseColor.g,baseColor.b,Mathf.Lerp(0,1,opacityCurve.Evaluate(timeEllapsed)));
            timeEllapsed += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(loopDelay);
        if (looping) StartCoroutine(coroutine());
    }
}
