using UnityEngine;
using UnityEngine.SceneManagement;

public class menuscript : MonoBehaviour
{
  void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void startGame()
    {
        SceneManager.LoadScene("MainScene");
        Time.timeScale = 1f;
    }
    public void menuGame()
    {
        SceneManager.LoadScene("Menu");
    }
    public void quitGame()
    {
        Application.Quit();
    }

}
