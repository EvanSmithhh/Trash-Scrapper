using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScript : MonoBehaviour
{

    public Image howTo;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        howTo.enabled = false;
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("Title");
    }
    
    public void Play()
    {
        SceneManager.LoadScene("Main");
    }

    public void HowToPlay()
    {
        if (howTo.enabled == false)
        {
            howTo.enabled = true;
        }
        else
        {
            howTo.enabled = false;
        }
    }

    public void Quit()
    {
        Application.Quit();
    }
}
