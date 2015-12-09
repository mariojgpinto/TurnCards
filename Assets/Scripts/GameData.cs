using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Board {
    public int id;
	public string matrix;
	public int boardSizeW;
    public int boardSizeH;

    public bool played = false;
    public int minMoves = 0;
    public int userMoves = 0;
}

public class GameData {
    #region VARIABLES
    public static GameData instance = null;

	public static List<Board> allBoards = new List<Board>();
    public static List<Board>[] boards = new List<Board>[3];
    public static List<LevelUIScript> uiLevels = new List<LevelUIScript>();

    static string[] originalBoardsFiles = new string[] {
        "Boards/boards4x4",
        "Boards/boards6x6",
        "Boards/boards8x8"
    };
    static string[] boardsFiles = new string[] {
        "boards4x4.dat",
        "boards6x6.dat",
        "boards8x8.dat"
    };

    static string boardsFile = "boards.dat";
    static string splitMarker = "|";
    #endregion

    #region LOAD_DATA
    static void LoadData() {
        if(System.IO.File.Exists(boardsFile)) {
            LoadBoards();
        }
        else {
            LoadInitialBoards(0, 4);
            LoadInitialBoards(1, 6);
            LoadInitialBoards(2, 8);

            SaveBoards();
        }
    }

    static void LoadBoards() {
        string txt = System.IO.File.ReadAllText(boardsFile);

        if (txt == null || txt == "")
            return;

        string[] txtBoards = txt.Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

        for(int i = 0; i < boardsFiles.Length; ++i) {
            boards[i] = new List<Board>();
        }        

        for (int i = 0; i < txtBoards.Length; ++i) {
            string txtBoardInfo = txtBoards[i];

            if (txtBoardInfo != null && txtBoardInfo != "") {
                string[] info = txtBoardInfo.Split(new string[] { splitMarker }, System.StringSplitOptions.RemoveEmptyEntries);

                if (info != null && info.Length >= 6) {
                    Board board = new Board();

                    board.boardSizeW = System.Convert.ToInt32(info[0]);
                    board.boardSizeH = System.Convert.ToInt32(info[1]);
                    board.played = info[2] == "1";
                    board.userMoves = System.Convert.ToInt32(info[3]);
                    board.minMoves = System.Convert.ToInt32(info[4]);
                    board.matrix = info[5];

                    bool added = false;
                    if (board.boardSizeW == 4 && boards[0].Count < 150) {
                        board.id = boards[0].Count;
                        boards[0].Add(board);
                        added = true;
                    }
                    else if (board.boardSizeW == 6 && boards[1].Count < 150) {
                        board.id = boards[1].Count;
                        boards[1].Add(board);
                        added = true;
                    }
                    else if (board.boardSizeW == 8 && boards[2].Count < 150) {
                        board.id = boards[2].Count;
                        boards[2].Add(board);
                        added = true;
                    }

                    if (added) {
                        allBoards.Add(board);
                    }
                }
            }           
        }        
    }

    static void LoadInitialBoards(int idx, int size) {
        TextAsset board4 = Resources.Load(originalBoardsFiles[idx]) as TextAsset;

        string[] auxBoards = board4.text.Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

        boards[idx] = new List<Board>();

        foreach (string board in auxBoards) {
            string[] auxBoard = board.Split(new string[] { splitMarker }, System.StringSplitOptions.RemoveEmptyEntries);

            Debug.Log(auxBoard.Length);
            if (auxBoard.Length > 1) {
                allBoards.Add(new Board {
                    matrix = auxBoard[0],
                    minMoves = System.Convert.ToInt32(auxBoard[1]),
                    boardSizeW = size,
                    boardSizeH = size
                });
            }
            else {
                allBoards.Add(new Board {
                    matrix = board,
                    boardSizeW = size,
                    boardSizeH = size
                });
            }

            boards[idx].Add(allBoards[allBoards.Count - 1]);
        }
    }
    #endregion

    #region SAVE_DATA
    public static void SaveBoards() {
        string str = "";
        for(int i = 0; i < allBoards.Count; ++i) {
            Board board = allBoards[i];

            str +=
                board.boardSizeW + splitMarker +
                board.boardSizeH + splitMarker +
                (board.played ? "1" : "0") + splitMarker +
                board.userMoves + splitMarker +
                board.minMoves + splitMarker +
                board.matrix + splitMarker +
                "\n";
        }

        System.IO.File.WriteAllText(boardsFile, str);
    }

    public static void SaveBoard(int level, int boardID) {
        //switch (level) {

        //}
        //SaveBoards();
    }
    #endregion

    #region FUNCTIONS
    static public void Initialize ()
    {
        for (int i = 0; i < boardsFiles.Length; ++i) {
            boardsFiles[i] = Application.streamingAssetsPath + "/" + boardsFiles[i];
        }

        boardsFile = Application.persistentDataPath + "/" + boardsFile;

        Debug.Log(boardsFile);

        GameData.LoadData();

        Debug.Log("TotalBoads: " + allBoards.Count);
    }

    public static int GetBoardIndex(string boardMatrix) {
        for(int i = 0; i < allBoards.Count; ++i) {
            if(allBoards[i].matrix == boardMatrix) {
                return i;
            }
        }
        return -1;
    }

    //// Use this for initialization
    //void Start () {
        
    //}
	
	//// Update is called once per frame
	//void Update () {
	
	//}
    #endregion
}
