using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditArrow : MonoBehaviour
{
    [SerializeField] private bool Yaxis;
    private bool shifted;
    private int startPos;
    private int elementStartPos;
    private float startScale;
    public TileBehaviour currentTile;
    private BoxCollider2D arrowCol;
    private SpriteRenderer spr;
    private bool startedWithShift;
    private bool nobugallowed;
    private float offset;
    private Vector3 startxarrPos;
    private Vector3 startyarrPos;
    private Vector3 baseParentPos;
    // Start is called before the first frame update
    void Start()
    {
        spr = GetComponent<SpriteRenderer>();
        spr.color = new Color(1,1,1,1f);
        arrowCol = GetComponent<BoxCollider2D>();
        transform.localScale = !Yaxis ? new Vector3(1/transform.parent.localScale.x/4,0.25f,0.25f) : new Vector3(0.25f,1/transform.parent.localScale.y/4,0.25f);
    }

    // Update is called once per frame
    void Update()
    {
        shifted = Input.GetKey(KeyCode.LeftShift); //TODO Change this for InputManager controls
        transform.localScale = new Vector3(1/transform.parent.localScale.x/4,1/transform.parent.localScale.y/4,0.25f);
        //arrowCol.enabled = Input.GetMouseButton(0);
    }

    void OnMouseDown()
    {
        spr.color = new Color(1,1,1,0.8f);
        elementStartPos = (Yaxis ? currentTile.iPos : currentTile.jPos);
        startPos = Yaxis ? EditCustomLevel.Instance.currentIPos : EditCustomLevel.Instance.currentJPos;
        startScale = Yaxis ? transform.parent.localScale.y : transform.parent.localScale.x;
        startxarrPos = currentTile.xarr.transform.localPosition;
        startyarrPos = currentTile.yarr.transform.localPosition;
        baseParentPos = transform.parent.localPosition;
        startedWithShift = shifted;
        currentTile.tileChild.TryGetComponent<AntigravityBehaviour>(out var ab);
        if (ab != null) ab.enabled = false;
    }

    void OnMouseDrag()
    {
        if (shifted != startedWithShift || nobugallowed)
        {
            nobugallowed = true;
            return;
        }
        if (shifted)// && currentTile.type != ObjectType.Player)
        {
            offset = (startPos - (Yaxis ? EditCustomLevel.Instance.currentIPos : EditCustomLevel.Instance.currentJPos)) * (Yaxis ? currentTile.grid.cellSize.y : currentTile.grid.cellSize.x);
            float newScale = Mathf.Max(startScale - offset,(Yaxis ? currentTile.grid.cellSize.y : currentTile.grid.cellSize.x));
            transform.parent.localScale = !Yaxis ? new Vector3(newScale,transform.parent.localScale.y,transform.parent.localScale.z) : new Vector3(transform.parent.localScale.x,newScale,transform.parent.localScale.z);
            float decPos = (Mathf.Clamp(elementStartPos - (newScale/(Yaxis ? currentTile.grid.cellSize.y : currentTile.grid.cellSize.x)+1)%2 / 2 - currentTile.grid.tileColumnNumber/2,-currentTile.grid.tileColumnNumber/2,currentTile.grid.tileColumnNumber/2 - 1) + 0.5f) * (Yaxis ? currentTile.grid.cellSize.y : currentTile.grid.cellSize.x);
            transform.parent.localPosition = !Yaxis ? new Vector3(decPos, transform.parent.position.y, 0) : new Vector3(transform.parent.position.x, decPos, 0);
            transform.localPosition = (!Yaxis ? new Vector3(2.75f/newScale,0,-0.55f) : new Vector3(0,2.75f/newScale,-0.55f));
            transform.localPosition = transform.localPosition + (newScale/(Yaxis ? currentTile.grid.cellSize.y : currentTile.grid.cellSize.x)+1)%2 / 2 * (Yaxis ? currentTile.grid.cellSize.y : currentTile.grid.cellSize.x) / newScale * (Yaxis ? Vector3.up : Vector3.right);
            if (Yaxis) currentTile.xarr.transform.localPosition = startxarrPos + (newScale/currentTile.grid.cellSize.y+1)%2 / 2 * currentTile.grid.cellSize.y / newScale * Vector3.up;
            else currentTile.yarr.transform.localPosition = startyarrPos + (newScale/currentTile.grid.cellSize.x+1)%2 / 2 * currentTile.grid.cellSize.x / newScale * Vector3.right;
        }
        else 
        {
            offset = (startPos - (Yaxis ? EditCustomLevel.Instance.currentIPos : EditCustomLevel.Instance.currentJPos));
            float newPos = Mathf.Clamp(elementStartPos + (startScale/(Yaxis ? currentTile.grid.cellSize.y : currentTile.grid.cellSize.x)+1)%2 / 2 - offset - currentTile.grid.tileColumnNumber/2,-currentTile.grid.tileColumnNumber/2,currentTile.grid.tileColumnNumber/2 - 1);
            newPos = (newPos - 0.5f) * (Yaxis ? currentTile.grid.cellSize.y : currentTile.grid.cellSize.x);
            transform.parent.localPosition = !Yaxis ? new Vector3(newPos, transform.parent.position.y, 0) : new Vector3(transform.parent.position.x, newPos, 0);
        }
    }

    void OnMouseUp()
    {
        spr.color = new Color(1,1,1,1f);
        if (!nobugallowed && offset != 0)
        {
            if (!startedWithShift)
            {
                var intOffset = (int)offset;
                if (!currentTile.ChildTileChange(currentTile.grid.tiles[currentTile.grid.tileColumnNumber * Mathf.Clamp(currentTile.iPos - (Yaxis ? intOffset : 0),0,currentTile.grid.tileColumnNumber-1) + Mathf.Clamp(currentTile.jPos - ((!Yaxis) ? intOffset : 0),0,currentTile.grid.tileColumnNumber-1)]))
                {
                    float newPos = elementStartPos - currentTile.grid.tileColumnNumber/2;
                    newPos = (newPos - 0.5f) * (Yaxis ? currentTile.grid.cellSize.y : currentTile.grid.cellSize.x);
                    transform.parent.localPosition = !Yaxis ? new Vector3(newPos, transform.parent.position.y, transform.parent.position.z) : new Vector3(transform.parent.position.x, newPos, transform.parent.position.z);
                }
            }
        }
        nobugallowed = false;
        currentTile.tileChild.TryGetComponent<AntigravityBehaviour>(out var ab);
        if (ab != null) ab.enabled = true;
    }
}
