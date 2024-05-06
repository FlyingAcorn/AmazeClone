using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;
public class GridManager : Singleton<GridManager>
{
    [SerializeField] public int width;
    [SerializeField] public int height;
    private GridPoint[,] grid;
    [SerializeField] public List<GameObject> whiteFloors;
    [SerializeField] public List<GameObject> blueFloors;
    [SerializeField] private Ball ball;
    private GameObject createdBall;
    [SerializeField] private Floor floor;
    [SerializeField] private Block block;
    [SerializeField] private int maxRepeat;
    private int ballPosWidth;
    private int ballPosHeight;
    private int lastStopW,lastStopH;
    private GameObject currentLevel;
    private int successNumber;
    private int levelNo;
    private bool isOnBlock;
    [SerializeField] private GridPoint gridPointObject;
    
    
    private int BallPosHeight
    {
        get => ballPosHeight;
        set
        {
            if (ballPosHeight < 1)
                ballPosHeight = 1;
            else if (ballPosHeight > height-2)
                ballPosHeight = height-2;
            else
                ballPosHeight = value;
        }
    }
    private int BallPosWidth
    {
        get => ballPosWidth;
        set
        {
            if (ballPosWidth < 1)
                ballPosWidth = 1;
            else if (ballPosWidth > width-2)
                ballPosWidth = width-2;
            else
                ballPosWidth = value;
        }
    }
    private void Start()
    {
        grid = new GridPoint[width, height];
        createdBall = Instantiate(ball.gameObject);
        createdBall.name = "Ball";
        GenerateGrids(0);
        GenerateLevel();
        CameraManager.Instance.AdjustCamera(width,height);
        
    }
    private void GenerateGrids(int gridXOffset)
    {
        var level = new GameObject("level"+levelNo);
        level.transform.position += new Vector3(gridXOffset, 0, 0);
        currentLevel = level;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var gO = Instantiate(gridPointObject, parent: level.transform);
                gO.gameObject.name = x+","+y;
                gO.gridPoints = new Vector2Int(x,y);
                grid[x, y] = gO;
                gO.transform.position = new Vector3(x+gridXOffset + 0.5f, 0, y + 0.5f);
                var floorObject = Instantiate(floor, new Vector3(x+gridXOffset + 0.5f, 0, y + 0.5f),
                    Quaternion.identity, grid[x, y].transform);
                floorObject.gameObject.name = "Floor" + x + "," + y;
                var blockObject = Instantiate(block, new Vector3(x+gridXOffset + 0.5f, 0.5f, y + 0.5f),
                    Quaternion.identity, grid[x, y].transform);
                blockObject.gameObject.name = "Block" + x + "," + y;
            }
            createdBall.transform.SetParent(level.transform);
        }
        
    }
    private void FloorList()
    {
        foreach (var t in grid)
        {
            var floorOfGrid = t.transform.GetComponentInChildren<Floor>(true);
            if (floorOfGrid.gameObject.activeSelf)
            {
                whiteFloors.Add(floorOfGrid.gameObject);
            }
        }
        Debug.Log(whiteFloors.Count);
    }
    private void GenerateLevel()
    {
        ballPosWidth = Random.Range(1, width-1);
        ballPosHeight = Random.Range(1, height-1);
        createdBall.transform.position = 
            grid[ballPosWidth, ballPosHeight].transform.position + new Vector3(0, 0.5f, 0);
        for (int i = 0; i < maxRepeat; i++)
        {
            lastStopH = BallPosHeight;
            lastStopW = BallPosWidth;
            // hateketin yonu 0 yukarı 1 asağı 2 sağ 3 sol 
            var direction = Random.Range(0, 4);
            var movement = Random.Range(2, 9);
            for (int j = 0; j <= movement; j++)
            {
                switch (direction) 
                    {
                    case 0:
                        ManageMovement(movement, j,true);
                        break;
                    case 1:
                        ManageMovement(movement, j, true,true);
                        break;
                    case 2:
                        ManageMovement(movement, j);
                        break;
                    case 3:
                        ManageMovement(movement, j, isNegative: true);
                        break; 
                    }
            }
        }
        FillEmptyGrids();
        FloorList();
        TryLevel();
    }
    private void ManageMovement(int movement, int idx, bool isVertical = false, bool isNegative = false)
    {
        //TODO: Bu kısımda nadiren algoritmanın bir hamlesini belirli bir sırada yapman gerekiyor yoksa dead end oluşturuyor.
        //TODO: bunu cihana anlat danış Ps. cihan ilk algoritmam pls show mercy
        var blockOfGrid = grid[BallPosWidth, BallPosHeight].transform.GetComponentInChildren<Block>(true);
        var floorOfGrid = grid[BallPosWidth, BallPosHeight].transform.GetComponentInChildren<Floor>(true);
        if (idx < movement)
        {
            if (blockOfGrid.gameObject.activeSelf)
            {
                BallPosWidth = lastStopW;
                BallPosHeight = lastStopH;
                return;
            }
            floorOfGrid.gameObject.SetActive(true);
            if (isVertical)
            {
                BallPosHeight += isNegative ? -1 : +1;
            }
            else
            {
                BallPosWidth += isNegative ? -1 : +1;
            }
        }
        else
        {
            if (floorOfGrid.gameObject.activeSelf)
            {
                
                BallPosWidth = lastStopW; 
                BallPosHeight = lastStopH;
                return;
            } 
            blockOfGrid.gameObject.SetActive(true);
            if (isVertical)
            {
                BallPosHeight += isNegative ? +1 : -1;
            }
            else
            {
                BallPosWidth += isNegative ? +1 : -1;
            }
        }
    }
    private void FillEmptyGrids()
    {
        for (int i = 0; i < width; i++)
        {
            grid[i,0].transform.GetChild(1).gameObject.SetActive(true);
            grid[i,0].transform.GetChild(0).gameObject.SetActive(false);
        }
         
        for (int i = 0; i < height; i++)
        {
            grid[0,i].transform.GetChild(1).gameObject.SetActive(true);
            grid[0,i].transform.GetChild(0).gameObject.SetActive(false);
        }

        for (int i = width-1; i >= 0; i--)
        {
            grid[i,height-1].transform.GetChild(1).gameObject.SetActive(true);
            grid[i,height-1].transform.GetChild(0).gameObject.SetActive(false);
        }

        for (int i = height-1; i >=0 ; i--)
        {
            grid[width-1,i].transform.GetChild(1).gameObject.SetActive(true);
            grid[width-1,i].transform.GetChild(0).gameObject.SetActive(false);
        }

        foreach (var t in grid)
        {
            if (t.transform.GetChild(0).gameObject.activeSelf == false)
            {
                t.transform.GetChild(1).gameObject.SetActive(true);
            }
        }
    }

    private void Randomizer()
    {
        whiteFloors[Random.Range(0, whiteFloors.Count)].transform.parent.TryGetComponent(out GridPoint gridPoint);
        ballPosWidth = gridPoint.gridPoints.x;
        ballPosHeight = gridPoint.gridPoints.y;
        Debug.Log(ballPosWidth+" "+ballPosHeight+ "keke");
    }
    private void TryLevel()
    {
        Randomizer();
        
        for (int i = 0; i < 10000; i++)
        {
            isOnBlock = false;
            var direction = Random.Range(0, 4);
            while (!isOnBlock)
            {
                switch (direction) 
                {
                    case 0:
                        TryLevelsMovement(true);
                        break;
                    case 1:
                        TryLevelsMovement(true,true);
                        break;
                    case 2:
                        TryLevelsMovement();
                        break;
                    case 3:
                        TryLevelsMovement(isNegative: true);
                        break;
                }
            }
            Debug.Log(whiteFloors.Count);
            if (whiteFloors.Count == 0)
            {
                successNumber++;
                Debug.Log("success" +"x" + successNumber);
                whiteFloors.Clear();
                FloorList();
                Randomizer();
            }
            if (i != 9999) continue;
            if (successNumber > 200)
            {
                Debug.Log("yedi");
                whiteFloors.Clear();
                FloorList();
            }
            else if (whiteFloors.Count != 0)
            {
                Debug.Log("failed");
                foreach (var t in grid)
                {
                    var blockOfGridInT = t.transform.GetComponentInChildren<Block>(true);
                    var floorOfGridInT = t.transform.GetComponentInChildren<Floor>(true);
                    blockOfGridInT.gameObject.SetActive(false);
                    floorOfGridInT.gameObject.SetActive(false);
                }
                whiteFloors.Clear();
                GenerateLevel();
            }
            successNumber = 0;
        } 
    }

     private void TryLevelsMovement(bool isVertical = false, bool isNegative = false)
    {
        var blockOfGrid = grid[BallPosWidth, BallPosHeight].transform.GetComponentInChildren<Block>(true);
        var floorOfGrid = grid[BallPosWidth, BallPosHeight].transform.GetComponentInChildren<Floor>(true);
        if (floorOfGrid.gameObject.activeSelf)
        {
            whiteFloors?.Remove(floorOfGrid.gameObject);
            if (isVertical)
            {
                BallPosHeight += isNegative ? -1 : +1;
            }
            else
            {
                BallPosWidth += isNegative ? -1 : +1;
            }
        }
        else if (blockOfGrid.gameObject.activeSelf)
        {
            if (isVertical)
            {
                BallPosHeight += isNegative ? +1 : -1;
            }
            else
            {
                BallPosWidth += isNegative ? +1 : -1;
            }
            isOnBlock = true;
        }
    }
     public IEnumerator NextLevelSequence()
     {
         blueFloors.Clear();
         grid = null;
        CameraManager.Instance.myCamera.DOOrthoSize(width + 2, 1);
        yield return new WaitForSeconds(1);
        currentLevel.transform.DOMove(new Vector3(-30, 0,0),1);
        yield return new WaitForSeconds(1);
        createdBall.transform.parent = null;
        currentLevel.SetActive(false);
        width = Random.Range(6,13);
        height = Random.Range(6,13);
        grid = new GridPoint[width, height];
        GenerateGrids(30);
        GenerateLevel();
        currentLevel.transform.DOMove(new Vector3(0, 0, 0), 1);
        yield return new WaitForSeconds(1);
        CameraManager.Instance.AdjustCamera(width,height);
        yield return new WaitForSeconds(1.5f);
        GameManager.Instance.UpdateGameState(GameManager.GameState.Play);
        yield return null;
    }
}