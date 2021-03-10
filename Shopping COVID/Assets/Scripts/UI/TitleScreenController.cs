using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenController : MonoBehaviour
{
    public void StartGame() {
        //Load Level 1
        SceneManager.LoadScene(1);
    }
}
