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
    private Vector3 KUBLatestRight;
    private Vector3 KUBLatestUp;
    private Vector3 KUBLatestForward;
    private int moving;
    private float currentEA;
    static public List<AntigravityBehaviour> staticPlatforms = new List<AntigravityBehaviour>();
    // Start is called before the first frame update
    void Start()
    {
        staticPlatforms.Add(this);
        baseEulerAngles = transform.localEulerAngles;
        baseRot = transform.localRotation;
        KUBLatestRight = KUB.right;
        KUBLatestUp = KUB.up;
        KUBLatestForward = KUB.forward;
        moving = 0;
        currentEA = transform.localEulerAngles.z;
    }

    // Update is called once per frame
    void Update()
    {
        if (!CameraControl.currentlyOrthographic) return;
        if (KUBLatestForward != KUB.forward || KUBLatestRight != KUB.right || KUBLatestUp != KUB.up)
        {
            moving = -1;
            if (KUBLatestRight == KUB.right) moving = 1;
            if (KUBLatestUp == KUB.up) moving = 2;
            if (KUBLatestForward == KUB.forward) moving = 3;
        }
        if (moving != 0)
        {
            if (KUBLatestForward == KUB.forward && KUBLatestRight == KUB.right && KUBLatestUp == KUB.up)
            {
                moving = 0;
                currentEA = transform.localEulerAngles.z;
                transform.localEulerAngles = new Vector3(Mathf.RoundToInt(transform.localEulerAngles.x/90)*90,Mathf.RoundToInt(transform.localEulerAngles.y/90)*90,Mathf.RoundToInt(transform.localEulerAngles.z/90)*90);
            }
            if (moving == 1 && axis == Axis.X) 
            {
                transform.Rotate(90 * Vector3.forward * Time.unscaledDeltaTime, Space.Self);
            }
            if (moving == 2 && axis == Axis.Y) 
            {
                transform.Rotate(90 * Vector3.forward * Time.unscaledDeltaTime, Space.Self);
            }
            if (moving == 3 && axis == Axis.Z) 
            {
                transform.Rotate(90 * Vector3.forward * Time.unscaledDeltaTime, Space.Self);
            }
        }
        KUBLatestForward = KUB.forward;
        KUBLatestRight = KUB.right;
        KUBLatestUp = KUB.up;
    }
}
