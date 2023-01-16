using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ObjectType
{
    None,
    Player,
    EndPoint,
    Platform,
    Danger,
    Static,
    JumpResetter
}

public enum Face
{
    Xplus,
    Xminus,
    Yplus,
    Yminus,
    Zplus,
    Zminus
}

public class Disposed
{
    public ObjectType type;
    public Vector3 position;
    public Vector3 rotation;
    public Vector3 scale;
    public Face face;
}

[System.Serializable]
public class LevelFile
{
    public string name;
    public List<Disposed> objects;
    public Image backgroundImage;
    public Color backgroundColor;
    public bool[] doorOpened;
    public bool[] faceReachable;
}