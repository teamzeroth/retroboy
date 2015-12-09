using UnityEngine;
using System.Collections;
using X_UniTMX;

[ExecuteInEditMode]
public class MapReader : MonoBehaviour {
    private TiledMapComponent TMComponent;

    void Start() {
        TMComponent = GetComponent<TiledMapComponent>();
        //TMComponent.OnMapComplet = OnMapLoaded;
    }

    void OnMapLoaded(Map map) {
        print("map:" + map);
        ///TODO: Aqui você pode continuar a contrução do codigo para dar load no objetos do jogo.
    }
}