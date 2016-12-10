using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BoardManager : MonoBehaviour {
	public static BoardManager boardManager;	//Instance of the BoardManager
	public static int score = 0;				//Player Score
	public GameObject[] levels;					//All the levels
	public GameObject gameOverLevel;			//Game Over level
	public GameObject finishLevel;				//Finish level

	private int currentLevel = 0;				//Current level
	private GameObject level;					//Also current level


	void Awake () {
		//If there is no boardmanager yet,
		if (boardManager == null)
			//This is the boardmanager.
			boardManager = this;
		//If this isn't the boardmanager,
		else if (boardManager != this)
			//Destroy this.
			Destroy(this);

		//For each level,
		foreach (GameObject lvl in levels)
			//Deactivate.
			lvl.SetActive(false);

		//Reset the score.
		score = 0;
		//Deactivate the finish level and the game over level.
		gameOverLevel.SetActive(false);
		finishLevel.SetActive(false);
		//Load the first level.
		loadLevel();
	}

	/// <summary>
	/// Loads the next level depending on the current level.
	/// </summary>
	public void loadLevel() {
		//If the current level isn't null,
		if (level != null)
			//Deactivate it.
			level.SetActive(false);

		//If the current level is the last level,
		if (currentLevel >= levels.Length) {
			//Activate the finish screen.
			finishLevel.SetActive(true);
			//Load the main menu after 2 seconds.
			StartCoroutine(loadMainMenu(2f));
			//Don't activate the next level.
			return;
		}

		//Activate the next level.
		level = levels[currentLevel];
		currentLevel++;
		level.SetActive(true);
	}

	/// <summary>
	/// This is called once the player deid.
	/// </summary>
	public void gameOver() {
		//If the current level isn't null,
		if (level != null)
			//Deactivate it.
			level.SetActive(false);

		//Activate the game over screen.
		gameOverLevel.SetActive(true);
		//Load the main menu after 2 seconds.
		StartCoroutine(loadMainMenu(2f));
	}

	/// <summary>
	/// Loads the main menu.
	/// </summary>
	/// <param name="waitTime">How long it should wait before loading the main menu.</param>
	private IEnumerator loadMainMenu(float waitTime) {
		yield return new WaitForSeconds(waitTime);
		SceneManager.LoadScene("mainMenu");
	}

	/// <summary>
	/// Loads the next level after 1.5 seconds.
	/// </summary>
	public IEnumerator loadNextLevel() {
		yield return new WaitForSeconds(1.5f);
		loadLevel();
	}

	void OnGUI() {
		//Get the player health.
		int health = Player.health;
		//Create the score text.
		string scoreText = "";
		if (score < 100) {
			scoreText = "0";
		} 
		if (score < 10) {
			scoreText += "0";
		}
		scoreText += score + " ";

		//Show the score text and the health text.
		GameObject.Find("scoreText").GetComponent<Text>().text = "Score: " + scoreText;
		GameObject.Find("lifeText").GetComponent<Text>().text = " Lives: " + health;
	}
}
