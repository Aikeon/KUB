using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntigravityBehaviour : MonoBehaviour
{
    public enum Axis{X,Y,Z};

    public Axis axis;
    [SerializeField] Transform KUB;
    public Quaternion baseRot;
    private Vector3 baseEulerAngles;
    private bool inEditor;
    static public List<AntigravityBehaviour> staticPlatforms = new List<AntigravityBehaviour>();
    // Start is called before the first frame update
    void Start()
    {
        inEditor = EditCustomLevel.Instance != null;
        KUB = GameManager.Instance.KUB.transform;
        staticPlatforms.Add(this);
        baseEulerAngles = transform.localEulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        if (!CameraControl.currentlyOrthographic) return;
        switch (axis)
        {
            case Axis.X: transform.localEulerAngles = baseEulerAngles + Vector3.forward * Vector3.Angle(KUB.up,Vector3.up); break;
            case Axis.Y: transform.localEulerAngles = baseEulerAngles + Vector3.forward * Vector3.Angle(KUB.forward,Vector3.forward); break;
            case Axis.Z: transform.localEulerAngles = baseEulerAngles + Vector3.forward * Vector3.Angle(KUB.right,Vector3.right); break;
        }
    }
}
