using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using UnityEngine.UI;

public class Death : MonoBehaviour {

    public Text text;
	// Use this for initialization
	void Start () {
        text.enabled = false;        		
	}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        text.enabled = true;
        GameManager.Instance.GameContinued = false;
        if (GameManager.Instance.IsPlayerTurn.Value)
        {
            text.text = "プレイヤー2の勝利";
        }
        else
        {
            text.text = "プレイヤー1の勝利";
        }


    }

    // Update is called once per frame
    void Update () {
		
	}
}
