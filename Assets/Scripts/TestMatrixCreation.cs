using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TestMatrixCreation : MonoBehaviour {
	#region VARIABLES	
	public string currentBoard = "";
	
	public const int boardWidth = 8;
	public const int boardHeight = 8;

	bool[,] cards = new bool[boardWidth, boardHeight];

	int nBoards = 10000;
	int counter;

	public Text text_board;
	public Text text_counter;

	Dictionary<string,int> dict = new Dictionary<string, int>();
	#endregion
	
	#region SETUP	

	
	string fullStr = "";
	string path = "C:/Dev/Data/boards.txt";

	void GenerateRandomMatrix(){
		string str = "";
		
		for (int j = 0; j < boardHeight; ++j){
			for (int i = 0; i < boardWidth; ++i){
				if(Random.Range(0,100) > 70){
					cards[i,j] = true;
					str += "1";
				}
				else{
					cards[i,j] = false;
					str += "0";
				}
			}
		}

		if(CheckIfIsValid()){
			if(dict.ContainsKey(str)){
				int temp = 0;
				dict.TryGetValue(str, out temp);

				dict.Remove(str);

				dict.Add(str, temp + 1);
				Debug.Log("Repeated");
			}
			else{
				dict.Add(str, 1);

				fullStr += str + "\n";
			}
		}
	}

	bool CheckIfIsValid(){
		int ac = 0;

		ac = 0;
		for (int j = 0; j < boardHeight; ++j){
			if(!cards[0,j])
				ac++;
			else 
				break;
		}
		if(ac == boardHeight)
			return false;

		ac = 0;
		for (int j = 0; j < boardHeight; ++j){
			if(!cards[boardWidth-1,j])
				ac++;
			else 
				break;
		}
		if(ac == boardHeight)
			return false;

		ac = 0;
		for (int i = 0; i < boardWidth; ++i){
			if(!cards[i,0])
				ac++;
			else 
				break;
		}
		if(ac == boardWidth)
			return false;

		ac = 0;
		for (int i = 0; i < boardWidth; ++i){
			if(!cards[i,boardHeight-1])
				ac++;
			else 
				break;
		}
		if(ac == boardWidth)
			return false;

		return true;
	}
	#endregion

	void CreateBoards(){
//		for( ; counter < nBoards ; ++counter){
//			counter++;
//
//			if(!GenerateRandomMatrix())
//				counter--;
//
//			Debug.Log("Counter " + counter);
//		}

		while(dict.Count < nBoards){
			GenerateRandomMatrix();

//			Debug.Log("Counter " + dict.Count);
		}

		Debug.Log("Counter " + dict.Count);
	}
	
	#region UNITY_CALLBACKS
	// Use this for initialization
	void Start () {
		CreateBoards();

		System.IO.File.WriteAllText(path, fullStr);
	}
	
	// Update is called once per frame	
	void OnGUI(){
		if(Input.GetKeyDown(KeyCode.Escape)){
			Application.LoadLevel("00-Menu");
		}
	}
	#endregion
}
