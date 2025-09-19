using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene1 : MonoBehaviour
{
    public void GoToMenu()
    {
        Debug.Log("Home Button Clicked!");
        SceneManager.LoadScene("Menu"); 
    }
}
