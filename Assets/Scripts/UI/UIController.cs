
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button exitButton;


    private void Start()
    {
        playButton.onClick.AddListener(ToPlayScene);
        exitButton.onClick.AddListener(Exit);
    }

    void ToPlayScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Level1", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    void Exit()
    {
        Application.Quit();
    }
}
