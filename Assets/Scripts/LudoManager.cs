using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class LudoManager : MonoBehaviour
{
    public static LudoManager instance;
    public PlayerPositionManager[] players;
    public Sprite[] playerSprite;
    public GameObject playerPrefab;

    public const int numOfPawnPerPlayer = 4;

    public List<Player> pawnScriptForPlayer1 = new List<Player>();
    public List<Player> pawnScriptForPlayer2 = new List<Player>();

    public static int currentDiceValue;

    public Text gameOverText;

    public DiceRoll diceScript;
    // Start is called before the first frame update

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    void Start()
    {
        DiceRoll.OnDiceRolled += SetCurrentDiceValue;
        switch (GameManager.instance.currentColorMode)
        {
            case ColorMode.BlueRed:
                GeneratePlayerPieces(0, pawnScriptForPlayer1);
                GeneratePlayerPieces(2, pawnScriptForPlayer2);
                break;
            case ColorMode.YellowGreen:
                GeneratePlayerPieces(1, pawnScriptForPlayer1);
                GeneratePlayerPieces(3, pawnScriptForPlayer2);
                break;
        }
    }

    void SetCurrentDiceValue(int diceVal)
    {
        currentDiceValue = diceVal;
    }

    void GeneratePlayerPieces(int index, List<Player> pawnScriptForPlayer)
    {
        for (int i = 0; i < numOfPawnPerPlayer; i++)
        {
            GameObject pawn = PhotonNetwork.Instantiate(playerPrefab.name, Vector3.zero, Quaternion.identity, 0);
            pawn.transform.SetParent(players[index].transform);
            pawn.transform.localPosition = players[index].startPoints[i];
            pawn.transform.localScale = Vector3.one;
            pawn.name = players[index].name + i;
            Player playerScript = pawn.GetComponent<Player>();
            playerScript.startingPos = players[index].startPoints[i];
            playerScript.playerImg.sprite = playerSprite[index];
            playerScript.pathPoints = players[index].pathPoints;
            playerScript.parent = players[index].transform;
            playerScript.startingStar = players[index].startingStar;
            pawnScriptForPlayer.Add(playerScript);
            playerScript.playerType = (Turn)index;
        }
    }
    public bool isMoveValid(int num)
    {
        if (num == 6)
        {
            diceScript.dontRoll = false;
            return true;
        }
        if (DiceRoll.currentTurn == Turn.Player1)
        {
            for (int i = 0; i < pawnScriptForPlayer1.Count; i++)
            {
                if (pawnScriptForPlayer1[i].isPlayerOut)
                {
                    CheckAutoMove(pawnScriptForPlayer1);
                    return true;
                }
            }
        }
        else
        {
            for (int i = 0; i < pawnScriptForPlayer2.Count; i++)
            {
                if (pawnScriptForPlayer2[i].isPlayerOut)
                {
                    CheckAutoMove(pawnScriptForPlayer2);
                    return true;
                }
            }
        }
        return false;
    }

    void CheckAutoMove(List<Player> pawnList)
    {
        int numOfPawnsOut = 0;
        int indexOfPawnOut = -1;
        for (int i = 0; i < pawnList.Count; i++)
        {
            if (pawnList[i].isPlayerOut && !pawnList[i].isPlayerReachedHome)
            {
                indexOfPawnOut = i;
                numOfPawnsOut++;
            }
        }
        if (numOfPawnsOut == 1)
        {
            pawnList[indexOfPawnOut].OnClickPlayer();
        }
    }

    public void OnClickBack()
    {
        GameManager.instance.OnClickBack();
    }

    void CheckGameOver(Turn currentTurn)
    {
        if (IsGameOver(currentTurn))
        {
            gameOverText.gameObject.SetActive(true);
            gameOverText.text = currentTurn + " WINS !!!";
        }
    }

    public bool IsGameOver(Turn currentTurn)
    {
        switch (currentTurn)
        {
            case Turn.Player1:
                return AreAllPawnsHome(pawnScriptForPlayer1);
            case Turn.Player2:
                return AreAllPawnsHome(pawnScriptForPlayer2);
        }
        return false;
    }

    bool AreAllPawnsHome(List<Player> pawnList)
    {
        int numberOfPawsHome = 0;
        for (int i = 0; i < pawnList.Count; i++)
        {
            if (pawnList[i].isPlayerReachedHome)
            {
                numberOfPawsHome++;
            }
        }
        if (numberOfPawsHome == numOfPawnPerPlayer)
        {
            diceScript.dontRoll = true;
            return true;
        }
        return false;
    }
}
