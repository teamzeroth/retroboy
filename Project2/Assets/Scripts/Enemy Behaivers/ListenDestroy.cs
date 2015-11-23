using UnityEngine;
using System.Collections;

public class ListenDestroy : MonoBehaviour {

    public EnemiesRadar radar;

    public void OnDestroy() {
        radar.OnDestroyEnemy(GetComponent<BaseEnemy>());
    }
}
