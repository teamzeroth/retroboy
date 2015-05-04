using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UiController : MonoBehaviour {

    private static float TOTAL_SIZE_CHARGE = 92.4f;

    public static UiController self;

    private static GameObject LifePoint;

    private int _life;
    public int life{
        get{
            return _life;
        }
        set{
            _life = Mathf.Max(0, value);
            changeLifeCounter();
        }
    }

    private float _charge;
    public float charge {
        get {
            return _charge;
        }
        set {
            _charge = Mathf.Clamp01(value);
            changeCharge();
        }
    }

    private Transform _ui;
    private Transform _lifeHud;
    private Transform _chargeHud;

    private int hideLifePoints = 0;

    void Awake() {
        loadPoolObject();
        loadprivateObject();

        life = 4;
        charge = 0;

        self = this;       
    }

        private void loadPoolObject(){
            if (LifePoint == null) LifePoint = Resources.Load<GameObject>("GUI/life-point");
            LifePoint.CreatePool();
        }

        private void loadprivateObject() {
            _ui = GameObject.FindWithTag("UI").transform;
            _lifeHud = _ui.Find("Health Panel/life");
            _chargeHud = _ui.Find("Health Panel/charge-slider");

            print(_lifeHud);
        }

    private void changeCharge() {
        RectTransform chargeTransform = _chargeHud.transform as RectTransform;
        chargeTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, TOTAL_SIZE_CHARGE * _charge);
    }

    private void changeLifeCounter(){
        print("life: " + life + " " + _lifeHud.childCount + " => " + (life < _lifeHud.childCount));

        var lifePoints = _lifeHud.childCount;

        if (life > lifePoints)
            for (var i = 0; i < _life - lifePoints; i++) {
                GameObject lp = LifePoint.Spawn(_lifeHud);
                lp.transform.localScale = Vector3.one;
            }
        
        else if (life < lifePoints)
            for (var i = 1; i <= _lifeHud.childCount - _life; i++) {
                _lifeHud.GetChild(_lifeHud.childCount - i).gameObject.Recycle();
            }
    }   
}
