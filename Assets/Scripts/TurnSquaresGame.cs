using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TurnSquaresGame : MonoBehaviour {
	#region VARIABLES
	public static bool isRandom = true;

    public static int boardLevel;
    public static int boardID;

    public static string boardMatrix = "";

	public string currentBoard = "";

	public static int boardWidth = 6;
	public static int boardHeight = 6;

	int currentBoardMinWidth;
	int currentBoardMaxWidth;
	int currentBoardMinHeight;
	int currentBoardMaxHeight;
	float step = 2.5f;

	float waitTimeStep = .1f;

	TurnCard[,] cards;

	int scoreCount = 0;

	bool isBusy = false;
    bool retry = false;

    float timeInit = 0;

    public GameObject panelScore;
    public GameObject panelCurrentScore;
    
    public Text textCurrentScore;
    public Text textBoardID;
    public Text textFinalScore;
    public Text textTimerCountdown;

    #endregion

    #region SETUP
    void CreateCardMatrix()
	{
		GameObject parent = new GameObject();
		parent.name = "Cards";

		cards = new TurnCard[boardWidth,boardHeight];

		for (int i = 0; i < boardWidth; ++i)
		{
			for (int j = 0; j < boardHeight; ++j)
			{
				GameObject go = Instantiate(Resources.Load("Prefabs/Card") as GameObject);
				go.transform.position = new Vector3(i * step, j * step, 0);
				cards[i,j] = go.GetComponent<TurnCard>();
				go.transform.parent = parent.transform;
				cards[i,j].Init(i,j);
			}
		}		
	}
	
	void AdjustCamera(){
		Camera cam = GameObject.Find("Main Camera").GetComponent<Camera>();
		
		cam.transform.position = new Vector3(((boardWidth - 1) * step) / 2,
		                                     ((boardHeight - 1) * step) / 2,
		                                     -10);
		
		float v1 = ((boardWidth) * step);
		float v3 = cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0)).x;
		float orthograficStep = v3 / 20f;        
		int ac = 0;
		while (v3 < v1 && ac < 100)
		{
			ac++;
			cam.orthographicSize += orthograficStep;
			v3 = cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0)).x;
		}
	}

	void GenerateRandomMatrix(){
		currentBoardMaxWidth = boardWidth;
		currentBoardMaxHeight = boardHeight;
		currentBoardMinWidth = 0;
		currentBoardMinHeight = 0;

		scoreCount = 0;
        panelCurrentScore.SetActive(true);
        UpdateGUI_MovesCount();

		string str = "";

		for (int j = 0; j < boardHeight; ++j){
			for (int i = 0; i < boardWidth; ++i){
				cards[i,j].ResetCard();
				if(Random.Range(0,100) > 70){
					cards[i,j].SetWhite();
					str += "1";
				}
				else{
					cards[i,j].SetBlack();
					str += "0";
				}
			}
		}


		if(!CheckIfIsValid()) {
			GenerateRandomMatrix();
		}
		else{
			boardMatrix = str;
			UpdateGUI_BoardID();

			StartCoroutine(ResizeBorders(.5f));
		}

		GameLog.StartGame(str,boardWidth, boardHeight);

        panelScore.SetActive(false);
    }

    void GenerateNextMatrix() {
        if(boardID < GameData.boards[boardLevel].Count -1 ) {
            boardID++;
            
            boardMatrix = GameData.boards[boardLevel][boardID].matrix;

            LoadBoardFromID(boardMatrix);
        }
    }

    bool CheckIfIsValid(){
		int ac = 0;
		
		ac = 0;
		for (int j = 0; j < boardHeight; ++j){
			if(cards[0,j].isBack)
				ac++;
			else 
				break;
		}
		if(ac == boardHeight)
			return false;
		
		ac = 0;
		for (int j = 0; j < boardHeight; ++j){
			if(cards[boardWidth-1,j].isBack)
				ac++;
			else 
				break;
		}
		if(ac == boardHeight)
			return false;
		
		ac = 0;
		for (int i = 0; i < boardWidth; ++i){
			if(cards[i,0].isBack)
				ac++;
			else 
				break;
		}
		if(ac == boardWidth)
			return false;
		
		ac = 0;
		for (int i = 0; i < boardWidth; ++i){
			if(cards[i,boardHeight-1].isBack)
				ac++;
			else 
				break;
		}
		if(ac == boardWidth)
			return false;
		
		return true;
	}

	void LoadBoardFromID(string id){
		currentBoardMaxWidth = boardWidth;
		currentBoardMaxHeight = boardHeight;
		currentBoardMinWidth = 0;
		currentBoardMinHeight = 0;
		
		scoreCount = 0;
        panelCurrentScore.SetActive(true);
        UpdateGUI_MovesCount();

		int x = 0 ; 
		int y = 0;
		for(int i = 0 ; i < id.Length ; ++i){
			cards[x,y].ResetCard();
			if(id[i] == '0'){
				cards[x,y].SetBlack();
			}else{
				cards[x,y].SetWhite();
			}

			x++;

			if(x >= boardWidth){
				x = 0;
				y++;
			}
		}

		UpdateGUI_BoardID();

		GameLog.StartGame(id,boardWidth, boardHeight);

        panelScore.SetActive(false);

        StartCoroutine(ResizeBorders(.5f));
	}
	#endregion

	#region GAME_LOGIC
	void ProcessTouch(float touchX, float touchY)
	{
		Ray ray = Camera.main.ScreenPointToRay(new Vector3(touchX, touchY, 0));
		RaycastHit hit;
		
		if (Physics.Raycast(ray, out hit) && hit.transform.gameObject.name.StartsWith("Card"))
		{
			TurnCard card = hit.collider.GetComponent<TurnCard>();

			int x = card.posX;
			int y = card.posY;

			if(x < currentBoardMaxWidth && x >= currentBoardMinWidth && 
			   y < currentBoardMaxHeight && y >= currentBoardMinHeight){

				isBusy = true;

				card.FlipCard();

				PropagateTouch(x+1,y  ,1, waitTimeStep);
				PropagateTouch(x  ,y+1,3, waitTimeStep);
				PropagateTouch(x-1,y  ,5, waitTimeStep);
				PropagateTouch(x  ,y-1,7, waitTimeStep);

				StartCoroutine(ResizeBorders());

				StartCoroutine(WaitNext());

				++scoreCount;

				UpdateGUI_MovesCount();

				GameLog.AddMove(x,y);
			}
		}
	}

	void PropagateTouch(int x, int y, int direction, float waitTime){
		if(x < currentBoardMaxWidth && 
		   x >= currentBoardMinWidth && 
		   y < currentBoardMaxHeight && 
		   y >= currentBoardMinHeight &&
		   cards[x,y].isActive){
			cards[x,y].FlipCard(waitTime);

			switch(direction){
			case 1: //UP
				PropagateTouch(x+1,y  ,1, waitTime + waitTimeStep);
				break;
			case 3: //RIGHT
				PropagateTouch(x  ,y+1,3, waitTime + waitTimeStep);
				break;
			case 5: //DOWN
				PropagateTouch(x-1,y  ,5, waitTime + waitTimeStep);
				break;
			case 7: //LEFT
				PropagateTouch(x  ,y-1,7, waitTime + waitTimeStep);
				break;
			default: break;
			}
		}
	}

	IEnumerator ResizeBorders(float waitTime = 0){
		yield return new WaitForSeconds(waitTime);

	Init:
		for(int i = currentBoardMinWidth ; i < currentBoardMaxWidth ; ++i){
			for(int j = currentBoardMinHeight ; j < currentBoardMaxHeight ; ++j){
				if(cards[i,j].isBusy){
					yield return new WaitForSeconds(waitTimeStep);
					goto Init;
				}
			}
		}

		int ac;

	if(currentBoardMinHeight >= currentBoardMaxHeight || currentBoardMinWidth >= currentBoardMaxWidth){
		goto EndGame;
	} 

	InitBottom:		
		ac = 0;
		//BOTTOM LINE
		for(int i = currentBoardMinWidth ; i < currentBoardMaxWidth ; ++i){
			if(!cards[i,currentBoardMinHeight].isBack)
				break;
			else
				ac++;
		}
		if(ac == (currentBoardMaxWidth - currentBoardMinWidth)){
			for(int i = currentBoardMinWidth ; i < currentBoardMaxWidth ; ++i){
				cards[i,currentBoardMinHeight].HideCard();
			}
			currentBoardMinHeight++;
			if(currentBoardMinHeight >= currentBoardMaxHeight){
				goto EndGame;
			} else {
				goto InitBottom;
			}
			
		}


	InitTop:
		ac = 0;
		//TOP LINE
		for(int i = currentBoardMinWidth ; i < currentBoardMaxWidth ; ++i){
			if(!cards[i,currentBoardMaxHeight-1].isBack)
				break;
			else
				ac++;
		}
		if(ac == (currentBoardMaxWidth - currentBoardMinWidth)){
			for(int i = currentBoardMinWidth ; i < currentBoardMaxWidth ; ++i){
				cards[i,currentBoardMaxHeight-1].HideCard();
			}
			currentBoardMaxHeight--;
			if(currentBoardMaxHeight <= currentBoardMinHeight){
				goto EndGame;
			}else{
				goto InitTop;
			}
		}

	InitLeft:
		ac = 0;		
		//LEFT COLUMN
		for(int i = currentBoardMinHeight ; i < currentBoardMaxHeight ; ++i){
			if(!cards[currentBoardMinWidth, i].isBack)
				break;
			else
				ac++;
		}
		if(ac == (currentBoardMaxHeight - currentBoardMinHeight)){
			for(int i = currentBoardMinHeight ; i < currentBoardMaxHeight ; ++i){
				cards[currentBoardMinWidth, i].HideCard();
			}
			currentBoardMinWidth++;
			if(currentBoardMinWidth >= currentBoardMaxWidth){
				goto EndGame;
			}else{
				goto InitLeft;
			}
		}

	InitRight:
		//RIGHT COLUMN
		ac = 0;
		for(int i = currentBoardMinHeight ; i < currentBoardMaxHeight ; ++i){
			if(!cards[currentBoardMaxWidth-1,i].isBack)
				break;
			else
				ac++;
		}
		if(ac == (currentBoardMaxHeight - currentBoardMinHeight)){
			for(int i = currentBoardMinHeight ; i < currentBoardMaxHeight ; ++i){
				cards[currentBoardMaxWidth-1,i].HideCard();
			}
			currentBoardMaxWidth--;
			if(currentBoardMaxWidth <= currentBoardMinWidth){
				goto EndGame;
			}else{
				goto InitRight;
			}
		}

//		GameObject.Find("Text_Moves").GetComponent<Text>().text = "" + 
//			 "Width[" + currentBoardMinWidth  + "," + currentBoardMaxWidth  + "]\n" + 
//			"Height[" + currentBoardMinHeight + "," + currentBoardMaxHeight + "]\n";

		yield break;

	EndGame:
		EndGame();
		GameLog.EndGame();
        GameDataWWW.UpdateBoardInfo(
            boardMatrix,
            scoreCount,
            GameLog.GetTime()
        );

        GameData.SaveBoard(boardLevel, boardID);


        textTimerCountdown.text = "Next\nIn 5";
        if (retry)
            goto EndFunc;
        yield return new WaitForSeconds(1);
        if (retry)
            goto EndFunc;
        textTimerCountdown.text = "Next\nIn 4";
        yield return new WaitForSeconds(1);
        if (retry)
            goto EndFunc;
        textTimerCountdown.text = "Next\nIn 3";
        yield return new WaitForSeconds(1);
        if (retry)
            goto EndFunc;
        textTimerCountdown.text = "Next\nIn 2";
        yield return new WaitForSeconds(1);
        if (retry)
            goto EndFunc;
        textTimerCountdown.text = "Next\nIn 1";
        yield return new WaitForSeconds(1);
        if (retry)
            goto EndFunc;
        textTimerCountdown.text = "Next\nIn 0";
        yield return new WaitForSeconds(.2f);


        if (retry) {
            goto EndFunc;
        }
        else {
            if (isRandom) {
                GenerateRandomMatrix();
            }
            else {
                GenerateNextMatrix();
            }
        }

    EndFunc:
        retry = false;
		yield break;
	}

	IEnumerator WaitNext(){
		yield return new WaitForSeconds(1);
		isBusy = false;
	}

	void EndGame(){
		//Debug.Log(currentBoardMinHeight + " >= " + currentBoardMaxHeight + " || " + currentBoardMinWidth + " >= " + currentBoardMaxWidth);
        panelScore.SetActive(true);
        panelCurrentScore.SetActive(false);

        if (GameData.boards[boardLevel][boardID].played) {
            if (GameData.boards[boardLevel][boardID].userMoves > scoreCount)
                GameData.boards[boardLevel][boardID].userMoves = scoreCount;
        }
        else {
            GameData.boards[boardLevel][boardID].played = true;
            GameData.boards[boardLevel][boardID].userMoves = scoreCount;
        }
        
        textFinalScore.text = "Level finished\nin " + scoreCount + " Moves.";
	}
	#endregion

	#region GUI_UPDATE
	void UpdateGUI_MovesCount(){
        textCurrentScore.text = "" + scoreCount;
	}

	void UpdateGUI_BoardID(){
		textBoardID.text = boardMatrix;
	}
	#endregion

	#region BUTTONS_CALLBACKS
	public void OnButtonPressed(int id){
		switch(id){
		    case 0 : //SETTINGS

			    break;
		    case 1 : //RESET
			    this.LoadBoardFromID(boardMatrix);
			    break;
		    case 2 : //NEW BOARD
                retry = true;
			    this.GenerateNextMatrix();
			    break;
            case 3: //RETRY SAME BOARD
                retry = true;
                this.LoadBoardFromID(boardMatrix);
                break;
		    default: break;
		}
	}
	#endregion

	#region UNITY_CALLBACKS
	// Use this for initialization
	void Start () {
		CreateCardMatrix();
		
		AdjustCamera();

		if(isRandom){
			GenerateRandomMatrix();
            //			LoadBoardFromID("000000111111000000111111000000111111");
        }
        else {
            LoadBoardFromID(boardMatrix);
        }
	}
	
	// Update is called once per frame
	void Update () {
		#if UNITY_EDITOR
		if (Input.GetMouseButtonDown(0))
		{
			if(!isBusy)
				ProcessTouch(Input.mousePosition.x, Input.mousePosition.y);
			
		}
		#else
		//if (Input.touchCount > 0/* && Input.touches[0].phase == TouchPhase.Began*/)
		if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
		{
			if(!isBusy)
				ProcessTouch(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y);
		}
		#endif
	}

	void OnGUI(){
		if(Input.GetKeyDown(KeyCode.Escape)){
			Application.LoadLevel("01-MenuBoards");
		}
	}
	#endregion
}
