using System;
using System.Collections;
using System.Collections.Generic;
// using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class DiceRoll : MonoBehaviour
{

    public Sprite defaultDiceImage;
    public Image diceImage;
    public Sprite[] diceImages;
    public static Turn currentTurn;

    public delegate void DiceRolled(int value);
    public static event DiceRolled OnDiceRolled;
    public Text playerText;

    public Animator diceAnimator;

    public bool dontRoll;
    private Vector3[] dicePos = new Vector3[] { new Vector3(-350f, -650f, 0f), new Vector3(-350f, 650f, 0f), new Vector3(300f, 650f, 0f), new Vector3(300f, -650f, 0f) };
    // Use this for initialization
    void Start()
    {
        Player.OnMoveOver += ChangeTurn;
        diceImage.sprite = defaultDiceImage;
        this.transform.localPosition = dicePos[(int)currentTurn];
        playerText.text = currentTurn.ToString();
    }

    public void OnClickDice()
    {
        // PhotonView photonView = PhotonView.Get(this);
        // photonView.RPC("OnClickDiceRoll", RpcTarget.All);
        OnClickDiceRoll();
    }

    // [PunRPC]
    public void OnClickDiceRoll()// Method for generating random number on click
    {

        if (dontRoll)
        {
            return;
        }
        diceAnimator.Rebind();
        diceAnimator.enabled = true;
    }

    public void DiceRollComplete()
    {
        int randomNumber = UnityEngine.Random.Range(0, 6);
        int num = randomNumber + 1;
        if (OnDiceRolled != null)
        {
            OnDiceRolled(num);
        }
        diceAnimator.enabled = false;
        diceImage.sprite = diceImages[randomNumber];
        dontRoll = true;
        if (!LudoManager.instance.isMoveValid(num))
        {
            Invoke("ChangeTurn", 0.5f);
        }
    }

    void ChangeTurn()
    {
        if (LudoManager.currentDiceValue != 6)
        {
            if (currentTurn == Turn.Player1)
            {
                currentTurn = Turn.Player2;
            }
            else
            {
                currentTurn = Turn.Player1;
            }
        }
        this.transform.localPosition = dicePos[(int)currentTurn];
        dontRoll = false;
        LudoManager.currentDiceValue = -1;
        playerText.text = currentTurn.ToString();

    }

}
