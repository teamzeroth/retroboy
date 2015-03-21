using UnityEngine;
using System.Collections;

public class Director : MonoBehaviour {

    public Transform player = null;
    public Follow prefab = null;
    [HideInInspector]
    public Follow f = null;
    [HideInInspector]
    public int score = 0;
    public int interval = 5;
    
	// Use this for initialization
	void Start () {
	    if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;
        prefab.CreatePool();
        StartCoroutine(SpawnEnemy(interval));
	}

    public void increaseScore()
    {
        this.score++;
        GameObject.Find("UI").GetComponentInChildren<UnityEngine.UI.Text>().text = "Score   " + score;
    }

    IEnumerator SpawnEnemy(int seconds)
    {
        while (prefab.CountSpawned<Follow>() < 5)
        {
            yield return new WaitForSeconds(seconds);
            f = prefab.Spawn();
            float distance = 0;
            do
            {
                f.transform.position = new Vector3((Random.value * 2 - 1) * 10f, (Random.value * 2 - 1) * 5f, 0);
                distance = (player.transform.position - f.transform.position).magnitude;
                print("Distancia: " + distance);
            } while (distance < 8 || distance > 15);
        }
    }
}
