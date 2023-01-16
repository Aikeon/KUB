using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageColorChange : MonoBehaviour
{
    private Image img;
    [SerializeField] private float colorSwapSpeed = 1f;
    // Start is called before the first frame update
    void Start()
    {
        img = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        Color.RGBToHSV(img.color, out float hue, out float sat, out float v);
        hue += 0.01f * Time.deltaTime * colorSwapSpeed;
        img.color = Color.HSVToRGB(hue,sat,v);
    }
}
