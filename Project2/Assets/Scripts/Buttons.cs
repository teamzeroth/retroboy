using UnityEngine;
using System.Collections;

public class Buttons : MonoBehaviour {

	public void Quit()
    {
        Application.Quit();
    }

    public void StartGame()
    {
        ChangeScene(1);
    }

    public void ChangeScene(int index)
    {
        Application.LoadLevel(index);
    }

    public void Pause()
    {
        Time.timeScale = 0f;
    }

    public void Resume()
    {
        Time.timeScale = 1f;
    }

    public void clicked()
    {
        if (Debug.isDebugBuild)
            Debug.Log("Clicked");
    }
}
