using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class EditCustomLevel : MonoBehaviour
{
    public static EditCustomLevel Instance;
    [SerializeField] private GameObject placingMenu;
    [SerializeField] private GameObject editionMenu;
    [SerializeField] private GameObject facesMenu;
    public GameObject[] elementPrefabs;
    public Image backgroundImage; //TODO Make a menu to set background color
    public GameObject[] faces;
    public GridManager[] grids;
    public GameObject[] doors;
    [SerializeField] private Image[] buttons;
    [SerializeField] private TextMeshProUGUI warningText;
    public Transform KUB;
    public ObjectType currentBrush;
    public bool placing;
    public bool playerPlaced;
    public int mode; //0 for nothing, 1 for placing, 2 for editing
    public int currentIPos;
    public int currentJPos;
    public TileBehaviour selectedTile;
    public Face currentFace;
    private int currentFaceRot;
    private Coroutine move;
    private Coroutine warning;
    [SerializeField] private GameObject exitMenu;
    private bool showingExitMenu;
    private bool willExit;
    void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        Cursor.lockState = CursorLockMode.None;
    }

    // Start is called before the first frame update
    void Start()
    {
        currentFace = Face.Zminus;
        currentFaceRot = 0;
        mode = 0;
        selectedTile = null;
        grids[0].GridActive(true);
        showingExitMenu = false;
        ChangeReachableOrOpenState(0,true,true);
        ChangeReachableOrOpenState(1,true,true);
        ChangeReachableOrOpenState(2,true,true);
        ChangeReachableOrOpenState(3,true,true);
        ChangeReachableOrOpenState(4,true,true);
        ChangeReachableOrOpenState(5,true,true);
    }

    // Update is called once per frame
    void Update()
    {
        if (SimpleFileBrowser.FileBrowser.IsOpen) return;
        if (Input.GetKeyDown(InputManager.Instance.cancel))
        {
            showingExitMenu = !showingExitMenu;
            exitMenu.SetActive(!exitMenu.activeSelf);
            willExit = false;
        }
        if (showingExitMenu)
        {
            placing = false;
            if (Input.GetKeyDown(InputManager.Instance.left) || Input.GetKeyDown(InputManager.Instance.right))
            {
                willExit = !willExit;
                exitMenu.transform.GetChild(2).GetComponent<TextMeshProUGUI>().color = (willExit ? Color.yellow : Color.white);
                exitMenu.transform.GetChild(3).GetComponent<TextMeshProUGUI>().color = (willExit ? Color.white : Color.yellow);
            }
            if (Input.GetKeyDown(InputManager.Instance.enter))
            {
                if (willExit) SceneManager.LoadScene("TitleScreen");
                else {showingExitMenu = !showingExitMenu; exitMenu.SetActive(!exitMenu.activeSelf); willExit = false;}
            }
        }
        else 
        {
            placing = Input.GetMouseButton(0); //TODO change for InputManager settings
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var hits = Physics2D.RaycastAll(ray.origin, ray.direction, 50f);
            Debug.DrawRay(ray.origin,ray.origin + ray.direction * 500f,Color.blue,5f);
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider.gameObject.TryGetComponent<TileBehaviour>(out var tile))
                {
                    currentIPos = tile.iPos;
                    currentJPos = tile.jPos;
                }
            }
            if (Input.GetKeyDown(InputManager.Instance.cam_left))
            {
                MoveKUB(2);
            }
            if (Input.GetKeyDown(InputManager.Instance.cam_right))
            {
                MoveKUB(3);
            }
            if (Input.GetKeyDown(InputManager.Instance.cam_up))
            {
                MoveKUB(0);
            }
            if (Input.GetKeyDown(InputManager.Instance.cam_down))
            {
                MoveKUB(1);
            }
            if (Input.GetKeyDown(InputManager.Instance.cam_turn_left))
            {
                MoveKUB(4);
            }
            if (Input.GetKeyDown(InputManager.Instance.cam_turn_right))
            {
                MoveKUB(5);
            }
        }
    }

    void OnDestroy()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ChangeMode(int newmode)
    {
        if (mode == newmode) mode = 0; else mode = newmode;
        placingMenu.SetActive(mode == 1);
        editionMenu.SetActive(mode == 2);
        facesMenu.SetActive(mode == 3);
        if (mode != 2)
        {
            if (selectedTile != null)
            {
                if (selectedTile.xarr != null) Destroy(selectedTile.xarr.gameObject);
                if (selectedTile.yarr != null) Destroy(selectedTile.yarr.gameObject);
            }
            selectedTile = null;
        }
    }

    public void ChangeBrush(int t)
    {
        currentBrush = (ObjectType)t;
    }

    public void ButtonFaceDoorState(int i)
    {
        ChangeReachableOrOpenState(i);
    }

    public void ChangeReachableOrOpenState(int i, bool giveState = false, bool givenState = false)
    {
        if (i < 6)
        {
            if (i != 0) faces[i].SetActive((giveState) ? !givenState : !faces[i].activeSelf);
            grids[i].gameObject.SetActive((giveState) ? givenState : !grids[i].gameObject.activeSelf);
            buttons[i].color = ((i != 0) ? (faces[i].activeSelf) : false) ? Color.gray : Color.white;
        }
        else 
        {
            doors[i-6].SetActive((giveState) ? !givenState : !doors[i-6].activeSelf);
            buttons[i].color = (doors[i-6].activeSelf) ? new Color(0.2f,0.2f,0.2f,1f) : new Color(0.7f,0.7f,0.7f,1f);
        }
    }

    public void MoveKUB(int dir)
    {
        if (move != null) return;
        switch (dir)
        {
            case 0: move = StartCoroutine(goUp()); break;
            case 1: move = StartCoroutine(goDown()); break;
            case 2: move = StartCoroutine(goLeft()); break;
            case 3: move = StartCoroutine(goRight()); break;
            case 4: move = StartCoroutine(turnLeft()); break;
            case 5: move = StartCoroutine(turnRight()); break;
        }
    }

    IEnumerator goUp()
    {
        var timeEllapsed = 0f;
        grids[(int)currentFace].GridActive(false);
        currentFace = getNextFace(currentFace, currentFaceRot, 0);
        var nextRot = KUB.rotation * Quaternion.Euler(90,0,0);
        while (timeEllapsed < 0.25f)
        {
            KUB.Rotate(90 * 4 * Vector3.left * Time.deltaTime, Space.World);
            timeEllapsed += Time.deltaTime;
            yield return null;
        }
        KUB.eulerAngles = new Vector3(Mathf.RoundToInt(KUB.eulerAngles.x/90)*90,Mathf.RoundToInt(KUB.eulerAngles.y/90)*90,Mathf.RoundToInt(KUB.eulerAngles.z/90)*90);
        move = null;
        grids[(int)currentFace].GridActive(true);
    }

    IEnumerator goDown()
    {
        var timeEllapsed = 0f;
        grids[(int)currentFace].GridActive(false);
        currentFace = getNextFace(currentFace, currentFaceRot, 180);
        while (timeEllapsed < 0.25f)
        {
            KUB.Rotate(-90 * 4 * Vector3.left * Time.deltaTime, Space.World);
            timeEllapsed += Time.deltaTime;
            yield return null;
        }
        KUB.eulerAngles = new Vector3(Mathf.RoundToInt(KUB.eulerAngles.x/90)*90,Mathf.RoundToInt(KUB.eulerAngles.y/90)*90,Mathf.RoundToInt(KUB.eulerAngles.z/90)*90);
        move = null;
        grids[(int)currentFace].GridActive(true);
    }

    IEnumerator goLeft()
    {
        var timeEllapsed = 0f;
        grids[(int)currentFace].GridActive(false);
        currentFace = getNextFace(currentFace, currentFaceRot, 270);
        while (timeEllapsed < 0.25f)
        {
            KUB.Rotate(-90 * 4 * Vector3.up * Time.deltaTime, Space.World);
            timeEllapsed += Time.deltaTime;
            yield return null;
        }
        KUB.eulerAngles = new Vector3(Mathf.RoundToInt(KUB.eulerAngles.x/90)*90,Mathf.RoundToInt(KUB.eulerAngles.y/90)*90,Mathf.RoundToInt(KUB.eulerAngles.z/90)*90);
        move = null;
        grids[(int)currentFace].GridActive(true);
    }

    IEnumerator goRight()
    {
        var timeEllapsed = 0f;
        grids[(int)currentFace].GridActive(false);
        currentFace = getNextFace(currentFace, currentFaceRot, 90);
        while (timeEllapsed < 0.25f)
        {
            KUB.Rotate(90 * 4 * Vector3.up * Time.deltaTime, Space.World);
            timeEllapsed += Time.deltaTime;
            yield return null;
        }
        KUB.eulerAngles = new Vector3(Mathf.RoundToInt(KUB.eulerAngles.x/90)*90,Mathf.RoundToInt(KUB.eulerAngles.y/90)*90,Mathf.RoundToInt(KUB.eulerAngles.z/90)*90);
        move = null;
        grids[(int)currentFace].GridActive(true);
    }

    IEnumerator turnRight()
    {
        var timeEllapsed = 0f;
        grids[(int)currentFace].GridActive(false);
        currentFaceRot -= 90;
        while (timeEllapsed < 0.25f)
        {
            KUB.Rotate(-90 * 4 * Vector3.forward * Time.deltaTime, Space.World);
            timeEllapsed += Time.deltaTime;
            yield return null;
        }
        KUB.eulerAngles = new Vector3(Mathf.RoundToInt(KUB.eulerAngles.x/90)*90,Mathf.RoundToInt(KUB.eulerAngles.y/90)*90,Mathf.RoundToInt(KUB.eulerAngles.z/90)*90);
        move = null;
        grids[(int)currentFace].GridActive(true);
    }

    IEnumerator turnLeft()
    {
        var timeEllapsed = 0f;
        grids[(int)currentFace].GridActive(false);
        currentFaceRot += 90;
        while (timeEllapsed < 0.25f)
        {
            KUB.Rotate(90 * 4 * Vector3.forward * Time.deltaTime, Space.World);
            timeEllapsed += Time.deltaTime;
            yield return null;
        }
        KUB.eulerAngles = new Vector3(Mathf.RoundToInt(KUB.eulerAngles.x/90)*90,Mathf.RoundToInt(KUB.eulerAngles.y/90)*90,Mathf.RoundToInt(KUB.eulerAngles.z/90)*90);
        move = null;
        grids[(int)currentFace].GridActive(true);
    }

    public void SaveFile()
    {
        if (playerPlaced)
        StartCoroutine(CustomLevelFileManager.WaitForSelectionInExplorer(true, true));
        else
        {
            if (warning != null) StopCoroutine(warning);
            ShowWarning("Player missing !");
        }
        
    }

    public void ShowWarning(string txt)
    {
        warning = StartCoroutine(WarningCoroutine(txt));
    }

    IEnumerator WarningCoroutine(string txt)
    {
        warningText.text = txt;
        warningText.color = Color.white;
        var timeEllapsed = 0f;
        while (timeEllapsed < 5f)
        {
            warningText.color = Color.Lerp(Color.white, new Color(1,1,1,0), timeEllapsed - 2f);
            timeEllapsed += Time.deltaTime;
            yield return null;
        }
        warningText.color = new Color(1,1,1,0);
        warning = null;
    }

    public void LoadFile()
    {
        StartCoroutine(CustomLevelFileManager.WaitForSelectionInExplorer(false,true));
    }

    private Face getNextFace(Face f, int rot, int dir) //dir : 0 = up, 90 = right, 180 = down, 270 = left
    {
        switch ((int)f)
        {
            case 0: switch (rot + dir) 
            {
                case 0: return Face.Yplus;
                case 90: return Face.Xplus;
                case 180: return Face.Yminus;
                case 270: return Face.Xminus;  
            } break;
            case 1: switch (rot + dir) 
            {
                case 0: return Face.Yplus;
                case 90: return Face.Zplus;
                case 180: return Face.Yminus;
                case 270: return Face.Zminus;  
            } break;
            case 2: switch (rot + dir) 
            {
                case 0: return Face.Yplus;
                case 90: return Face.Xminus;
                case 180: return Face.Yminus;
                case 270: return Face.Xplus;  
            } break;
            case 3: switch (rot + dir) 
            {
                case 0: return Face.Yplus;
                case 90: return Face.Zminus;
                case 180: return Face.Yminus;
                case 270: return Face.Zplus;  
            } break;
            case 4: switch (rot + dir) 
            {
                case 0: return Face.Zplus;
                case 90: return Face.Xplus;
                case 180: return Face.Zminus;
                case 270: return Face.Xminus;  
            } break;
            case 5: switch (rot + dir) 
            {
                case 0: return Face.Zminus;
                case 90: return Face.Xplus;
                case 180: return Face.Zplus;
                case 270: return Face.Xminus;  
            } break;
        }
        return Face.Zminus;
    }
}
