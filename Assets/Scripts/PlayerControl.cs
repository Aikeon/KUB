using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float jumpSpeed;
    [SerializeField] private float gravity;
    [SerializeField] private float maxFall;
    [SerializeField] private float maxHighJumpTime;
    [SerializeField] private Transform KUB;
    [SerializeField] private GameObject deathFX;
    public int additionalJumps;
    float inputH;
    float yVelocity;
    float highJumpTimer;
    int jumpsLeft;
    Vector3 basePos;
    Rigidbody rb;
    private Vector3 groundCheckScale;
    private Vector3 wallCheckScale;
    private Vector3 newVelocity;
    private bool isGrounded;
    private Collider[] colls;
    private bool hasPlayedSound;

    // Start is called before the first frame update
    void Start()
    {
        colls = new Collider[20];
        basePos = transform.position;
        rb = GetComponent<Rigidbody>();
        groundCheckScale = new Vector3(0.49995f,0.0005f,0.1f);
        wallCheckScale = new Vector3(0.0005f, 0.4995f, 0.1f);
        jumpsLeft = additionalJumps;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.pause) {rb.velocity = Vector3.zero; return;}

        if (Input.GetKey(InputManager.Instance.left))
        {
            inputH -= Time.deltaTime * 10;
        }
        if (Input.GetKey(InputManager.Instance.right))
        {
            inputH += Time.deltaTime * 10;
        }

        if (!Input.GetKey(InputManager.Instance.left) && !Input.GetKey(InputManager.Instance.right))
        {
            inputH = 0;
        }

        yVelocity -= Time.deltaTime * gravity;

        yVelocity = Mathf.Clamp(yVelocity, maxFall, Mathf.Infinity);

        if (Physics.CheckBox(rb.worldCenterOfMass - 0.505f * Vector3.up, groundCheckScale, Quaternion.identity, LayerMask.GetMask("Default"), QueryTriggerInteraction.Ignore))
        {
            yVelocity = 0;
            highJumpTimer = 0;
            ResetJumps();
            if (!isGrounded) GameManager.Instance.PlayTouch(1);
            isGrounded = true;
        } else 
        {
            if (isGrounded && yVelocity < 0)
            {
                isGrounded = false;
                highJumpTimer = Mathf.Infinity;
            }
        }

        if (Physics.CheckBox(rb.worldCenterOfMass - 0.505f * Vector3.right, wallCheckScale, Quaternion.identity, LayerMask.GetMask("Default"), QueryTriggerInteraction.Ignore))
        {
            inputH = Mathf.Clamp(inputH,0,1);
        }

        if (Physics.CheckBox(rb.worldCenterOfMass + 0.505f * Vector3.right, wallCheckScale, Quaternion.identity, LayerMask.GetMask("Default"), QueryTriggerInteraction.Ignore))
        {
            inputH = Mathf.Clamp(inputH,-1,0);
        }

        if (highJumpTimer > 0 && jumpsLeft > 0 && Input.GetKeyDown(InputManager.Instance.jump)) 
        {
            jumpsLeft--;
            highJumpTimer = 0;
        }

        if (Input.GetKey(InputManager.Instance.jump) && highJumpTimer < maxHighJumpTime)
        {
            if (!hasPlayedSound) 
            {
                GameManager.Instance.PlayTouch(0);
                hasPlayedSound = true;
            }
            yVelocity = jumpSpeed - Mathf.Pow(highJumpTimer,2.5f) * gravity;
            if (yVelocity < 0)
            {
                highJumpTimer = Mathf.Infinity;
                yVelocity = 0;
            }
            highJumpTimer += Time.deltaTime;
        }
        
        if (Input.GetKeyUp(InputManager.Instance.jump))
        {
            highJumpTimer = Mathf.Infinity;
            hasPlayedSound = false;
        }

        inputH = Mathf.Clamp(inputH,-1,1);

        newVelocity = new Vector3(inputH*speed, (Physics.CheckBox(rb.worldCenterOfMass + 0.505f * Vector3.up, groundCheckScale, Quaternion.identity, LayerMask.GetMask("Default"), QueryTriggerInteraction.Ignore) && yVelocity > 0) ? 0 : yVelocity, 0f);
        // rb.velocity = newVelocity;
    }

    void FixedUpdate()
    {
        if (GameManager.Instance.pause) {rb.velocity = Vector3.zero; return;}
        rb.velocity = newVelocity;
    }

    public void ResetJumps()
    {
        jumpsLeft = additionalJumps;
    }

    public void RegainJump()
    {
        jumpsLeft = additionalJumps + 1;
    }

    public void Die()
    {
        //TODO : Death anim, death marker ?
        Instantiate(deathFX,transform.position, transform.rotation);
        GetComponent<Renderer>().enabled = false;
        GameManager.Instance.PlaySound(4);
        if (DoorBehaviour.movement != null) {StopCoroutine(DoorBehaviour.movement); DoorBehaviour.movement = null;};
        StartCoroutine(death());
    }

    IEnumerator death()
    {
        GameManager.Instance.pause = true;
        yVelocity = 0;
        inputH = 0;
        yield return new WaitForSeconds(0.5f);
        var timeEllapsed = 0f;
        while (timeEllapsed < 1f)
        {
            KUB.rotation = Quaternion.Lerp(KUB.rotation, Quaternion.identity, timeEllapsed);
            timeEllapsed += Time.deltaTime;
            yield return null;
        }
        KUB.rotation = Quaternion.identity;
        transform.position = basePos;
        GetComponent<Renderer>().enabled = true;
        GameManager.Instance.pause = false;
    }
}
