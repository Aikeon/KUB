using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChangeForKeys : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI[] texts;
    [SerializeField] InputManager.Keys[] codes;
    // Start is called before the first frame update
    void Start()
    {
        var len = codes.Length;
        for (int i = 0; i < len; i++)
        {
            texts[i].text = InputManager.Instance.convert(codes[i]).ToString();
        }
    }
}
