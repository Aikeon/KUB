using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private GameManagement eventManager;
    [SerializeField] private float jumpForce;
    [SerializeField] private float speed;
    [SerializeField] private float gravity;
    [SerializeField] private LayerMask platformLayer;
    [SerializeField] private BoxCollider2D leftExit;
    [SerializeField] private BoxCollider2D upExit;
    [SerializeField] private BoxCollider2D rightExit;
    [SerializeField] private BoxCollider2D downExit;

    private float inputH;
    private float inputV;
    private Rigidbody2D rb;
    private CapsuleCollider2D boxcol;
    private bool onGround;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        boxcol = GetComponent<CapsuleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
        inputH = Input.GetAxis("Horizontal");
        inputV -= gravity*Time.deltaTime;

        onGround = isGrounded();

        if (onGround || hitsCeiling() && inputV > 0){
            inputV = 0f;
        }

        if (Input.GetKeyDown(KeyCode.Space) && onGround){
            inputV = jumpForce;
        }

        rb.velocity = new Vector2(inputH*speed,inputV);
    }

    private bool isGrounded(){
        RaycastHit2D hit1 = Physics2D.Raycast(boxcol.bounds.center - new Vector3(boxcol.bounds.extents.x,0,0)/2, Vector2.down, boxcol.bounds.extents.y + 0.02f, platformLayer);
        RaycastHit2D hit2 = Physics2D.Raycast(boxcol.bounds.center + new Vector3(boxcol.bounds.extents.x,0,0)/2, Vector2.down, boxcol.bounds.extents.y + 0.02f, platformLayer);
        Color test1 = Color.green;
        Color test2 = Color.green;
        if (hit1.collider == null)
            test1 = Color.red;
        if (hit2.collider == null)
            test2 = Color.red;
        Debug.DrawRay(boxcol.bounds.center - new Vector3(boxcol.bounds.extents.x,0,0)/2, Vector2.down * (boxcol.bounds.extents.y + 0.2f),test1);
        Debug.DrawRay(boxcol.bounds.center + new Vector3(boxcol.bounds.extents.x,0,0)/2, Vector2.down * (boxcol.bounds.extents.y + 0.2f),test2);
        return hit1.collider != null || hit2.collider != null;
    }

    private bool hitsCeiling(){
        RaycastHit2D hit1 = Physics2D.Raycast(boxcol.bounds.center - new Vector3(boxcol.bounds.extents.x,0,0)/2, Vector2.up, boxcol.bounds.extents.y + 0.02f, platformLayer);
        RaycastHit2D hit2 = Physics2D.Raycast(boxcol.bounds.center + new Vector3(boxcol.bounds.extents.x,0,0)/2, Vector2.up, boxcol.bounds.extents.y + 0.02f, platformLayer);
        Color test1 = Color.green;
        Color test2 = Color.green;
        if (hit1.collider == null)
            test1 = Color.red;
        if (hit2.collider == null)
            test2 = Color.red;
        Debug.DrawRay(boxcol.bounds.center - new Vector3(boxcol.bounds.extents.x,0,0)/2, Vector2.up * (boxcol.bounds.extents.y + 0.2f),test1);
        Debug.DrawRay(boxcol.bounds.center + new Vector3(boxcol.bounds.extents.x,0,0)/2, Vector2.up * (boxcol.bounds.extents.y + 0.2f),test2);
        return hit1.collider != null || hit2.collider != null;
    }

    void OnCollisionEnter(Collider col){
        if (col == rightExit){
            eventManager.moveRight();
            Debug.Log("moved right");
        }
    }
}
