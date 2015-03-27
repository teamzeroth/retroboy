using UnityEngine;
using System.Collections;

public class Director : MonoBehaviour {

    public Transform player = null;
    public Enemy prefab = null;
    public int interval = 5;
    public GameObject ui = null;

    [HideInInspector]
    public Enemy f = null;
    [HideInInspector]
    public int score = 0;
    
    
	// Use this for initialization
	void Start () {
	    if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;
        if (ui == null)
            ui = GameObject.Find("UI");
        prefab.CreatePool();
        StartCoroutine(SpawnEnemy(interval));
	}

    public void increaseScore()
    {
        this.score++;
        ui.GetComponentsInChildren<UnityEngine.UI.Text>()[1].text = "Score - " + score;
    }

    public void updateLife(float life)
    {
        ui.GetComponentInChildren<UnityEngine.UI.Text>().text = "Life - " + life;
    }

    IEnumerator SpawnEnemy(int seconds)
    {
        while (prefab.CountSpawned<Enemy>() < 5)
        {
            yield return new WaitForSeconds(seconds);
            f = prefab.Spawn();
            float distance = 0;
            do
            {
                f.transform.position = new Vector3((Random.value * 2 - 1) * 10f, (Random.value * 2 - 1) * 5f, 0);
                distance = (player.transform.position - f.transform.position).magnitude;                
            } while (distance < 8 || distance > 15);
        }
    }
}
