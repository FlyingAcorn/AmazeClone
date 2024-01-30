using System;
public class GameManager : Singleton<GameManager>
{
    public static event Action<GameState> OnGameStateChanged;
    public enum GameState
    {
        Play,
        Pause,
        Settings,
        MainMenu
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
        OnGameStateChanged?.Invoke(newState);
    }
}
