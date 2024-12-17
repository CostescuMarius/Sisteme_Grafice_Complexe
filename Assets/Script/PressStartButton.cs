using UnityEngine;
using UnityEngine.SceneManagement;

public class PressStartButton : MonoBehaviour
{
    public void Press()
    {
        SceneManager.LoadScene("Select Level");
    }
}
