using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLevelButton : MonoBehaviour
{
    public void LoadLevelOnClick(string levelToLoad)
    {
        SceneManager.LoadScene(levelToLoad);
    }
}
