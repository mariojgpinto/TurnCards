using UnityEngine;

public class LevelUIScript : MonoBehaviour {
    public int levelDifficulty;
    public int levelNumber;

    public void SetLevel(int difficulty, int number)
    {
        levelDifficulty = difficulty;
        levelNumber = number;
        this.GetComponentInChildren<UnityEngine.UI.Text>().text = "" + (levelNumber + 1);
    }

    public void OnButtonPressed()
    {
        Debug.Log("Button(" + levelDifficulty + "," + levelNumber + ")");
    }

	//// Use this for initialization
	//void Start () {
	    
	//}
}
