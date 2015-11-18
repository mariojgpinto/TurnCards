using UnityEngine;

public class LevelUIScript : MonoBehaviour {
    public int levelDifficulty;
    public int levelNumber;

    public UnityEngine.UI.Text text_ID;
    public GameObject image_User;
    public UnityEngine.UI.Text text_User;
    public GameObject image_Min;
    public UnityEngine.UI.Text text_Min;


    public void SetLevel(int difficulty, int number)
    {
        levelDifficulty = difficulty;
        levelNumber = number;
        text_ID.text = "" + (levelNumber + 1);

        Board board = GameData.boards[difficulty][number];

        if (board.played) {
            image_User.SetActive(true);
            text_User.text = "" + board.userMoves;
        }
        else {
            image_User.SetActive(false);
        }

        if (board.minMoves > 0) {
            image_Min.SetActive(true);
            text_Min.text = "" + board.minMoves;
        }
        else {
            image_Min.SetActive(false);
        }
    }

    public void OnButtonPressed()
    {
        Debug.Log("Button(" + levelDifficulty + "," + levelNumber + ")");

        TurnSquaresGame.isRandom = false;

        TurnSquaresGame.boardWidth = levelDifficulty == 0 ? 4 : levelDifficulty == 1 ? 6 : 8;
        TurnSquaresGame.boardHeight = TurnSquaresGame.boardWidth;


        TurnSquaresGame.boardLevel = levelDifficulty;
        TurnSquaresGame.boardID = levelNumber;
        TurnSquaresGame.boardMatrix = GameData.boards[levelDifficulty][levelNumber].matrix;

        Application.LoadLevel("01-TurnSquaresGame");

    }

    //// Use this for initialization
    //void Start () {

    //}
}
