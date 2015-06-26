using UnityEngine;
using System.Collections;

public class Buttons : MonoBehaviour {

    public GameObject ui;

	public void quit()
    {
        Application.Quit();
    }

    public void mainMenu()
    {
        changeScene(0);
    }

    public void startGame()
    {
        changeScene(1);
    }

    public void restart()
    {
        changeScene(Application.loadedLevel);
    }

    public void changeScene(int index)
    {
        Application.LoadLevel(index);
    }

    public void pause()
    {
        Time.timeScale = 0f;
    }

    public void resume()
    {
        Time.timeScale = 1f;
    }

    public void clicked()
    {
        if (Debug.isDebugBuild)
            Debug.Log(this.name + " clicked");
    }

    void Update()
    {
        if (Input.anyKeyDown)
            Application.LoadLevel("title-screen");
        if (Input.GetKeyDown(KeyCode.Escape) && ui != null)
            ui.SetActive(!ui.activeSelf);
    }
}
