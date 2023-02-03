using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntigravityBehaviour : MonoBehaviour
{
    public enum Axis{X,Y,Z};

    private Vector3 baseEulerAngles;
    private bool inEditor;
    private Vector3 positionOnFace;
    private Transform faceParent;
    // Start is called before the first frame update
    void Start()
    {
        positionOnFace = transform.localPosition;
        inEditor = EditCustomLevel.Instance != null;
        baseEulerAngles = transform.localEulerAngles;
        faceParent = transform.parent;
        transform.SetParent(null);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = faceParent.position + faceParent.rotation * positionOnFace;
        transform.eulerAngles = baseEulerAngles;
    }
}
