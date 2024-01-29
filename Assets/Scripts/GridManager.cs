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
        ballPosWidth = Random.Range(0, 10);
        ballPosHeight = Random.Range(0, 10);
        var ballObject = Instantiate(ball, grid[ballPosWidth, ballPosHeight].transform.position,
            Quaternion.identity, grid[ballPosWidth, ballPosHeight].transform);
        ballObject.name = "Ball";
        ballObject.transform.position += new Vector3(0, 0.5f, 0);
        for (int i = 0; i < maxRepeat; i++)
        {
            // hateketin yonu 0 yukarı 1 asağı 2 sağ 3 sol 
            var direction = Random.Range(0, 4);
            var movement = Random.Range(2, 9);
            for (int j = 0; j <= movement; j++)
            {
                //boundsu aşmama kodunu ayarlayacaksın sonra onunde kutu varsa durmasını sağlayacaksın
                //en son boş kalan yerlere kutu koymasını sağlayacaksın
                // şu anda kod block koyduğu yerde durduğunu zannediyor.
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
            Debug.Log("stopped at "+ ballPosWidth.ToString()+ballPosHeight.ToString());
        }
    }

    private void ManageMovement(int movement, int j, bool isVertical = false, bool isNegative = false)
    {
        Debug.Log(ballPosWidth.ToString() + ballPosHeight.ToString()+isNegative.ToString());
        if (j < movement)
        {
            grid[ballPosWidth, ballPosHeight].transform.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            if (isVertical)
            {
                grid[ballPosWidth, ballPosHeight].transform.GetChild(0).gameObject.SetActive(true);
                grid[ballPosWidth, isNegative ? ballPosHeight - 1 : ballPosHeight + 1].transform.GetChild(1).gameObject.SetActive(true);
            }
            else
            {
                grid[ballPosWidth, ballPosHeight].transform.GetChild(0).gameObject.SetActive(true);
                grid[isNegative ? ballPosWidth -1: ballPosWidth +1, ballPosHeight].transform.GetChild(1).gameObject.SetActive(true);
            }
        }
        if ( isVertical)
        {
            ballPosHeight += isNegative ? -1 : +1;
        }
        if (!isVertical)
        {
            ballPosWidth += isNegative ? -1 : +1;
        }
        
    }
}