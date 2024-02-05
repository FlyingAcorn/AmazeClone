using UnityEngine;

public class Floor : MonoBehaviour
{
    private Renderer myRender;
    private void Start()
    {
        myRender = GetComponent<Renderer>();
    }

    public void OnTouched()
    {
        myRender.material.color = Color.blue;
        GridManager.Instance.whiteFloors.Remove(gameObject);
        GridManager.Instance.blueFloors.Add(gameObject);
        if (GridManager.Instance.whiteFloors.Count == 0) 
            GameManager.Instance.UpdateGameState(GameManager.GameState.Victory);
    }

    private void OnDisable()
    {
        if (GameManager.Instance.state == GameManager.GameState.Victory)
        {
            myRender.material.color = Color.white;
        }
    }
}
