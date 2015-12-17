using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {
	#region VARIABLES

	#endregion

	#region BUTTONS_CALLBACKS
	public void OnButtonPressed(int id){
		switch(id){
		case 0 : //4x4
            PopulateGUIScript.difficulty = 0;
			PopulateGUIScriptPreLoaded.difficulty = 0;
//            Application.LoadLevel("01-MenuBoards");
			Application.LoadLevel("01-MenuBoardsRuntime");

			//TurnSquaresGame.boardWidth = 4;
			//TurnSquaresGame.boardHeight = 4;

			//Application.LoadLevel("01-TurnSquaresGame");
			break;
		case 1 : //6x6
            PopulateGUIScript.difficulty = 1;
			PopulateGUIScriptPreLoaded.difficulty = 1;
//            Application.LoadLevel("01-MenuBoards");
			Application.LoadLevel("01-MenuBoardsRuntime");

            //TurnSquaresGame.boardWidth = 6;
			//TurnSquaresGame.boardHeight = 6;
			
			//Application.LoadLevel("01-TurnSquaresGame");
			break;
		case 2 : //8x8
            PopulateGUIScript.difficulty = 2;
			PopulateGUIScriptPreLoaded.difficulty = 2;
//            Application.LoadLevel("01-MenuBoards");
			Application.LoadLevel("01-MenuBoardsRuntime");

			//TurnSquaresGame.boardWidth = 8;
			//TurnSquaresGame.boardHeight = 8;
			
			//Application.LoadLevel("01-TurnSquaresGame");
			break;
		case 3 : //STATISTIC
			
			break;
		case 4 : //SETTINGS
			Application.LoadLevel("02-TurnSquaresIdle");
			break;
        case 99: //SPACECAN
#if UNITY_ANDROID && !UNITY_EDITOR
            Application.OpenURL("https://play.google.com/store/apps/details?id=com.BINTERACTIVE.SpaceCan");
#elif UNITY_IPHONE && !UNITY_EDITOR
            Application.OpenURL("https://itunes.apple.com/pt/app/id1040093401");
#else
            Application.OpenURL("https://facebook.com/spacecangame");
#endif
            break;
            default: break;
		}
	}
    #endregion

    #region UNITY_CALLBACKS
    // Use this for initialization
    void Start() {

    }
    //	
    //	// Update is called once per frame
    //	void Update () {
    //	
    //	}

    void OnGUI () {
		if(Input.GetKeyDown(KeyCode.Escape)){
			Application.Quit();
		}
	}
	#endregion
}
