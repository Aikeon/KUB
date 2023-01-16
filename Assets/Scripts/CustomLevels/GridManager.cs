using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public Vector2 cellSize;
    public Vector2 activeGridSize;
    [SerializeField] private float gridLineWidth;
    [SerializeField] private GameObject TilePrefab;
    [SerializeField] private GameObject LinePrefab;
    private Vector2 latestCellSize;
    private Vector2 latestActiveGridSize;
    private float latestGridLineWidth;
    public TileBehaviour[] tiles;
    public bool isFrontGrid;
    public int tileColumnNumber;
    // Start is called before the first frame update
    void Start()
    {
        latestCellSize = cellSize;
        latestActiveGridSize = activeGridSize;
        latestGridLineWidth = gridLineWidth;
        GenerateGrid();
    }

    // Update is called once per frame
    void Update()
    {
        if (latestActiveGridSize != activeGridSize || latestCellSize != cellSize || latestGridLineWidth != gridLineWidth)
        {
            ClearGrid();
            latestCellSize = cellSize;
            latestActiveGridSize = activeGridSize;
            latestGridLineWidth = gridLineWidth;
            GenerateGrid();
        }
    }

    public void ClearGrid()
    {
        var c = transform.childCount;
        for (int i = 0; i < c; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }

    public void GenerateGrid()
    {
        tileColumnNumber = (int)(activeGridSize.x / cellSize.x);
        tiles = new TileBehaviour[(int)(activeGridSize.y / cellSize.y) * tileColumnNumber];
        if (cellSize.x == 0 || cellSize.y == 0) return;
        var genLine = false;
        var iP = -3;
        var jP = -3;
        for (float i = -activeGridSize.y / 2f - 5f * cellSize.y / 2f; i < activeGridSize.y / 2f + 3 * cellSize.y; i += cellSize.y)
        {
            jP = -3;
            if (genLine && iP >= 0 && iP < tileColumnNumber)
            {
                var line_i = Instantiate(LinePrefab);
                line_i.transform.SetParent(transform);
                var lri = line_i.GetComponent<LineRenderer>();
                lri.startWidth = gridLineWidth;
                lri.endWidth = gridLineWidth;
                lri.SetPosition(0,new Vector3(i - cellSize.y / 2f,-activeGridSize.x/2f,0));
                lri.SetPosition(1,new Vector3(i - cellSize.y / 2f,activeGridSize.x/2f,0));
                lri.GetComponent<GridLineBehaviour>().point1 = new Vector3(i - cellSize.y / 2f,-activeGridSize.x/2f,0);
                lri.GetComponent<GridLineBehaviour>().point2 = new Vector3(i - cellSize.y / 2f,activeGridSize.x/2f,0);
            }
            for (float j = -activeGridSize.x / 2f - 5f * cellSize.x / 2f; j < activeGridSize.x / 2f + 3 * cellSize.x; j += cellSize.x)
            {
                if (!genLine && j != -activeGridSize.x / 2f + cellSize.x / 2f && jP >= 0 && jP < tileColumnNumber)
                {
                    var line_j = Instantiate(LinePrefab);
                    line_j.transform.SetParent(transform);
                    var lrj = line_j.GetComponent<LineRenderer>();
                    lrj.startWidth = gridLineWidth;
                    lrj.endWidth = gridLineWidth;
                    lrj.SetPosition(0,new Vector3(-activeGridSize.y / 2f,j - cellSize.x / 2f,0));
                    lrj.SetPosition(1,new Vector3(activeGridSize.y / 2f,j - cellSize.x / 2f,0));
                    lrj.GetComponent<GridLineBehaviour>().point1 = new Vector3(-activeGridSize.y / 2f,j - cellSize.x / 2f,0);
                    lrj.GetComponent<GridLineBehaviour>().point2 = new Vector3(activeGridSize.y / 2f,j - cellSize.x / 2f,0);
                }
                var tile = Instantiate(TilePrefab);
                tile.transform.SetParent(transform);
                tile.transform.localPosition = new Vector3(j,i,0f);
                tile.GetComponent<TileBehaviour>().iPos = iP;
                tile.GetComponent<TileBehaviour>().jPos = jP;
                tile.GetComponent<TileBehaviour>().grid = this;
                if (!(iP < 0 || jP < 0 || iP >= tileColumnNumber || jP >= tileColumnNumber)) tiles[iP*tileColumnNumber + jP] = tile.GetComponent<TileBehaviour>();
                tile.GetComponent<TileBehaviour>().active = !(iP < 0 || jP < 0 || iP > tileColumnNumber || jP > tileColumnNumber);
                jP++;
            }
            genLine = true;
            iP++;
        }
    }
}
