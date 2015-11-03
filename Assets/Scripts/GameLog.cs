using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public struct GameData{
	public int boardSizeW;
	public int boardSizeH;
	public string boardID;
	public List<KeyValuePair<int,int>> moves;
	public float startTime;
	public float endTime;
};

public class GameLog : MonoBehaviour {
	#region VARIABLES
	public static string path;

	static GameData currentGame;

	#endregion

	#region LOG_ACCESS
	public static void StartGame(string bID, int sizeW, int sizeH){
		currentGame = new GameData(){
			boardID = bID,
			boardSizeH = sizeH,
			boardSizeW = sizeW,
			moves = new List<KeyValuePair<int,int>>(),
			startTime = Time.realtimeSinceStartup
		};
	}

	public static void AddMove(int moveWidth, int moveHeight){
		currentGame.moves.Add(new KeyValuePair<int, int>(moveWidth, moveHeight));
	}

	public static void EndGame(){
		currentGame.endTime = Time.realtimeSinceStartup;
		AddData();
	}
	#endregion

	#region DATA_PERSISTENCE
	static void AddData(){
		string str = "";

		str += 	currentGame.boardSizeW + "|" + 
				currentGame.boardSizeH + "|" + 
				currentGame.boardID + "|" + 
				(currentGame.endTime - currentGame.startTime) + "|" + 
				currentGame.moves.Count + "|";

		for(int i = 0 ; i < currentGame.moves.Count ; ++i){
			str += "(" + currentGame.moves[i].Key + ";" + currentGame.moves[i].Value + ")";
		}

		str += "\n";


		File.AppendAllText(GameLog.path, str);
	}
	#endregion

	#region UNITY_CALLBACKS
	void Awake () {
		path = Application.persistentDataPath + "/game.dat";

		Debug.Log("Path:" + path);
	}
	#endregion
}
