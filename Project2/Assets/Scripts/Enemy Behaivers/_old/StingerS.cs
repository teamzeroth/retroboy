using UnityEngine;
using System.Collections;

public class StingerS : BaseEnemy {
    public float distPlayerPoint;

    private Vector3 _center;

    private Vector3 initialPos, targetDirection, pointDirection;
    private Vector3[] points;
    private float randomTime;
    private int pointIndex;
    private bool destroyed;

    void Start() {
        base.Start();

        _center = transform.Find("center").position;
        initialPos = renderer.transform.localPosition;
        randomTime = Random.value * 3;
        points = new Vector3[4];
        pointIndex = -1;
        pointDirection = Vector3.zero;
        targetDirection = Vector3.zero;
    }

    void Update() {
        if (destroyed) return;
        float currDistance = 0f;

        if (target != null) {
            updatePoints();

            Vector3 heading = target.transform.position - transform.position;
            targetDirection = heading.normalized;
            currDistance = heading.magnitude;

            if (pointIndex == -1)
                nearestPoint();

            heading = points[pointIndex] - transform.position;
            pointDirection = heading.normalized;
        }

        Vector3 senoid = initialPos + new Vector3(0, 0.2f, 0) * Mathf.Sin((randomTime + Time.time) * 2);

        renderer.transform.localPosition = senoid;

        print(" | " + pointIndex + " | ");
        UpdateMove();
        UpdateAnimation();
    }

    protected void updatePoints() {
        Vector3 t = new Vector3(-1, 1, 0);
        points[0] = target.transform.position + (Vector3)Vector2.one * distPlayerPoint;
        points[1] = target.transform.position + t * distPlayerPoint;
        points[2] = target.transform.position - (Vector3)Vector2.one * distPlayerPoint;
        points[3] = target.transform.position - t * distPlayerPoint;
    }

    void nearestPoint() {
        float minDistance = 10000f, d;
        for (int i = 0; i < points.GetLength(0); i++) {
            d = (transform.position - points[i]).sqrMagnitude;
            if (d < minDistance) {
                minDistance = d;
                pointIndex = i;
            }
        }
    }

    void UpdateMove() {
        //pointDirection = Vector3.zero;

        if (target != null) pointDirection = (points[pointIndex] - transform.position).normalized * speed / distPlayerPoint;
        print(pointDirection + " " + impulseForce);
        Vector3 currentDirection = (pointDirection + impulseForce) / 2;

        GetComponent<Rigidbody2D>().MovePosition(transform.position + currentDirection * Time.deltaTime);
    }

    void UpdateAnimation() {
        //if (target != null) _anim.SetTrigger("Tracking");
        _anim.SetFloat("Vertical", targetDirection.y);
        _anim.SetFloat("Horizontal", -targetDirection.x);
    }

    public void OnDrawGizmosSelected() {
        Gizmos.color = Color.blue;
        for (int i = 0; i < 4; i++)
            Gizmos.DrawWireCube(points[i], Vector3.one);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, points[pointIndex]);
        Gizmos.DrawWireSphere(transform.position, rangeAtack);
    }

    public override void LostPlayer(MovableBehaviour player) { }
}
