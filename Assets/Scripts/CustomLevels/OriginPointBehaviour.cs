using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OriginPointBehaviour : MonoBehaviour
{
    private MeshRenderer origin;
    private TileBehaviour tile;
    // Start is called before the first frame update
    void Start()
    {
        tile = transform.parent.GetComponent<TileBehaviour>();
        origin = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        origin.enabled = EditCustomLevel.Instance.mode == 2 && EditCustomLevel.Instance.selectedTile == null && tile.active && tile.tileChild != null;
    }
}
