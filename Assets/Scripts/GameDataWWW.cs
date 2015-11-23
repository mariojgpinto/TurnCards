using UnityEngine;
using System.Collections;

public class GameDataWWW : MonoBehaviour {
    #region VARIABLES
    static GameDataWWW instance;

    public const string _id = "MACRO_ID";
    public const string _moves = "MACRO_MOVES";
    public const string _time = "MACRO_TIME";

    static string wwwGetBoardInfo = "www.binteractive.pt/ricardo/jogomario/getjogo.php?id=MACRO_ID";
    static string wwwGetAllBoardsInfo = "www.binteractive.pt/ricardo/jogomario/getjogo.php";
    static string wwwSendBoardInfo = "www.binteractive.pt/ricardo/jogomario/insert-jogada.php?jogo=MACRO_ID&movimentos=MACRO_MOVES&tempo=MACRO_TIME";
    #endregion

    public static void UpdateBoardInfo(string matrix, int moves, int time) {
        instance.StartCoroutine(UpdateBoardInfo_routine(matrix, moves, time));
    }

    public static void GetAllInfo() {
        instance.StartCoroutine(GetAllInfo_routine());
    }

    public static void GetBoardInfo(string matrix) {
        instance.StartCoroutine(GetBoardInfo_routine(matrix));
    }




    static IEnumerator UpdateBoardInfo_routine(string matrix, int moves, int time) {
        WWW www = new WWW(GameDataWWW.wwwSendBoardInfo.Replace(_id, matrix).Replace(_moves, ""+moves).Replace(_time, "" + time));
        yield return www;

        int id = GameData.GetBoardIndex(matrix);
        if (id >= 0) {
            if (www != null) {
                if (www.text != "") {
                    Debug.Log("Informations Sent: " + www.text);
                    GameData.allBoards[id].sync = true;

                    if (GameData.allBoards[id].minMoves == 0 || GameData.allBoards[id].minMoves >= moves)
                        GameData.allBoards[id].minMoves = moves;
                }
                else {
                    Debug.Log("UpdateBoardInfo Error: " + www.error);
                    GameData.allBoards[id].sync = false;
                }
            }
            else {
                Debug.Log("UpdateBoardInfo Error: NULL");
                GameData.allBoards[id].sync = false;
            }
        }

        if(time != -1)
            GameData.SaveBoards();
    }

    static IEnumerator GetBoardInfo_routine(string matrix) {
        WWW www = new WWW(GameDataWWW.wwwGetBoardInfo.Replace(_id,matrix));
        yield return www;

        if (www.text != "") {
            try {
                JSONObject js = new JSONObject(www.text);

                for (int i = 0; i < js.Count; ++i) {
                    string boardID = js[i].GetField("id").ToString().Replace("\"", "");
                    string strMin = js[i].GetField("minimo").ToString().Replace("\"", "");

                    int min = System.Convert.ToInt32(strMin);

                    if (min > 0) {
                        int tempIdx = GameData.GetBoardIndex(boardID);

                        if (tempIdx >= 0 && tempIdx < GameData.allBoards.Count) {
                            if (min < GameData.allBoards[tempIdx].minMoves) {
                                GameData.allBoards[tempIdx].minMoves = min;
                            }
                        }
                    }
                }

                Debug.Log(www.text);
            }
            catch (System.Exception e) {
                Debug.Log("Exception: " + e.ToString());
            }
        }
    }

    static IEnumerator GetAllInfo_routine() {
        WWW www = new WWW(GameDataWWW.wwwGetAllBoardsInfo);
        yield return www;

        if (www.text != "") {
            try {
                JSONObject js = new JSONObject(www.text);

                for (int i = 0; i < js.Count; ++i) {
                    string boardID = js[i].GetField("id").ToString().Replace("\"", "");
                    string strMin = js[i].GetField("minimo").ToString().Replace("\"", "");

                    int min = System.Convert.ToInt32(strMin);

                    if (min > 0) {
                        int tempIdx = GameData.GetBoardIndex(boardID);

                        if (tempIdx >= 0 && tempIdx < GameData.allBoards.Count) {
                            if (min < GameData.allBoards[tempIdx].minMoves || GameData.allBoards[tempIdx].minMoves == 0) {
                                GameData.allBoards[tempIdx].minMoves = min;
                            }
                            else
                            if (min > GameData.allBoards[tempIdx].minMoves) {
                                GameDataWWW.UpdateBoardInfo(
                                    GameData.allBoards[tempIdx].matrix,
                                    GameData.allBoards[tempIdx].minMoves,
                                    -1);
                            }
                        }
                    }
                }

                Debug.Log(www.text);
            }
            catch (System.Exception e) {
                Debug.Log("Exception: " + e.ToString());
            }
        }
    }

    #region UNITY_CALLBACKS
    void Awake () {
        // if the singleton hasn't been initialized yet
        if (instance != null) {
            Destroy(this.gameObject);
            return;//Avoid doing anything else
        }

        if (instance == this) {
            return;
        }

        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    // Use this for initialization
    void Start () {
        StartCoroutine(GetAllInfo_routine());
	}
	
	// Update is called once per frame
	//void Update () {
	
	//}
    #endregion
}
