using UnityEngine;
using System.Collections;

public class SpaceCanGUI : MonoBehaviour {
    #region BUTTONS_CALLBACKS
    public void OnButtonPressed(int id) {
        switch (id) {
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
}
