using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PopulateGUIScriptPreLoaded : MonoBehaviour {
    public GameObject content;
	public GameObject[] pages;

    public static int difficulty = 0;


    int nElementsPerPage = 30;
//    int nColumns = 5;
//    //int nRows;
//    int spacing = 5;
//    int paddingH = 15;
//    int paddingV = 15;

    // Use this for initialization
	void GeneratePage(int startNumber, int pageNumber)
    {
		string str =  "Level " + (difficulty == 0 ? "4x4" : (difficulty == 1 ? "6x6" : "8x8"));
		pages[pageNumber].transform.Find("Text").GetComponent<Text>().text = str;

		GameObject contentGameObject = pages[pageNumber].transform.Find("Panel").gameObject;
        UnityEngine.UI.GridLayoutGroup grid = contentGameObject.GetComponentInChildren<UnityEngine.UI.GridLayoutGroup>();
        

//        grid.constraint = UnityEngine.UI.GridLayoutGroup.Constraint.FixedColumnCount;
//        grid.constraintCount = nColumns;
//
//        grid.childAlignment = TextAnchor.MiddleCenter;
//
//        grid.spacing = new Vector2(spacing, spacing);
//        grid.padding = new RectOffset(paddingH, paddingH, paddingV, paddingV);
//
//        grid.cellSize = new Vector2(
//            (Screen.width - Screen.width * 0.3f - (nColumns - 1) * spacing - paddingH * 2) / nColumns,
//            //(Screen.height- Screen.height* 0.2f - (nRows-1) * spacing - paddingV * 2) / nRows
//            (Screen.width - Screen.width * 0.3f - (nColumns - 1) * spacing - paddingH * 2) / nColumns
//            );

        for (int i = 0; i < nElementsPerPage; ++i)
        {
//            GameObject go = Instantiate(levelPrefab) as GameObject;
			contentGameObject.transform.GetChild(i).GetComponent<LevelUIScript>().SetLevel(difficulty, startNumber + i);

//            go.transform.GetComponent<LevelUIScript>().SetLevel(difficulty, startNumber + i);

//            go.transform.SetParent(contentGameObject.transform);
        }
    }

    
    public void GeneratePages (List<Board> boards) {
        //int nPages = boards.Count / nElementsPerPage;
        int nPages = 150 / nElementsPerPage;

        Debug.Log("nPages: " + nPages);

        for (int i = 0; i < nPages; ++i)
        {
            GeneratePage(i * 30, i);
        }
	}

    // Use this for initialization
    void Start ()
    {
        //nRows = nElementsPerPage / nColumns;
        GeneratePages(GameData.boards[difficulty]);

    }

    void OnGUI () {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.LoadLevel(0);
        }
    }
}
