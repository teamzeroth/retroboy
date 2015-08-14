using UnityEngine;
using UnityEngine.UI;
using System.Collections;

using DG.Tweening;

public class UiController : MonoBehaviour {

    private static float TOTAL_SIZE_CHARGE = 92.4f;

    public static UiController self;

    private static GameObject LifePoint;

    private int _life;
    public int Life{
        get{
            return _life;
        }
        set{
            _life = Mathf.Max(0, value);
            changeLifeCounter();
        }
    }

    private float _charge;
    public float Charge {
        get {
            return _charge;
        }
        set {
            _charge = Mathf.Clamp01(value);
            changeCharge();
        }
    }

    private int _coins;
    public int Coins {
        get {
            return _coins;
        }
        set {
            changeCoins(_coins, value);
            _coins = value;
        }
    }

    private Transform _ui;
    private Transform _lifeHud;
    private Transform _chargeHud;
    private Transform _coinsHud;

    private Transform _pausePanel;

    private int hideLifePoints = 0;

    void Awake() {
        loadPoolObject();
        loadprivateObject();

        Life = 4;
        Charge = 0;

        self = this;       
    }

        private void loadPoolObject(){
            if (LifePoint == null) LifePoint = Resources.Load<GameObject>("GUI/life-point(new)");
            LifePoint.CreatePool();
        }

        private void loadprivateObject() {
            _ui = GameObject.FindWithTag("UI").transform;
            _lifeHud = _ui.Find("Health Panel/life");
            _chargeHud = _ui.Find("Health Panel/charge-slider");
            _coinsHud = _ui.Find("Coins Panel");

            _pausePanel = _ui.parent.Find("Pause Menu");
        }

    #region UI Buttons messegens

    public void OnClickResume(){
        print("Resume");
    }

    public void OnClickRestart() {
        print("Restart");
    }

    public void OnClickQuit() {
        print("Quit");
    }

    #endregion

    public void TogglePauseGame(bool show) {
        if (show) {
            _pausePanel.gameObject.SetActive(true);
        } else {
            _pausePanel.gameObject.SetActive(false);
        }
    }

    private void changeCharge() {
        RectTransform chargeTransform = _chargeHud.transform as RectTransform;
        chargeTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, TOTAL_SIZE_CHARGE * _charge);
    }

    private void changeLifeCounter(){
        var lifePoints = _lifeHud.childCount;

        if (Life > lifePoints)
            for (var i = 0; i < _life - lifePoints; i++) {
                GameObject lp = LifePoint.Spawn(_lifeHud);
                lp.transform.localScale = Vector3.one;
            }
        
        else if (Life < lifePoints)
            for (var i = 1; i <= _lifeHud.childCount - _life; i++) {
                _lifeHud.GetChild(_lifeHud.childCount - i).gameObject.Recycle();
            }
    }

    Tween coinsToween;
    private void changeCoins(int before, int current) {
        if (coinsToween != null) {
            coinsToween.Kill();
        }
        
        int delta = before;
        coinsToween = DOTween.To(() => delta, x => delta = x, current, 0.5f).OnUpdate(()=>{
            
            Transform countText = _coinsHud.Find("coins-count");
                countText.GetComponent<Text>().text = delta.ToString();
            countText = _coinsHud.Find("coins-count-shadow");
                countText.GetComponent<Text>().text = delta.ToString();

        });

    }

}
