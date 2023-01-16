using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridLineBehaviour : MonoBehaviour
{
    private Transform parent;
    public Vector3 point1;
    public Vector3 point2;
    private LineRenderer lr;
    // Start is called before the first frame update
    void Start()
    {
        lr = GetComponent<LineRenderer>();
        parent = transform.parent;
    }

    // Update is called once per frame
    void Update()
    {
        lr.SetPosition(0,parent.position + parent.rotation * point1);
        lr.SetPosition(1,parent.position + parent.rotation * point2);
    }
}
