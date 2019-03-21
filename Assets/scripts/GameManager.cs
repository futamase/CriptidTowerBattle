using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using UnityEngine.UI;

// 左クリック：オブジェクト操作開始
// 話すと落下
// 回転は右クリック
// 

public class GameManager : SingletonMonoBehaviour<GameManager> {

    public bool GameContinued = true;
    public ReactiveProperty<bool> IsPlayerTurn;
    public bool released = false;

    public Vector3 InitialPos;

    public GameObject[] prefabs;

    public GameObject currentObj;

    public Text CurrentPlayerText;

    public Text ObjectSum;
    public ReactiveProperty<uint> NumObj;

	// Use this for initialization
	void Start () {
        var player = Random.Range(0, 2);

        IsPlayerTurn = new ReactiveProperty<bool>();
        IsPlayerTurn.Value = (player == 0);

        NumObj = new ReactiveProperty<uint>();
        NumObj.Value = 0;

        this.ChangePlayer();

        Debug.Log("現在のターンは " + (IsPlayerTurn.Value ? "プレイヤー1" : "プレイヤー2"));

        var leftMouseDownStream =
            this.UpdateAsObservable()
            .Where(_ => GameContinued)
            .Where(_ => !released)
            .Where(_ => Input.GetMouseButton(0));

        var rightMouseDownStream =
            this.UpdateAsObservable()
            .Where(_ => GameContinued)
            .Where(_ => !released)
            .Where(_ => Input.GetMouseButtonDown(1));

        var leftMouseUpStream =
            this.UpdateAsObservable()
            .Where(_ => GameContinued)
            .Where(_ => Input.GetMouseButtonUp(0));

        this.UpdateAsObservable()
            .Where(_ => GameContinued)
            .Where(_ => released)
            .Where(_ => currentObj != null)
            .Where(_ => 
                currentObj.GetComponent<Rigidbody2D>().IsSleeping())
            .Subscribe(_ => this.ChangePlayer());

        //leftMouseDownStream.Subscribe(_ => )

        rightMouseDownStream
            .Subscribe(_ =>
            {
                if (currentObj == null)
                    return;

                currentObj.transform.Rotate(new Vector3(0, 0, -45f));
            });

        leftMouseDownStream
            .Subscribe(_ =>
            {
                if (currentObj == null)
                    return;

                var p = currentObj.transform.position;
                var newp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                p.x = newp.x;
                currentObj.transform.position = p;
            });

        leftMouseUpStream
            .Subscribe(_ =>
            {
                released = true;
                currentObj.GetComponent<Rigidbody2D>().isKinematic=false;
            });

        IsPlayerTurn.Subscribe(_ => CurrentPlayerText.text = "プレイヤー" + (IsPlayerTurn.Value ? "１" : "2") + "の番です");

        NumObj.Subscribe(_ => ObjectSum.text = "オブジェクトの個数：" + NumObj.Value);
	}

    void ChangePlayer()
    {
        // プレイヤーの番を変えて次のオブジェクトを生成
        IsPlayerTurn.Value = !IsPlayerTurn.Value;

        var num = Random.Range(0, prefabs.Length);
        currentObj = Instantiate(prefabs[num], InitialPos, Quaternion.identity);
        currentObj.GetComponent<Rigidbody2D>().isKinematic = true;

        NumObj.Value++;

        released = false;
    }
}
