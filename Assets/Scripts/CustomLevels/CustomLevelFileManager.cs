using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using SimpleFileBrowser;
using UnityEngine.SceneManagement;
using UnityEngine.ResourceManagement.AsyncOperations;

public class CustomLevelFileManager : MonoBehaviour
{
    static private LevelFile CreateLevelFile(string name)
    {
        LevelFile file = new LevelFile();

        file.backgroundR = EditCustomLevel.Instance.backgroundImage.color.r;
        file.backgroundG = EditCustomLevel.Instance.backgroundImage.color.g;
        file.backgroundB = EditCustomLevel.Instance.backgroundImage.color.b;
        file.doorOpened = new bool[12];
        for (int k = 0; k < 12; k++)
        {
            file.doorOpened[k] = !EditCustomLevel.Instance.doors[k].activeSelf;
        }
        file.faceReachable = new bool[6];
        for (int k = 0; k < 6; k++)
        {
            file.faceReachable[k] = (EditCustomLevel.Instance.faces[k] != null) ? !EditCustomLevel.Instance.faces[k].activeSelf : true;
        }
        file.name = name;
        file.objects = new List<Disposed>();
        for (int k = 0; k < 6; k++)
        {
            for (int i = 0; i < EditCustomLevel.Instance.grids[k].tileColumnNumber; i++)
            {
                for (int j = 0; j < EditCustomLevel.Instance.grids[k].tileColumnNumber; j++)
                {
                    TileBehaviour tile = EditCustomLevel.Instance.grids[k].tiles[i*EditCustomLevel.Instance.grids[k].tileColumnNumber + j];
                    if (tile.tileChild != null)
                    {
                        Disposed obj = new Disposed();
                        obj.posX = tile.tileChild.transform.localPosition.x;
                        obj.posY = tile.tileChild.transform.localPosition.y;
                        obj.posZ = tile.tileChild.transform.localPosition.z;
                        obj.scaleX = tile.tileChild.transform.localScale.x;
                        obj.scaleY = tile.tileChild.transform.localScale.y;
                        obj.scaleZ = tile.tileChild.transform.localScale.z;
                        obj.iPos = tile.iPos;
                        obj.jPos = tile.jPos;
                        obj.type = tile.type;
                        obj.face = (Face)k;
                        file.objects.Add(obj);
                    }
                }
            }
        }

        return file;
    }

    static public void SaveLevel(string path)
    {
        LevelFile levelfile = CreateLevelFile(string.Format("*/$s.KUB"));

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(path);
        Debug.Log(Application.persistentDataPath);
        bf.Serialize(file, levelfile);
        file.Close();

        Debug.Log("Game Saved");
    }

    static public void LoadLevel(string path, bool inEditor)
    { 
        // 1
        if (File.Exists(path))
        {
            // 2
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(path, FileMode.Open);
            LevelFile levelfile = (LevelFile)bf.Deserialize(file);
            file.Close();

            if (inEditor)
            {
                EditCustomLevel.Instance.KUB.rotation = Quaternion.identity;
                for (int k = 0; k < 6; k++)
                {
                    EditCustomLevel.Instance.grids[k].ClearGrid();
                    EditCustomLevel.Instance.grids[k].GenerateGrid();
                    EditCustomLevel.Instance.ChangeReachableOrOpenState(k,true,levelfile.faceReachable[k]);
                    EditCustomLevel.Instance.grids[k].GridActive(k == 0);                
                }
                for (int k = 0; k < 12; k++)
                {
                    EditCustomLevel.Instance.ChangeReachableOrOpenState(k+6,true,levelfile.doorOpened[k]);
                }
                EditCustomLevel.Instance.backgroundImage.color = new Color(levelfile.backgroundR,levelfile.backgroundG,levelfile.backgroundB,1);
                foreach (Disposed obj in levelfile.objects)
                {
                    var g = EditCustomLevel.Instance.grids[(int)obj.face];
                    g.tiles[obj.iPos * g.tileColumnNumber + obj.jPos].SetTile(obj.type);
                    g.tiles[obj.iPos * g.tileColumnNumber + obj.jPos].tileChild.transform.localRotation = Quaternion.identity;
                    g.tiles[obj.iPos * g.tileColumnNumber + obj.jPos].tileChild.transform.localScale = new Vector3(obj.scaleX,obj.scaleY,obj.scaleZ);
                }
            }
            else
            {
                Transform faceFolder = GameManager.Instance.KUB.transform.GetChild(1);
                Transform doorFolder = GameManager.Instance.KUB.transform.GetChild(3);
                faceFolder.GetChild(0).GetChild(0).gameObject.SetActive(!levelfile.faceReachable[1]);
                faceFolder.GetChild(1).GetChild(0).gameObject.SetActive(!levelfile.faceReachable[3]);
                faceFolder.GetChild(2).GetChild(0).gameObject.SetActive(!levelfile.faceReachable[4]);
                faceFolder.GetChild(3).GetChild(0).gameObject.SetActive(!levelfile.faceReachable[5]);
                faceFolder.GetChild(4).GetChild(0).gameObject.SetActive(!levelfile.faceReachable[2]);
                for (int k = 0; k < 12; k++)
                {
                    doorFolder.GetChild(k).gameObject.SetActive(!levelfile.doorOpened[k]);
                }
                GameObject.Find("Background").transform.GetChild(0).GetChild(0).GetComponent<Image>().color = new Color(levelfile.backgroundR,levelfile.backgroundG,levelfile.backgroundB,1);
                foreach (Disposed obj in levelfile.objects)
                {
                    string objname = "";
                    switch (obj.type)
                    {
                        case ObjectType.Player : objname = "Player"; break;
                        case ObjectType.Danger : objname = "Danger"; break;
                        case ObjectType.EndPoint : objname = "EndPoint"; break;
                        case ObjectType.JumpResetter : objname = "JumpResetter"; break;
                        case ObjectType.Platform : objname = "Platform"; break;
                        case ObjectType.Static : objname = "Static"; break;
                    }
                    if (objname != "Player")
                    {
                        GameObject oNonInst = Addressables.LoadAssetAsync<GameObject>(objname).WaitForCompletion();
                        var o = Instantiate(oNonInst);
                        int faceInt = 0;
                        switch (obj.face)
                        {
                            case Face.Zminus: faceInt = 5; break;
                            case Face.Zplus: faceInt = 4; break;
                            case Face.Xminus: faceInt = 1; break;
                            case Face.Xplus: faceInt = 0; break;
                            case Face.Yminus: faceInt = 3; break;
                            case Face.Yplus: faceInt = 2; break;
                        }
                        o.transform.SetParent(faceFolder.GetChild(faceInt));
                        o.transform.localPosition = new Vector3(obj.posX,obj.posY,obj.posZ);
                        o.transform.localRotation = Quaternion.identity;
                        o.transform.localScale = new Vector3(obj.scaleX,obj.scaleY,obj.scaleZ);
                    }
                    else 
                    {
                        GameManager.Instance.Player.transform.SetParent(faceFolder.GetChild(5));
                        GameManager.Instance.Player.transform.localPosition = new Vector3(obj.posX,obj.posY,obj.posZ);
                        GameManager.Instance.Player.transform.SetParent(faceFolder.parent);
                        GameManager.Instance.Player.transform.localRotation = Quaternion.identity;
                        GameManager.Instance.Player.transform.localScale = new Vector3(obj.scaleX,obj.scaleY,obj.scaleZ);
                        GameManager.Instance.Player.ResetCheckBoxes();
                        GameManager.Instance.Player.gameObject.SetActive(true);
                    }
                }
            }

            // Resets menu display

            Debug.Log("Save Loaded");
        }
        else
        {
            Debug.Log("Not a valid save path");
        }
    }

    static public IEnumerator WaitForSelectionInExplorer(bool save,bool inEditor)
    {
        var path = "";
        Cursor.lockState = CursorLockMode.None;
        FileBrowser.SetFilters(true,".kub");
        if (save) FileBrowser.ShowSaveDialog(null,null,FileBrowser.PickMode.Files); else FileBrowser.ShowLoadDialog(null,null,FileBrowser.PickMode.Files);
        while (FileBrowser.IsOpen)
        {
            yield return null;
        }
        if (FileBrowser.Success)
        {
            path = FileBrowser.Result[0];
            Debug.Log("Path : " + path);
            if (save) SaveLevel(path); 
            else 
            {
                if (inEditor) LoadLevel(path,true);
                else 
                {
                    SceneManager.LoadScene("CustomLevel");
                }
            }
        }
        if (!inEditor) Cursor.lockState = CursorLockMode.Locked;
    }
}
