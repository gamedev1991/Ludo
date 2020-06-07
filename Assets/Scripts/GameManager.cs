using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Turn
{
    Player1 = 0,
    Player2 = 2
}

public enum ColorMode
{
    BlueRed,
    YellowGreen
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public ColorMode currentColorMode;


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        DontDestroyOnLoad(instance);
    }
    public void OnClickOneVOne()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScreen");
    }

    public void OnClickBack()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}
