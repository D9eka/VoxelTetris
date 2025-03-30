using UnityEngine;

public class LevelController : MonoBehaviour
{
    public void StartGame()
    {
        ServiceLocator.Instance.InputManager.EnableInput();
        ServiceLocator.Instance.FiguresController.StartSpawning();
    }

    public void EndGame()
    {
        ServiceLocator.Instance.InputManager.DisableInput();
        ServiceLocator.Instance.GridController.ClearPlanes();
        ServiceLocator.Instance.FiguresController.Clear();
    }

    public void RestartGame()
    {
        EndGame();
        StartGame();
    }
}