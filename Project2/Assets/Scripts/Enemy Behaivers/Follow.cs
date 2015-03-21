using UnityEngine;
using System.Collections;

public class Follow : Enemy {

    private bool seek;
    private Camera camera;

	// Use this for initialization
	void Start () {
        seek = true;
        target = GameObject.FindGameObjectWithTag("Player").transform;        
	}
	
	// Update is called once per frame
	void Update () {
        if (life <= 0)
        {
            GameObject.Destroy(this.gameObject);
            Camera.main.GetComponent<Director>().increaseScore();
        }
        else if (seek)
        {
            UpdatePosition();
            Movement();
        }
	}
    
    void Movement()
    {
        // TODO:  Não é recomendado mudar a posição do inimigo, em vez disso o recomendando é adicionar um Rigdibory2D e mudar a velocity dele

        this.gameObject.transform.position += direction * speed * Time.deltaTime;
        Debug.DrawLine(this.gameObject.transform.position, target.position);
    }

    void Attack(GameObject obj)
    {
        print(obj.gameObject.name);
        if (Debug.isDebugBuild) Debug.LogWarning("Matei!");
        Time.timeScale = 0f;
        // TODO: esse tipo de busca é muito caro, tente evitar
        foreach (Transform child in GameObject.Find("UI").transform) 
            child.gameObject.SetActive(true);//.GetComponentInChildren<UnityEngine.UI.Image>().gameObject.name);//.gameObject.SetActive(true);

        //Object.Destroy(obj.gameObject);
        obj = null;
        seek = false;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
            Attack(collision.gameObject);
    }

    void OnTriggerEnter2D(Collider2D trigger)
    {
        print(trigger.gameObject.name);
        if (trigger.gameObject.name.Contains("shoot"))
        {
            this.life -= trigger.gameObject.GetComponent<ShootMove>().damage;
            Object.Destroy(trigger.gameObject);
            if (Debug.isDebugBuild) Debug.Log(this.life);
        } 
    }
}
