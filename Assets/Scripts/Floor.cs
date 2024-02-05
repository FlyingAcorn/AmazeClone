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
        GridManager.Instance.floors.Remove(gameObject);
        if (GridManager.Instance.floors.Count == 0) 
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
