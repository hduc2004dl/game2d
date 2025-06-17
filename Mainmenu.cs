using UnityEngine;
using UnityEngine.SceneManagement;

public class Mainmenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("Game"); // Thay "GameScene" bằng tên màn chơi chính
    }

    public void Quit()
    {
        Application.Quit();
    }
}
