using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public void GoToPlayground()
    {
        SceneManager.LoadScene("Playground"); 
    }
}
