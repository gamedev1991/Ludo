using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Player : MonoBehaviour
{
    public Turn playerType;
    public bool isPlayerOut;
    public bool isPlayerReachedHome;
    public Transform parent;
    public Image playerImg;
    public GameObject[] pathPoints;
    public GameObject startingStar;

    public Vector3 startingPos;
    public float animationSpeed;
    public float animationTime;

    public int currentPosIndex = 0;
    int maxSteps = 0;
    int currentStep = 0;

    Vector3[] player_Path;
    public bool moveStarted = false;

    public delegate void MoveOver();
    public static event MoveOver OnMoveOver;

    bool isExtraTurn;


    public void Start()
    {
        // DiceRoll.OnDiceRolled += Move;
    }

    public int PointsLeft()
    {
        return pathPoints.Length - currentPosIndex;
    }

    public void Move(int steps)
    {
        if (steps == -1)
        {
            return;
        }

        moveStarted = true;
        if (currentPosIndex + steps > pathPoints.Length)
        {
            Debug.Log("Invalid Move");
            moveStarted = false;
            return;
        }

        maxSteps = steps;
        currentStep = 0;
        player_Path = new Vector3[steps];

        int tempSteps = 0;
        for (int i = 0; i < steps; i++)
        {
            player_Path[i] = pathPoints[currentPosIndex + tempSteps].transform.position;
            tempSteps++;
        }
        // Debug.Log("position = " + player_Path[0]);
        iTween.MoveTo(this.gameObject, iTween.Hash("position", player_Path[0], "speed", animationSpeed, "time", animationTime, "easetype", "easeOutBack", "looptype", "none", "oncomplete", "MoveComplete", "oncompletetarget", this.gameObject));

    }


    public void MoveComplete()
    {
        if (PointsLeft() == 0)
        {
            moveStarted = false;
            isPlayerReachedHome = true;
            LudoManager.instance.diceScript.dontRoll = false;
            LudoManager.instance.IsGameOver(playerType);
            return;

        }

        if (currentStep < maxSteps)
        {
            iTween.MoveTo(this.gameObject, iTween.Hash("position", player_Path[currentStep], "speed", animationSpeed, "time", animationTime, "easetype", "easeOutBack", "looptype", "none", "oncomplete", "MoveComplete", "oncompletetarget", this.gameObject));
            currentPosIndex++;
            currentStep++;

        }
        else
        {
            CheckIfKillingOtherPawn();
            if (OnMoveOver != null && !isExtraTurn)
            {
                OnMoveOver();
            }
            moveStarted = false;
            //Move Over
        }

    }

    public void ResetPosition()
    {
        if (isPlayerOut)
        {
            this.transform.SetParent(parent);
            this.transform.localPosition = startingPos;
            isPlayerOut = false;
        }
    }

    public void OnClickPlayer()
    {
        if (DiceRoll.currentTurn != playerType)
        {
            return;
        }

        if (LudoManager.currentDiceValue == 6)
        {
            isExtraTurn = true;
        }
        else
        {
            isExtraTurn = false;
        }

        if (!isPlayerOut && LudoManager.currentDiceValue == 6)
        {
            this.transform.SetParent(pathPoints[0].transform.parent.transform);
            this.transform.localPosition = startingStar.transform.localPosition;
            isPlayerOut = true;
            CheckIfKillingOtherPawn();
        }
        else if (isPlayerOut)
        {
            Move(LudoManager.currentDiceValue);
        }
        LudoManager.currentDiceValue = -1;
    }

    void CheckIfKillingOtherPawn()
    {
        switch (playerType)
        {
            case Turn.Player1:
                CheckExtraTurn(LudoManager.instance.pawnScriptForPlayer2);
                break;
            case Turn.Player2:
                CheckExtraTurn(LudoManager.instance.pawnScriptForPlayer1);
                break;
        }
    }

    void CheckExtraTurn(List<Player> pawnList)
    {
        for (int i = 0; i < pawnList.Count; i++)
        {
            if (pawnList[i].isPlayerOut && !pawnList[i].isPlayerReachedHome)
            {
                if (currentPosIndex > 0 && !pathPoints[currentPosIndex - 1].name.Contains("Star") && pawnList[i].currentPosIndex > 0)
                {
                    if (pathPoints[currentPosIndex - 1].name == pawnList[i].pathPoints[pawnList[i].currentPosIndex - 1].name)
                    {
                        pawnList[i].ResetPosition();
                        LudoManager.instance.diceScript.dontRoll = false;
                        isExtraTurn = true;
                        break;
                    }
                }
                else
                {
                    if (pawnList[i].currentPosIndex > 0 && currentPosIndex > 0 && pathPoints[currentPosIndex - 1].name == pawnList[i].pathPoints[pawnList[i].currentPosIndex - 1].name
                    || currentPosIndex > 0 && pathPoints[currentPosIndex - 1].name == pawnList[i].startingStar.name
                    || pawnList[i].currentPosIndex > 0 && startingStar.name == pawnList[i].pathPoints[pawnList[i].currentPosIndex - 1].name)
                    {
                        this.transform.localPosition = new Vector3(this.transform.localPosition.x - 10, this.transform.localPosition.y, this.transform.localPosition.z);
                    }
                }
            }
        }
    }

}
