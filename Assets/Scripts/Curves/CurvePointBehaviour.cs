using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurvePointBehaviour : MonoBehaviour
{
    [SerializeField] private Transform parent;
    [SerializeField] private float variationSpeed;
    [SerializeField] private float maxVarSpeed;
    [SerializeField] private Vector2 limits;
    private float currentVar;
    private float Ycoord;

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.pauseOption != -1) return;
        float addition = Random.Range(-variationSpeed,variationSpeed);
        if (currentVar + transform.position.y > limits.y + ((parent != null) ? parent.position.y : 0))
        {
            addition = Mathf.Clamp(addition,addition,0);
        }
        if (currentVar + transform.position.y < limits.x + ((parent != null) ? parent.position.y : 0))
        {
            addition = Mathf.Clamp(addition,0,addition);
        }
        currentVar += addition;
        currentVar = Mathf.Clamp(currentVar,-maxVarSpeed,maxVarSpeed);
        Ycoord += currentVar * Time.deltaTime;
        transform.position = new Vector3(transform.position.x, (Ycoord + ((parent != null) ? parent.position.y : 0)) , transform.position.z);
    }
}
