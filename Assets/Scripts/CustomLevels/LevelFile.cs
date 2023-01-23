using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
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

[System.Serializable]
public enum Face
{
    Zminus,
    Xplus,
    Zplus,
    Xminus,
    Yplus,
    Yminus
}

[System.Serializable]
public class Disposed
{
    public ObjectType type;
    public float posX;
    public float posY;
    public float posZ;
    public int iPos;
    public int jPos;
    public float scaleX;
    public float scaleY;
    public float scaleZ;
    public Face face;
}

[System.Serializable]
public class LevelFile
{
    public string name;
    public List<Disposed> objects;
    public float backgroundR;
    public float backgroundG;
    public float backgroundB;
    public bool[] doorOpened;
    public bool[] faceReachable;
}