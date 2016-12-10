using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

// @Author: Grafisch Lyceum Rotterdam, Maarten de Goede
public class Player : MonoBehaviour {
	public static int health = 3;	//Player health.
	public float speed = 4;			//Walking speed.

	private CharacterController characterController;
	private Animator animator;
	private Sprite startSprite;
	private bool isHacking;
	private bool arrivedAtFinish;



	void Start () {
		//Reset the health.
		health = 3;
		characterController = GetComponent<CharacterController>();
		animator = GetComponent<Animator>();
		startSprite = GetComponent<SpriteRenderer>().sprite;
	}
	void Update () {
		//If the player has arrived at the finish,
		if (arrivedAtFinish) {
			//If the player has not yet entered the finish elevator,
			if (transform.position.y < GameObject.FindGameObjectWithTag("Finish").transform.position.y) {
				//Walk toward the finish elevator.
				transform.Translate(Vector3.up * Time.deltaTime * (speed / 3));
				transform.localScale -= new Vector3(1 * Time.deltaTime, 1 * Time.deltaTime, 0);
				return;
			}
			//Hide the player and start loading the next level.
			GetComponent<SpriteRenderer>().enabled = false;
			StartCoroutine(BoardManager.boardManager.loadNextLevel());
			return;
		}
		if (!isHacking)
			//Move the player.
			walkPlayer();
	}
	void OnTriggerStay(Collider col) {
		//If the player is touching the finish and is trying to enter it,
		if (col.gameObject.tag == "Finish" && Input.GetAxisRaw("Vertical") == 1) {
			//Open the elevator.
			col.gameObject.GetComponent<Animator>().SetTrigger("OpenLift");
			//Set all the values so that the player will enter the elevator. Don't feel like explaining it more.
			arrivedAtFinish = true;
			animator.enabled = true;
			animator.SetInteger("WalkState", 2);
			characterController.enabled = false;
			//Set the x position to the elevator x position.
			transform.position = new Vector3(col.gameObject.transform.position.x, transform.position.y, 0);
		}
	}
	void OnTriggerEnter(Collider col) {
		//If the player hit a laser,
		if (col.gameObject.tag == "Laser") {
			//Take some damage.
			hurtPlayer(col.gameObject.GetComponent<Laser>().damage);
		}
		//If the player hit the back of a robot,
		if (col.gameObject.tag == "RobotBack") {
			//If the robot isn't hacking,
			if (!isHacking) {
				//Hack.
				startHacking(col.transform.parent.gameObject.GetComponent<Robot>());
			}
		}
		//If the player hit a robot,
		if (col.gameObject.tag == "Robot") {
			//Take some damage.
			hurtPlayer(col.gameObject.GetComponent<Robot>().damage);
		}
	}

	public void startHacking(Robot hackedBot) {
		//Set the robot to hacked.
		hackedBot.setHacked(true);
		//Get some points.
		BoardManager.score += hackedBot.points;
		//Hack!
		isHacking = true;
		animator.enabled = false;
		GetComponent<SpriteRenderer>().sprite = startSprite;
	}
	public void stopHacking(Robot hackedBot) {
		//Start to stop hacking.
		StartCoroutine(stopHackRoutine(hackedBot));
	}
	private IEnumerator stopHackRoutine(Robot hackedBot) {
		//Why did I do this? I don't remember. It looks very weird.. Why start a coroutine when you are immediatly going to stop it?
		StopCoroutine(stopHackRoutine(hackedBot));
		//Destroy the hacked bot.
		Destroy(hackedBot.gameObject);
		//Get some points.
		BoardManager.score += hackedBot.points;
		//Wait for a second.
		yield return new WaitForSeconds(1f);
		//Set the player to stop hacking.
		isHacking = false;
		animator.enabled = true;
	}


	public void hurtPlayer(int damage) {
		//Take some damage.
		health -= damage;

		//If the player is dead,
		if (health < 0) {
			//The player is dead.
			BoardManager.boardManager.gameOver();
		}
	}

	/// <summary>
	/// Moves the player to the location based on the input, and plays the correct animation.
	/// </summary>
	private void walkPlayer() {
		//Calculate the input/walking speed
		float x = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
		float y = Input.GetAxis("Vertical") * speed * Time.deltaTime;

		//Reset the animator and rotation
		transform.rotation = new Quaternion(0, 0, 0, 0);
		animator.enabled = true;
		//Test the direction and set the animation.
		if (x > 0) {
			//Right
			animator.SetInteger("WalkState", 1);
		} else if (x < 0) {
			//Left
			animator.SetInteger("WalkState", 1);
			//Flip the player horizontally 
			transform.rotation = new Quaternion(0, 180, 0, 0);
		} else if (y > 0) {
			//Up
			animator.SetInteger("WalkState", 2);
		} else if (y < 0) {
			//Down
			animator.SetInteger("WalkState", 0);
		} else {
			//The player isn't walking. Stop the animation and set a standing sprite.
			animator.enabled = false;
			GetComponent<SpriteRenderer>().sprite = startSprite;
		}

		//Move the player
		characterController.Move(new Vector3(x, y, 0));
	}
}
