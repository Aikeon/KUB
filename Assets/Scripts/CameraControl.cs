using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    Camera cam;
    public static Coroutine movement;
    private Quaternion KUBBaseRotation;
    [SerializeField] float rotationSpeed = 100f;
    [SerializeField] bool fixedRotation = false;
    [SerializeField] bool baseOrtho = true;


    public float ProjectionChangeTime = 0.5f;
    public static bool _changing = false;
    private bool canRotate = false;
    public static bool currentlyOrthographic;
    private Matrix4x4 persMat;
    private Matrix4x4 orthoMat;
    private float inputH;
    private float inputV;
    private float inputZ;
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        _changing = false;

        movement = null;
        
        if (baseOrtho)
        {
            persMat = cam.projectionMatrix;
 
            cam.orthographic = true;
            cam.ResetProjectionMatrix();
            orthoMat = cam.projectionMatrix;
            currentlyOrthographic = true;
        } 
        else
        {
            cam.orthographic = true;
            cam.ResetProjectionMatrix();
            orthoMat = cam.projectionMatrix;

            cam.orthographic = false;
            cam.ResetProjectionMatrix();
            persMat = cam.projectionMatrix;

            currentlyOrthographic = false; 
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (fixedRotation || GameManager.Instance.pauseOption != -1 || DoorBehaviour.movement != null) return;

        if (Input.GetKeyDown(InputManager.Instance.zoom) && !_changing && movement == null && DoorBehaviour.movement == null)
        {
            _changing = true;
            movement = StartCoroutine(zoom(orthoMat, persMat));
        }

        if (!canRotate || GameManager.Instance.pauseOption != -1) return;

        inputH = (Mathf.Abs(Input.GetAxis("Horizontal")) > Mathf.Abs((Input.GetKey(InputManager.Instance.cam_left) ? -1 : 0) + (Input.GetKey(InputManager.Instance.cam_right) ? 1 : 0))) ? Input.GetAxis("Horizontal")/10 : (Input.GetKey(InputManager.Instance.cam_left) ? -1 : 0) + (Input.GetKey(InputManager.Instance.cam_right) ? 1 : 0);
        inputV = (Mathf.Abs(Input.GetAxis("Vertical")) > Mathf.Abs((Input.GetKey(InputManager.Instance.cam_down) ? -1 : 0) + (Input.GetKey(InputManager.Instance.cam_up) ? 1 : 0))) ? Input.GetAxis("Vertical")/10 : (Input.GetKey(InputManager.Instance.cam_down) ? -1 : 0) + (Input.GetKey(InputManager.Instance.cam_up) ? 1 : 0);
        inputZ = (Input.GetKey(InputManager.Instance.cam_turn_left) ? -1 : 0) + (Input.GetKey(InputManager.Instance.cam_turn_right) ? 1 : 0);
        // transform.Rotate(rotationSpeed * Time.deltaTime * new Vector3(inputV, -inputH, inputZ), Space.World);  
    }

    void FixedUpdate()
    {
        if (!canRotate || GameManager.Instance.pauseOption != -1 || DoorBehaviour.movement != null) return;
        transform.Rotate(rotationSpeed * Time.fixedDeltaTime * new Vector3(inputV, -inputH, inputZ), Space.World);  
    }

    public void changePerspective()
    {
        if (!_changing && movement == null)
        {
            _changing = true;
            movement = StartCoroutine(zoom(orthoMat, persMat));
        }
    }

    IEnumerator zoom(Matrix4x4 orthoMat, Matrix4x4 persMat)
    {
        canRotate = !canRotate;
        var timeEllapsedReturn = 0f;
        if (!currentlyOrthographic && !fixedRotation)
        {
            while (timeEllapsedReturn < ProjectionChangeTime)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, KUBBaseRotation, timeEllapsedReturn);
                timeEllapsedReturn += Time.deltaTime;
                yield return null;
            }
        } else 
        {
            if (!fixedRotation) GameManager.Instance.pause = true;
            KUBBaseRotation = transform.rotation;
        }
        if (currentlyOrthographic) cam.orthographic = false;
        var timeEllapsed = 0f;
        while (timeEllapsed < ProjectionChangeTime)
        {
            if(currentlyOrthographic)
            {
                cam.projectionMatrix = MatrixLerp(orthoMat, persMat, (timeEllapsed/ProjectionChangeTime) * (timeEllapsed/ProjectionChangeTime));
            }
            else
            {
                cam.projectionMatrix = MatrixLerp(persMat, orthoMat, Mathf.Pow(timeEllapsed/ProjectionChangeTime, 0.25f));
            }
            timeEllapsed += Time.deltaTime;
            yield return null;
        }
        _changing = false;
        cam.orthographic = !currentlyOrthographic;
        cam.ResetProjectionMatrix();
        currentlyOrthographic = !currentlyOrthographic;
        if (currentlyOrthographic && !fixedRotation) GameManager.Instance.pause = false;
        movement = null;
    }

    private Matrix4x4 MatrixLerp(Matrix4x4 from, Matrix4x4 to, float t)
    {
        t = Mathf.Clamp(t, 0.0f, 1.0f);
        var newMatrix = new Matrix4x4();
        newMatrix.SetRow(0, Vector4.Lerp(from.GetRow(0), to.GetRow(0), t));
        newMatrix.SetRow(1, Vector4.Lerp(from.GetRow(1), to.GetRow(1), t));
        newMatrix.SetRow(2, Vector4.Lerp(from.GetRow(2), to.GetRow(2), t));
        newMatrix.SetRow(3, Vector4.Lerp(from.GetRow(3), to.GetRow(3), t));
        return newMatrix;
    }
}
