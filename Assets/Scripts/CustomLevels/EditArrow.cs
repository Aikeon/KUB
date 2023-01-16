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
    private int offset;
    private Vector3 baseParentPos;
    // Start is called before the first frame update
    void Start()
    {
        spr = GetComponent<SpriteRenderer>();
        spr.color = new Color(1,1,1,0.8f);
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
        spr.color = new Color(1,1,1,1);
        elementStartPos = (Yaxis ? currentTile.iPos : currentTile.jPos);
        startPos = Yaxis ? EditCustomLevel.Instance.currentIPos : EditCustomLevel.Instance.currentJPos;
        startScale = Yaxis ? transform.parent.localScale.y : transform.parent.localScale.x;
        baseParentPos = transform.parent.localPosition;
        startedWithShift = shifted;
    }

    void OnMouseDrag()
    {
        if (shifted != startedWithShift || nobugallowed)
        {
            nobugallowed = true;
            return;
        }
        if (shifted)
        {
            offset = (startPos - (Yaxis ? EditCustomLevel.Instance.currentIPos : EditCustomLevel.Instance.currentJPos));
            float newScale = Mathf.Max(startScale - (startPos - (Yaxis ? EditCustomLevel.Instance.currentIPos : EditCustomLevel.Instance.currentJPos)),1);
            //if (newScale != (!Yaxis ? transform.parent.localScale.x : transform.parent.localScale.y)) transform.parent.localPosition = baseParentPos - offset * (!Yaxis ? new Vector3(currentTile.grid.cellSize.x/2,0,0) : new Vector3(0,currentTile.grid.cellSize.y/2,0));
            transform.parent.localScale = !Yaxis ? new Vector3(newScale,transform.parent.localScale.y,transform.parent.localScale.z) : new Vector3(transform.parent.localScale.x,newScale,transform.parent.localScale.z);
            //transform.localScale = !Yaxis ? new Vector3(1/newScale/4,0.25f,0.25f) : new Vector3(0.25f,1/newScale,0.25f);
            transform.localPosition = !Yaxis ? new Vector3(2.75f/newScale,0,-0.55f) : new Vector3(0,2.75f/newScale,-0.55f);
        }
        else 
        {
            offset = (startPos - (Yaxis ? EditCustomLevel.Instance.currentIPos : EditCustomLevel.Instance.currentJPos));
            float newPos = Mathf.Clamp(elementStartPos - (startPos - (Yaxis ? EditCustomLevel.Instance.currentIPos : EditCustomLevel.Instance.currentJPos)) - currentTile.grid.tileColumnNumber/2,-currentTile.grid.tileColumnNumber/2,currentTile.grid.tileColumnNumber/2 - 1);
            newPos = (newPos + 0.5f) * (Yaxis ? currentTile.grid.cellSize.y : currentTile.grid.cellSize.x);
            transform.parent.localPosition = !Yaxis ? new Vector3(newPos, transform.parent.position.y, transform.parent.position.z) : new Vector3(transform.parent.position.x, newPos, transform.parent.position.z);
        }
    }

    //TODO Add origin point for placed platforms

    void OnMouseUp()
    {
        spr.color = new Color(1,1,1,0.8f);
        if (!nobugallowed && offset != 0)
        {
            if (!startedWithShift)
            {
                if (!currentTile.ChildTileChange(currentTile.grid.tiles[currentTile.grid.tileColumnNumber * Mathf.Clamp(currentTile.iPos - (Yaxis ? offset : 0),0,currentTile.grid.tileColumnNumber-1) + Mathf.Clamp(currentTile.jPos - ((!Yaxis) ? offset : 0),0,currentTile.grid.tileColumnNumber-1)]))
                {
                    float newPos = elementStartPos - currentTile.grid.tileColumnNumber/2 + 1;
                    newPos = (newPos - 0.5f) * (Yaxis ? currentTile.grid.cellSize.y : currentTile.grid.cellSize.x);
                    transform.parent.localPosition = !Yaxis ? new Vector3(newPos, transform.parent.position.y, transform.parent.position.z) : new Vector3(transform.parent.position.x, newPos, transform.parent.position.z);
                }
            }
        }
        nobugallowed = false;
    }
}
