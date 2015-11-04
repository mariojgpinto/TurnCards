using UnityEngine;
using System.Collections.Generic;

public class Board {
	public enum STATUS{
		NEVER = 0,
		COMPLETED = 1,
		STARED = 2
	}
	public string ID;
	public int boardSize;

    public int minMoves;

	STATUS status = STATUS.NEVER;
}

public class GameData : MonoBehaviour {
    #region VARIABLES
    public static GameData instance = null;

	public static List<Board> allBoards = new List<Board>();
    public static List<Board>[] boards = new List<Board>[3];
    public static List<LevelUIScript> uiLevels = new List<LevelUIScript>();

    static string[] boardsFiles = new string[] {
        "Boards/boards4x4",
        "Boards/boards6x6",
        "Boards/boards8x8"
    };
    #endregion

    #region LOAD_DATA
    static void LoadData() {
        LoadBoards(0, 4);
        LoadBoards(1, 6);
        LoadBoards(2, 8);
    }

    static void LoadBoards(int idx, int size) {
        TextAsset board4 = Resources.Load(boardsFiles[idx]) as TextAsset;

        string[] auxBoards = board4.text.Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

        boards[idx] = new List<Board>();

		foreach(string board in auxBoards) {
            string[] auxBoard= board.Split(new char[] { ';' }, System.StringSplitOptions.RemoveEmptyEntries);

            Debug.Log(auxBoard.Length);
            if(auxBoard.Length > 1) {
                allBoards.Add(new Board {
                    ID = auxBoard[0],
                    minMoves = System.Convert.ToInt32(auxBoard[1]),
                    boardSize = size
                });
            }
            else {
                allBoards.Add(new Board {
                    ID = board,
                    boardSize = size
                });
            }

            boards[idx].Add(allBoards[allBoards.Count-1]);
		}
    }
    #endregion

    #region UNITY_CALLBACKS
    void Awake ()
    {
        if (GameData.instance != null)
            Destroy(this);

        GameData.instance = this;
    }
    // Use this for initialization
    void Start () {
        GameData.LoadData();
    }
	
	//// Update is called once per frame
	//void Update () {
	
	//}
    #endregion
}
