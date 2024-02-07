using System;
public class GameManager : Singleton<GameManager>
{
    public static event Action<GameState> OnGameStateChanged;
    public enum GameState
    {
        Generating,
        Play,
        Pause,
        Settings,
        MainMenu,
        Victory
    }
    public GameState state;
    

    public void UpdateGameState(GameState newState)
    {
        state = newState;
        if (newState == GameState.Pause)
        {
            
        }
        if (newState == GameState.Play)
        {
            
        }if (newState == GameState.Settings)
        {
            
        }if (newState == GameState.MainMenu)
        {
            
        }
        if (newState == GameState.Victory)
        {
            StartCoroutine(GridManager.Instance.NextLevelSequence());
        }
        OnGameStateChanged?.Invoke(newState);
    }
}
