using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntigravityBehaviour : MonoBehaviour
{
    public enum Axis{X,Y,Z};

    [SerializeField] Axis axis;
    [SerializeField] Transform KUB;
    private Vector3 baseEulerAngles;
    // Start is called before the first frame update
    void Start()
    {
        baseEulerAngles = transform.localEulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        if (!CameraControl.currentlyOrthographic) return;
        switch (axis)
        {
            case Axis.X: transform.localEulerAngles = baseEulerAngles + ((Mathf.Abs(KUB.eulerAngles.z) == 90) ? (new Vector3(0, 0, KUB.eulerAngles.z)) : new Vector3(0, 0, KUB.eulerAngles.x)); break;
            case Axis.Y: transform.localEulerAngles = baseEulerAngles + ((Mathf.Abs(KUB.eulerAngles.z) == 90) ? (new Vector3(0, 0, KUB.eulerAngles.x)) : new Vector3(0, 0 , KUB.eulerAngles.y)); break;
            case Axis.Z: transform.localEulerAngles = baseEulerAngles + ((Mathf.Abs(KUB.eulerAngles.z) == 90) ? (new Vector3(0, 0, KUB.eulerAngles.y)) : new Vector3(0, 0, KUB.eulerAngles.z)); break;
        }
    }
}
