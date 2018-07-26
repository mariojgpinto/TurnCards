using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.UI.Extensions;
using System.Collections;

public class PopulateGUIScript : MonoBehaviour {
    public GameObject panelUIPrefab;
    public GameObject levelPrefab;

    public GameObject content;

    public static int difficulty = 0;


    int nElementsPerPage = 30;
    int nColumns = 5;
    //int nRows;
    int spacing = 5;
    int paddingH = 15;
    int paddingV = 15;

    // Use this for initialization
    void GeneratePage(int startNumber)
    {
        GameObject page = Instantiate(panelUIPrefab) as GameObject;
        page.transform.SetParent(content.transform);

        page.GetComponentInChildren<Text>().text = "Level " + (difficulty == 0 ? "4x4" : (difficulty == 1 ? "6x6" : "8x8"));

        GameObject contentGameObject = page.transform.Find("Panel").gameObject;
        UnityEngine.UI.GridLayoutGroup grid = contentGameObject.GetComponentInChildren<UnityEngine.UI.GridLayoutGroup>();
        

        grid.constraint = UnityEngine.UI.GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = nColumns;

        grid.childAlignment = TextAnchor.MiddleCenter;

        grid.spacing = new Vector2(spacing, spacing);
        grid.padding = new RectOffset(paddingH, paddingH, paddingV, paddingV);

        grid.cellSize = new Vector2(
            (Screen.width - Screen.width * 0.3f - (nColumns - 1) * spacing - paddingH * 2) / nColumns,
            //(Screen.height- Screen.height* 0.2f - (nRows-1) * spacing - paddingV * 2) / nRows
            (Screen.width - Screen.width * 0.3f - (nColumns - 1) * spacing - paddingH * 2) / nColumns
            );

        for (int i = 0; i < nElementsPerPage; ++i)
        {
            GameObject go = Instantiate(levelPrefab) as GameObject;

            go.transform.GetComponent<LevelUIScript>().SetLevel(difficulty, startNumber + i);

            go.transform.SetParent(contentGameObject.transform);
        }
    }

    
    public void GeneratePages (List<Board> boards) {
        //int nPages = boards.Count / nElementsPerPage;
        int nPages = 150 / nElementsPerPage;

        Debug.Log("nPages: " + nPages);

        for (int i = 0; i < nPages; ++i)
        {
            GeneratePage(i * 30);
        }
	}

    // Use this for initialization
	IEnumerator Start ()
    {
        //nRows = nElementsPerPage / nColumns;
        GeneratePages(GameData.boards[difficulty]);

		yield return new WaitForEndOfFrame();

		float page = Mathf.FloorToInt((TurnSquaresGame.board_number + 1) / (float)nElementsPerPage);
		Debug.Log("Page: " + page);

		for(int i = 0 ; i < page ; ++i) {
			GameObject.Find("Horizontal Scroll Snap").GetComponent<HorizontalScrollSnap>().NextScreen();
			yield return new WaitForEndOfFrame();
		}

    }

    void OnGUI () {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.LoadLevel(0);
        }
    }
}
