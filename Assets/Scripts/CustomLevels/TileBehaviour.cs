using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBehaviour : MonoBehaviour
{
    public ObjectType type = ObjectType.None;
    public GameObject tileChild;
    public GridManager grid;
    [SerializeField] GameObject[] elementPrefabs;
    [SerializeField] GameObject XArrowPrefab;
    [SerializeField] GameObject YArrowPrefab;
    public int iPos;
    public int jPos;
    public EditArrow xarr;
    public EditArrow yarr;
    public bool active;

    void Update()
    {
        if (tileChild != null && !grid.isFrontGrid)
        {
            tileChild.transform.position = transform.position;
            tileChild.transform.rotation = transform.rotation;
        }
    }

    void OnMouseDown()
    {
        if (!active) return;
        switch (EditCustomLevel.Instance.mode)
        {
            case 0: break;
            case 1: SetTile(EditCustomLevel.Instance.currentBrush); break;
            case 2:
            if (EditCustomLevel.Instance.selectedTile != this && EditCustomLevel.Instance.selectedTile != null)
            {
                EditCustomLevel.Instance.selectedTile.DeselectTile();
            }
            if (tileChild != null && type != ObjectType.None && EditCustomLevel.Instance.selectedTile != this) 
            {
                xarr = Instantiate(XArrowPrefab).GetComponent<EditArrow>(); 
                xarr.transform.SetParent(tileChild.transform); 
                xarr.transform.localPosition = new Vector3(2.75f,0,-0.55f);
                xarr.GetComponent<EditArrow>().currentTile = this;
                yarr = Instantiate(YArrowPrefab).GetComponent<EditArrow>(); 
                yarr.transform.SetParent(tileChild.transform); 
                yarr.transform.localPosition = new Vector3(0,2.75f,-0.55f);
                yarr.GetComponent<EditArrow>().currentTile = this;
                EditCustomLevel.Instance.selectedTile = this;
            }
            break;
        }
        
    }

    void OnMouseEnter()
    {
        if (!active) return;
        if (EditCustomLevel.Instance.placing && EditCustomLevel.Instance.mode == 1) SetTile(EditCustomLevel.Instance.currentBrush);
    }

    public void SetTile(ObjectType t)
    {
        if (type != t)
        {
            if (t == ObjectType.None)
            {
                if (tileChild != null) Destroy(tileChild);
                tileChild = null;
            }
            else 
            {
                var obj = Instantiate(elementPrefabs[(int)t - 1]);
                obj.transform.SetParent(transform);
                obj.transform.localPosition = Vector3.zero;
                obj.transform.localScale = new Vector3(obj.transform.localScale.x * grid.cellSize.x, obj.transform.localScale.y * grid.cellSize.y, obj.transform.localScale.z);
                obj.transform.SetParent(null);
                tileChild = obj;
            }
            type = t;
        }
    }

    public bool ChildTileChange(TileBehaviour newtile)
    {
        if (this == newtile) return true;
        if (newtile.tileChild != null) return false;
        Debug.Log("swapped (" + jPos + "," + iPos + ") with (" + newtile.jPos + "," + newtile.iPos + ")");
        newtile.tileChild = tileChild;
        newtile.type = type;
        xarr.currentTile = newtile;
        yarr.currentTile = newtile;
        EditCustomLevel.Instance.selectedTile = newtile;
        newtile.xarr = xarr;
        newtile.yarr = yarr;
        xarr = null;
        yarr = null;
        tileChild = null;
        type = ObjectType.None;
        return true;
    }

    public void DeselectTile()
    {
        Destroy(xarr.gameObject);
        Destroy(yarr.gameObject);
        xarr = null; yarr = null;
        EditCustomLevel.Instance.selectedTile = null;
    }
}
