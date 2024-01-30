using UnityEngine;
using Random = UnityEngine.Random;

public class GridManager : MonoBehaviour
{
    [SerializeField] private int width;
    [SerializeField] private int height;
    private GameObject[,] grid;
    [SerializeField] private Ball ball;
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
            ballPosHeight = ballPosHeight switch
            {
                < 1 => 1,
                > 8 => 8,
                _ => value
            };
        }
    }

    private int BallPosWidth
    {
        get => ballPosWidth;
        set
        {
            ballPosWidth = ballPosWidth switch
            {
                < 1 => 1,
                > 8 => 8,
                _ => value
            };
        }
    }


    private void Start()
    {
        grid = new GameObject[width, height];
        GenerateGrids();
        GenerateLevel();
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

    }

    private void GenerateLevel()
    {
        //Topun yeri
        BallPosWidth = Random.Range(0, 10);
        BallPosHeight = Random.Range(0, 10);
        var ballObject = Instantiate(ball, grid[BallPosWidth, BallPosHeight].transform.position,
            Quaternion.identity, grid[BallPosWidth, BallPosHeight].transform);
        ballObject.name = "Ball";
        ballObject.transform.position += new Vector3(0, 0.5f, 0);
        for (int i = 0; i < maxRepeat; i++)
        {
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
            lastStopH = BallPosHeight;
            lastStopW = BallPosWidth;
        }
        FillEmptyGrids();
    }

    private void ManageMovement(int movement, int j, bool isVertical = false, bool isNegative = false)
    {
        
        if (j < movement)
        {
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
            grid[0,i].transform.GetChild(1).gameObject.SetActive(true);
            grid[0,i].transform.GetChild(0).gameObject.SetActive(false);
        }

        for (int j = 9; j >= 0; j--)
        {
            grid[j,9].transform.GetChild(1).gameObject.SetActive(true);
            grid[j,9].transform.GetChild(0).gameObject.SetActive(false);
            grid[9,j].transform.GetChild(1).gameObject.SetActive(true);
            grid[9,j].transform.GetChild(0).gameObject.SetActive(false);
        }

        foreach (var t in grid)
        {
            if (t.transform.GetChild(0).gameObject.activeSelf == false)
            {
                t.transform.GetChild(1).gameObject.SetActive(true);
            }
        }
    }
}