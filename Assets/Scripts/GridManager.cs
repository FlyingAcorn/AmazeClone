using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GridManager : Singleton<GridManager>
{
    [SerializeField] public int width;
    [SerializeField] public int height;
    private GameObject[,] grid;
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
    //kamerayı bunlara gore ayarla.
    //deadend verme durumunu coz
    private void Start()
    {
        grid = new GameObject[width, height];
        GenerateGrids();
        GenerateLevel();
        FloorList();
        CameraManager.Instance.AdjustCamera(width,height);
    }
    private void GenerateGrids()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var gameobject = new GameObject(x + "," + y);
                gameobject.transform.parent = transform;
                grid[x, y] = gameobject;
                gameobject.transform.position = new Vector3(x + 0.5f, 0, y + 0.5f);
                var floorObject = Instantiate(floor, new Vector3(x + 0.5f, 0, y + 0.5f),
                    Quaternion.identity, grid[x, y].transform);
                floorObject.gameObject.name = "Floor" + x + "," + y;
                var blockObject = Instantiate(block, new Vector3(x + 0.5f, 0.5f, y + 0.5f),
                    Quaternion.identity, grid[x, y].transform);
                blockObject.gameObject.name = "Block" + x + "," + y;
            }
        }
        createdBall = Instantiate(ball.gameObject);
        createdBall.name = "Ball";
    }
    private void FloorList()
    {
        foreach (var t in grid)
        {
            if (t.transform.GetChild(0).gameObject.activeSelf)
            {
                whiteFloors.Add(t.transform.GetChild(0).gameObject);
            } 
        }
    }
    private void GenerateLevel()
    {
        //Topun yeri encapsulationun dışında olmasının nedeni ilk seçime karışmasın.
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
    }
    private void ManageMovement(int movement, int j, bool isVertical = false, bool isNegative = false)
    {
        if (j < movement)
        {
            //var obj = grid[BallPosWidth, BallPosHeight]
            //.transform.GetComponentInChildren<Block>(buraya true yaparsan inaktif objelerede bakar);
            //if(obj.activeSelf) bu commentteki gibi yap getchild bad practice
            if (grid[BallPosWidth, BallPosHeight].transform.GetChild(1).gameObject.activeSelf)
            {
                BallPosWidth = lastStopW;
                BallPosHeight = lastStopH;
                return;
            }
            grid[BallPosWidth, BallPosHeight].transform.GetChild(0).gameObject.SetActive(true);
            if ( isVertical)
            {
                BallPosHeight += isNegative ? -1 : +1;
            }
            if (!isVertical)
            {
                BallPosWidth += isNegative ? -1 : +1;
            }
        }
        else
        {
            if (grid[BallPosWidth, BallPosHeight].transform.GetChild(0).gameObject.activeSelf)
            {
                BallPosWidth = lastStopW;
                BallPosHeight = lastStopH;
                return;
            }
            grid[BallPosWidth, BallPosHeight].transform.GetChild(1).gameObject.SetActive(true);
            if ( isVertical)
            {
                BallPosHeight += isNegative ? +1 : -1;
            }
            if (!isVertical)
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

        for (int j = width-1; j >= 0; j--)
        {
            grid[j,height-1].transform.GetChild(1).gameObject.SetActive(true);
            grid[j,height-1].transform.GetChild(0).gameObject.SetActive(false);
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
    public void DisableObjects()
    {
        foreach (var t in grid)
        {
            t.transform.GetChild(0).gameObject.SetActive(false);
            t.transform.GetChild(1).gameObject.SetActive(false);
        }
    }
    
}