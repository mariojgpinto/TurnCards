using UnityEngine;
using System.Collections;

public class GameDataWWW : MonoBehaviour {
    #region VARIABLES
    static GameDataWWW instance;

    public const string _id = "MACRO_ID";
    public const string _n_moves = "MACRO_N_MOVES";
    public const string _time = "MACRO_TIME";
    public const string _moves = "MACRO_MOVES";

    static string wwwGetBoardInfo = "http://www.binteractive.pt/ricardo/jogomario/getjogo.php?id=MACRO_ID";
    static string wwwGetAllBoardsInfo = "http://www.binteractive.pt/ricardo/jogomario/getjogo.php";
    static string wwwSendBoardInfo = "http://www.binteractive.pt/ricardo/jogomario/insert-jogada.php?jogo=MACRO_ID&movimentos=MACRO_N_MOVES&tempo=MACRO_TIME&jogadas=MACRO_MOVES";

	static string localTempFile = "tempInfo.dat";
    #endregion

    public static void UpdateBoardInfo(string matrix, int n_moves, int time, string moves = "") {
		if(Application.internetReachability == NetworkReachability.NotReachable){
			SaveInFile(matrix, n_moves, time, moves);
			GameData.SaveBoards();
		}
		else{
        	instance.StartCoroutine(UpdateBoardInfo_routine(matrix, n_moves, time, moves));
			StartCheck();
		}
    }

    public static void GetAllInfo() {
		if(Application.internetReachability != NetworkReachability.NotReachable){
        	instance.StartCoroutine(GetAllInfo_routine());
		}
    }

    public static void GetBoardInfo(string matrix) {
		if(Application.internetReachability != NetworkReachability.NotReachable){
        	instance.StartCoroutine(GetBoardInfo_routine(matrix));
		}
    }

	public static void StartCheck(){
		if(System.IO.File.Exists(localTempFile)){
			if(Application.internetReachability != NetworkReachability.NotReachable){
				string[] allText = System.IO.File.ReadAllLines(localTempFile);
				System.IO.File.Delete(localTempFile);

				if(allText != null && allText.Length > 0){
					foreach(string url in allText){
						instance.StartCoroutine(SendBoardInfo_routine(url));
					}
				}
			}
		}
	}


	static void SaveInFile(string matrix, int n_moves, int time, string moves){
		int id = GameData.GetBoardIndex(matrix);
		if (GameData.allBoards[id].minMoves == 0 || GameData.allBoards[id].minMoves >= n_moves)
			GameData.allBoards[id].minMoves = n_moves;
				
		System.IO.File.AppendAllText(
			localTempFile,
			GameDataWWW.wwwSendBoardInfo.
			Replace(_id, matrix).
			Replace(_n_moves, ""+n_moves).
			Replace(_time, "" + time).
			Replace(_moves, moves) + "\n"
		);
	}

	static IEnumerator SendBoardInfo_routine(string url) {
		WWW www = new WWW(url);
		yield return www;

		if (www == null || www.error != null) {
			System.IO.File.AppendAllText(
				localTempFile,
				url
			);
		}
	}

    static IEnumerator UpdateBoardInfo_routine(string matrix, int n_moves, int time, string moves) {
        WWW www = new WWW(GameDataWWW.wwwSendBoardInfo.
            Replace(_id, matrix).
            Replace(_n_moves, ""+n_moves).
            Replace(_time, "" + time).
            Replace(_moves, moves));

        yield return www;

        int id = GameData.GetBoardIndex(matrix);
        if (id >= 0) {
            if (www != null) {
                if (www.error != "") {
                    Debug.Log("Informations Sent: " + www.text + " - " + www.url);

                    if (GameData.allBoards[id].minMoves == 0 || GameData.allBoards[id].minMoves >= n_moves)
                        GameData.allBoards[id].minMoves = n_moves;
                }
                else {
                    Debug.Log("UpdateBoardInfo Error: " + www.error);
					SaveInFile(matrix, n_moves, time, moves);
                }
            }
            else {
                Debug.Log("UpdateBoardInfo Error: NULL");
				SaveInFile(matrix, n_moves, time, moves);
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

        //GameObject.Find("Text_Log").GetComponent<UnityEngine.UI.Text>().text += "TEST3-" + (www==null) + "\n";
        //GameObject.Find("Text_Log").GetComponent<UnityEngine.UI.Text>().text += "TEST4-" + www.error + "\n";
        //GameObject.Find("Text_Log").GetComponent<UnityEngine.UI.Text>().text += "TEST5-" + www.url+ "\n";
        //GameObject.Find("Text_Log").GetComponent<UnityEngine.UI.Text>().text += "TEST5-" + www.text + "\n";
        try {
            if (www.text != "") {
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
//                            else
//                            if (min > GameData.allBoards[tempIdx].minMoves) {
//                                GameDataWWW.UpdateBoardInfo(
//                                    GameData.allBoards[tempIdx].matrix,
//                                    GameData.allBoards[tempIdx].minMoves,
//                                    -1);
//                            }
                        }
                    }
                }

                Debug.Log(www.text);
            }           
        }
        catch (System.Exception e) {
            Debug.Log("Exception: " + e.ToString());
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
		localTempFile = Application.persistentDataPath + "/tempInfo.dat";
		StartCheck();
		GetAllInfo();
	}
	
	// Update is called once per frame
	//void Update () {
	
	//}
    #endregion
}
