using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditCustomLevel : MonoBehaviour
{
    public static EditCustomLevel Instance;
    [SerializeField] private GameObject placingMenu;
    [SerializeField] private GameObject editionMenu;
    [SerializeField] private GameObject facesMenu;
    public ObjectType currentBrush;
    public bool placing;
    public bool playerPlaced; //TODO Make Player unique, stop placing if Player already exists
    public int mode; //0 for nothing, 1 for placing, 2 for editing
    public int currentIPos;
    public int currentJPos;
    public TileBehaviour selectedTile;
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
        mode = 0;
        selectedTile = null;
    }

    // Update is called once per frame
    void Update()
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
}
