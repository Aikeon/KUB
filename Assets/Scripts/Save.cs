using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Save
{
    public List<int> levelsState = new List<int>();
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
    public int musicVolume;
    public int soundVolume;
    public bool discovered;
    public bool konamiCoded;
}
