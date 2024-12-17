using UnityEngine;
using System.Collections;

public class PressQuitButton : MonoBehaviour
{
    public void Press()
    {
        #if UNITY_STANDALONE
            Application.Quit();
        #endif
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
