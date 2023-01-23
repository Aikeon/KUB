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
        // if (tileChild != null)
        // {
        //     tileChild.transform.position = transform.position;
        //     tileChild.transform.rotation = transform.rotation;
        // }
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
                Debug.Log(tileChild.transform.localScale.y/grid.cellSize.y);
                xarr.transform.localPosition = new Vector3(2.75f/tileChild.transform.localScale.x,0,-0.55f);
                xarr.GetComponent<EditArrow>().currentTile = this;
                yarr = Instantiate(YArrowPrefab).GetComponent<EditArrow>(); 
                yarr.transform.SetParent(tileChild.transform); 
                yarr.transform.localPosition = new Vector3(0,2.75f/tileChild.transform.localScale.y,-0.55f);
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
                if (type == ObjectType.Player) EditCustomLevel.Instance.playerPlaced = false;
                if (tileChild != null) Destroy(tileChild);
                tileChild = null;
            }
            else 
            {
                if (t == ObjectType.Player) 
                {
                    if (EditCustomLevel.Instance.currentFace != Face.Zminus) {EditCustomLevel.Instance.ShowWarning("Player must be on front face !"); return;}
                    if (EditCustomLevel.Instance.playerPlaced) {EditCustomLevel.Instance.ShowWarning("Only one Player allowed !"); return;}
                    EditCustomLevel.Instance.playerPlaced = true;
                }
                var obj = Instantiate(elementPrefabs[(int)t - 1]);
                obj.TryGetComponent<PlayerControl>(out var pc);
                if (pc != null) pc.enabled = false;
                obj.TryGetComponent<DangerBehaviour>(out var db);
                if (db != null) db.enabled = false;
                obj.transform.SetParent(transform);
                obj.TryGetComponent<AntigravityBehaviour>(out var ab);
                if (ab != null)
                {
                    switch (EditCustomLevel.Instance.currentFace)
                    {
                        case Face.Zminus:
                        case Face.Zplus: ab.axis = AntigravityBehaviour.Axis.Z; break;
                        case Face.Xminus:
                        case Face.Xplus: ab.axis = AntigravityBehaviour.Axis.X; break;
                        case Face.Yminus:
                        case Face.Yplus: ab.axis = AntigravityBehaviour.Axis.Y; break;
                    }
                }
                obj.transform.localPosition = Vector3.zero;
                obj.transform.localScale = new Vector3(obj.transform.localScale.x * grid.cellSize.x, obj.transform.localScale.y * grid.cellSize.y, obj.transform.localScale.z);
                obj.transform.SetParent(transform.parent.parent);
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
