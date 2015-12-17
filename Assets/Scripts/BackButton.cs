using UnityEngine;
using System.Collections;

public class BackButton : MonoBehaviour {
	public void OnButtonPressed(){
		//Debug.Log("Level" + Application.loadedLevel);

		if(Application.loadedLevel == 1){
			Application.LoadLevel("00-Menu");
		} else
		if(Application.loadedLevel == 2){
			Application.LoadLevel("01-MenuBoardsRuntime");
		} else{
			Application.LoadLevel("00-Menu");
		}
	}

	void Start(){
//		RectTransform r = this.GetComponent<RectTransform>();

		float value = Screen.width*.11f;

//		this.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(value/2f, -value/2f, 0);
		this.GetComponent<RectTransform>().anchoredPosition = new Vector2(value/1.5f, -value/1.5f);
		this.GetComponent<RectTransform>().sizeDelta = new Vector2(value, value);

		#if UNITY_ANDROID
		if(Application.loadedLevelName.Contains("Idle"))
			Destroy(this.gameObject);
		#endif

	}
}
