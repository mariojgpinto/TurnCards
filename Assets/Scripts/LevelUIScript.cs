using UnityEngine;

public class LevelUIScript : MonoBehaviour {
    public int levelDifficulty;
    public int levelNumber;

    public UnityEngine.UI.Text text_ID;
    public GameObject image_User;
    //public GameObject image_Min;
    //public UnityEngine.UI.Text text_Min;

	public GameObject image_UserScore;
	public UnityEngine.UI.Text text_score;


    public void SetLevel(int difficulty, int number)
    {
        levelDifficulty = difficulty;
        levelNumber = number;
        text_ID.text = "" + (levelNumber + 1);

        Board board = GameData.boards[difficulty][number];

//        if (board.played) {
            //image_User.SetActive(true);
			image_UserScore.SetActive(true);
			

		if (board.played) {
			text_score.text = "" + board.userMoves;

            if(board.userMoves > board.minMoves) {
				image_UserScore.GetComponent<UnityEngine.UI.Image>().color = new Color(1, 1, 0);
				//image_User.GetComponent<RectTransform>().eulerAngles = new Vector3(0, 0, 0);
            }
            else {
                image_UserScore.GetComponent<UnityEngine.UI.Image>().color = new Color(0, 1, 0);
				//image_User.GetComponent<RectTransform>().eulerAngles = new Vector3(0, 0, 45);
            }
		}
		else{
			text_score.text = "" + board.minMoves;
			image_UserScore.GetComponent<UnityEngine.UI.Image>().color = new Color(1, 1, 1);
		}
//        }
//        else {
//            //image_User.SetActive(false);
//			image_UserScore.SetActive(false);
//        }

        //if (board.minMoves > 0) {
        //    image_Min.SetActive(true);
        //    text_Min.text = "" + board.minMoves;
        //}
        //else {
        //    image_Min.SetActive(false);
        //}
    }

    public void OnButtonPressed()
    {
        Debug.Log("Button(" + levelDifficulty + "," + levelNumber + ")");

        TurnSquaresGame.isRandom = false;

        TurnSquaresGame.boardWidth = levelDifficulty == 0 ? 4 : levelDifficulty == 1 ? 6 : 8;
        TurnSquaresGame.boardHeight = TurnSquaresGame.boardWidth;
		TurnSquaresGame.board_number = levelNumber;


        TurnSquaresGame.boardLevel = levelDifficulty;
        TurnSquaresGame.boardID = levelNumber;
        TurnSquaresGame.boardMatrix = GameData.boards[levelDifficulty][levelNumber].matrix;

        Application.LoadLevel("01-TurnSquaresGame");

    }

    //// Use this for initialization
    //void Start () {

    //}
}
