using UnityEngine;
using System.Collections;

public class Controller : MonoBehaviour {
	#region VARIABLES
	public static Controller instance;
    
	public GamePreferences preferences;
	#endregion

	#region UNITY_CALLBACKS
	void Awake () {
        // if the singleton hasn't been initialized yet
        if (instance != null) {
            Destroy(this.gameObject);
            return;//Avoid doing anything else
        }

        if(instance == this) {
            return;
        }

        instance = this;
        DontDestroyOnLoad(this.gameObject);

        GameData.Initialize();


	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	#endregion
}
