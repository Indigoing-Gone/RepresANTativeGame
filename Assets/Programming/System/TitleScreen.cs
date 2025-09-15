using UnityEngine;

public class TitleScreen : MonoBehaviour
{
    [SerializeField] private string startSceneName;

    public void StartGame()
    {
        SceneController.instance.LoadScene(startSceneName);
    }
}
