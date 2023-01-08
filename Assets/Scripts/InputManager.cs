using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    public enum Keys
    {
        Up,
        Down,
        Left,
        Right,
        Jump,
        Enter,
        Cancel,
        Zoom,
        CamUp,
        CamDown,
        CamLeft,
        CamRight,
        CamRotLeft,
        CamRotRight,
        A,
        B
    }

    public KeyCode jump = KeyCode.Space;
    public KeyCode left = KeyCode.Q;
    public KeyCode right = KeyCode.D;
    public KeyCode up = KeyCode.Z;
    public KeyCode down = KeyCode.S;
    public KeyCode enter = KeyCode.Return;
    public KeyCode cancel = KeyCode.Escape;
    public KeyCode zoom = KeyCode.LeftShift;
    public KeyCode cam_left = KeyCode.Q;
    public KeyCode cam_right = KeyCode.D;
    public KeyCode cam_up = KeyCode.Z;
    public KeyCode cam_down = KeyCode.S;
    public KeyCode cam_turn_left = KeyCode.A;
    public KeyCode cam_turn_right = KeyCode.E;
    public KeyCode b_key = KeyCode.B;
    public KeyCode a_key = KeyCode.A;
    //...

    void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public KeyCode convert(Keys key)
    {
        switch (key)
        {
            case Keys.Up : return up;
            case Keys.Down : return down;
            case Keys.Left : return left;
            case Keys.Right : return right;
            case Keys.Jump : return jump;
            case Keys.Enter : return enter;
            case Keys.Cancel : return cancel;
            case Keys.Zoom : return zoom;
            case Keys.CamUp : return cam_up;
            case Keys.CamDown : return cam_down;
            case Keys.CamLeft : return cam_left;
            case Keys.CamRight : return cam_right;
            case Keys.CamRotLeft : return cam_turn_left;
            case Keys.CamRotRight : return cam_turn_right;
            case Keys.A : return a_key;
            case Keys.B : return b_key;
            default : return KeyCode.None;
        }
    }
}
