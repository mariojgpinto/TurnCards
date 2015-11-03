using UnityEngine;
using System.Collections;

public class Controller : MonoBehaviour {
	#region VARIABLES
	public static Controller instance;

	public GamePreferences preferences;
	#endregion

	#region UNITY_CALLBACKS
	void Awake () {
		instance = this;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	#endregion
}
