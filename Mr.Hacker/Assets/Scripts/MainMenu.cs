using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {
	void Update () {
		//If the player presses the left mouse button,
		if (Input.GetMouseButtonDown(0)) {
			//Create a raycast to check what the player is trying to click on.
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit raycastHit;

			if (Physics.Raycast(ray, out raycastHit)) {
				//Load the right level if the player hit the right things. Pretty simple.
				if (raycastHit.transform.name == "Play")
					SceneManager.LoadScene("gameLevel");
				if (raycastHit.transform.name == "Controls")
					SceneManager.LoadScene("controls");
				if (raycastHit.transform.name == "Back")
					SceneManager.LoadScene("mainMenu");
			}
		}
	
	}
}
