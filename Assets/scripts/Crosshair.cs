using UnityEngine;
using System.Collections;

public class Crosshair : MonoBehaviour {

    public Texture2D crosshairTexture;
    static bool OriginalOn = true;

	
	// Update is called once per frame
	void OnGUI () {
        if (OriginalOn) {
            Rect position = new Rect((Screen.width - crosshairTexture.width) / 2, (Screen.height - crosshairTexture.height) / 2, crosshairTexture.width, crosshairTexture.height);
            GUI.DrawTexture(position, crosshairTexture);
        }
	}
}
