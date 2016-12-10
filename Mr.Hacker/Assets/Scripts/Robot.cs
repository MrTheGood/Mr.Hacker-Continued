using UnityEngine;
using System.Collections;


// @Author: Grafisch Lyceum Rotterdam, Maarten de Goede
public class Robot : MonoBehaviour {
	public int health = 5;						//Robot health.
	public int points = 20;						//Points after killing it.
	public int damage = 1;						//Damage the robot gives to whatever it hit.
	public float speed = 3;						//Robot walk speed.
	public bool hackable = true;				//If the robot is hackable or not
	public Vector2[] path;						//The path the robot should use
	public GameObject laser;					
	//Box Colliders for the back of the robot
	public BoxCollider backColliderSide;
	public BoxCollider backColliderUp;
	public BoxCollider backColliderDown;

	private int targetWaypoint;					//The next point of the path
	private bool isHacked = false;				//If the robot is hacked
	private bool isShooting = false;			//If the robot is shooting
	private Animator animator;
	private Sprite startSprite;					//The starting sprite


	void Start () {
		targetWaypoint = 0;						//The start target waypoint is the first waypoint.
		animator = GetComponent<Animator>();
		startSprite = GetComponent<SpriteRenderer>().sprite;
	}
	void Update () {
		//If the robot's health is depleted,
		if (health <= 0) {
			//If the robot is hacked,
			if (isHacked) {
				//Because there are more players in the hierachy, let each player stop hacking. This will also destroy the robot and add score points.
				foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player")) {
					if (player.activeInHierarchy)
						player.GetComponent<Player>().stopHacking(this);
				}
				return;
			}

			//Destroy this ugly metal beast.
			Destroy(gameObject);
			BoardManager.score += points;
		}

		//If the player isn't hacked,
		if (!isHacked) {
			//If the player isn't shooting,
			if (!isShooting) {
				//Move the robot.
				walkRobot(speed * Time.deltaTime);
				//Look for the player.
				lookForPlayer();
			}
			//Set the walk animation.
			setWalkAnimation();
		} else {
			//The robot is hacked. Is it at least not shooting?
			if (!isShooting) {
				//If the player presses X,
				if (Input.GetKeyDown(KeyCode.X))
					//Kill this ugly metal beast.
					health = 0;
				//If the player presses Space,
				if (Input.GetKeyDown(KeyCode.Space)) {
					//Fire a laser.
					StartCoroutine(fireHackedLaser());
				}
				//Walk this hacked ugly metal monster.
				walkHackedRobot();
			}
		}
	}
	void OnTriggerEnter(Collider col) {
		//If this.. thing.. hit a laser,
		if (col.gameObject.tag == "Laser") {
			//If the laser was fired by this metal monster itself,
			if (col.gameObject.GetComponent<Laser>().parent == gameObject)
				//No need to do anything.
				return;

			//Remove damagepoints from the robot.
			health -= col.gameObject.GetComponent<Laser>().damage;
		}
	}

	public void setHacked(bool hacked) {
		//If the robot isn't hackable, stop.
		if (!hackable)
			return;

		//Stop shooting.
		isShooting = false;
		//Set isHacked.
		isHacked = hacked;
		animator.SetBool("isHacked", hacked);
	}


	private void walkHackedRobot() {
		//Calculate the input/walking speed
		float x = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
		float y = Input.GetAxis("Vertical") * speed * Time.deltaTime;

		//Reset the animator and rotation
		transform.rotation = new Quaternion(0, 0, 0, 0);
		animator.enabled = true;

		//Move the robot
		GetComponent<CharacterController>().Move(new Vector3(x, y, 0));

		//Test the direction and set the animation.
		if (x > 0) {
			//Right
			animator.SetInteger("WalkState", 2);
		} else if (x < 0) {
			//Left
			animator.SetInteger("WalkState", 2);
			//Flip the robot horizontally 
			transform.rotation = new Quaternion(0, 180, 0, 0);
		} else if (y > 0) {
			//Up
			animator.SetInteger("WalkState", 1);
		} else if (y < 0) {
			//Down
			animator.SetInteger("WalkState", 0);
		} else if (!isShooting) {
			//The robot isn't walking. Stop the animation and set a standing sprite.
			animator.enabled = false;
			GetComponent<SpriteRenderer>().sprite = startSprite;
		}
	}

	/// <summary>
	/// Calculates the walking direction.
	/// </summary>
	/// <returns>The walking direction.</returns>
	private Vector2 getWalkingDirection() {
		Vector2 targetLocation = path[targetWaypoint];

		if (transform.position.x > targetLocation.x) {
			return Vector2.right;
		}
		if (transform.position.x < targetLocation.x) {
			return Vector2.left;
		}
		if (transform.position.y > targetLocation.y) {
			return Vector2.up;
		}
		return Vector2.down;
	}


	private IEnumerator fireHackedLaser() {
		float x = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
		float y = Input.GetAxis("Vertical") * speed * Time.deltaTime;

		//Start the animation
		isShooting = true;
		animator.SetTrigger("Fire");
		yield return new WaitForSeconds(0.5f);

		//Instantiate the laser
		GameObject instance = (GameObject)Instantiate(laser, transform.position, new Quaternion());

		//Send the laser to the correct direction
		if (x < 0) {
			instance.transform.position -= new Vector3(0.2f, 0.1f, 0);
			instance.transform.rotation = new Quaternion(0, 0, 180, 0);
		}
		else if (y < 0) {
			instance.transform.position -= new Vector3(0.2f, 0, 0);
			instance.transform.rotation = new Quaternion(0, 0, 90, -90);
		}
		else if (y > 0) {
			instance.transform.position -= new Vector3(-0.2f, 0, 0);
			instance.transform.rotation = new Quaternion(0, 0, 90, 90);
		}
		else  {
			instance.transform.position -= new Vector3(0, 0.1f, 0);
			instance.transform.rotation = new Quaternion();
		}

		//Set the laser damage
		instance.GetComponent<Laser>().damage = damage;
		instance.GetComponent<Laser>().parent = gameObject;

		//Wait 0.6 seconds
		yield return new WaitForSeconds(0.6f);
		//Stop shooting
		isShooting = false;
	}


	private IEnumerator fireLaser(Vector2 direction) {
		//Start the animation
		isShooting = true;
		animator.SetTrigger("Fire");
		yield return new WaitForSeconds(0.5f);

		//Instantiate the laser
		GameObject instance = (GameObject)Instantiate(laser, transform.position, new Quaternion());;

		//Send the laser to the correct direction
		if (getWalkingDirection() == Vector2.left) {
			instance.transform.position -= new Vector3(0, 0.1f, 0);
			instance.transform.rotation = new Quaternion();
		}

		if (getWalkingDirection() == Vector2.right) {
			instance.transform.position -= new Vector3(0.2f, 0.1f, 0);
			instance.transform.rotation = new Quaternion(0, 0, 180, 0);
		}

		if (getWalkingDirection() == Vector2.down) {
			instance.transform.position -= new Vector3(-0.2f, 0, 0);
			instance.transform.rotation = new Quaternion(0, 0, 90, 90);
		}
		
		if (getWalkingDirection() == Vector2.up) {
			instance.transform.position -= new Vector3(0.2f, 0, 0);
			instance.transform.rotation = new Quaternion(0, 0, 90, -90);
		}

		//Set the laser damage
		instance.GetComponent<Laser>().damage = damage;
		instance.GetComponent<Laser>().parent = gameObject;

		//Wait 1.2 seconds
		yield return new WaitForSeconds(1.2f);
		//Stop shooting
		isShooting = false;
	}


	private void walkRobot(float speed) {
		//Get the location where the robot should walk to.
		Vector2 targetLocation = path[targetWaypoint];

		//If the robot hasn't yet arrived at the target location,
		if ((Vector2)transform.position != targetLocation) {
			//Move towards the target location.
			transform.position = Vector2.MoveTowards(transform.position, targetLocation, speed);
			return;
		}

		//The robot has arrived. Let's go to the next waypoint.
		targetWaypoint++;
		//If the next waypoint doesn't exist,
		if (targetWaypoint >= path.Length)
			//Go to the first waypoint.
			targetWaypoint = 0;
	}


	private void lookForPlayer() {
		RaycastHit hit;

		//If something is in front of the robot,
		if (Physics.Raycast(transform.position, -getWalkingDirection(), out hit)) {
			//If the object in front of the robot is the player,
			if (hit.collider.gameObject.tag == "Player") {
				//Fire a laser.
				StartCoroutine(fireLaser(getWalkingDirection()));
			}
			//If the object in front of the robot is a robot,
			if (hit.collider.gameObject.tag == "Robot") {
				//If the robot in front of this robot is hacked,
				if (hit.collider.gameObject.GetComponent<Robot>().isHacked) {
					//Fire a laser.
					StartCoroutine(fireLaser(getWalkingDirection()));
				}
			}
		}
	}


	private void setWalkAnimation() {
		Vector2 targetLocation = path[targetWaypoint];
		
		//Reset the animator and rotation
		transform.rotation = new Quaternion(0, 0, 0, 0);
		animator.enabled = true;

		//Test the direction and set the animation.
		if (targetLocation.x > transform.position.x) {
			//Right
			animator.SetInteger("WalkState", 2);
			if (hackable) {
				//Set the right back collider
				backColliderSide.enabled = true;
				backColliderUp.enabled = false;
				backColliderDown.enabled = false;
			}
		} else if (targetLocation.x < transform.position.x) {
			//Left
			animator.SetInteger("WalkState", 2);
			//Flip the robot horizontally 
			transform.rotation = new Quaternion(0, 180, 0, 0);
			if (hackable) {
					//Set the right back collider
					backColliderSide.enabled = true;
					backColliderUp.enabled = false;
					backColliderDown.enabled = false;
			}
		} else if (targetLocation.y > transform.position.y) {
			//Up
			animator.SetInteger("WalkState", 1);
			if (hackable) {
				//Set the right back collider
				backColliderSide.enabled = false;
				backColliderUp.enabled = false;
				backColliderDown.enabled = true;
			}
		} else if (targetLocation.y < transform.position.y) {
			//Down
			animator.SetInteger("WalkState", 0);
			if (hackable) {
				//Set the right back collider
				backColliderSide.enabled = false;
				backColliderUp.enabled = true;
				backColliderDown.enabled = false;
			}
		} else if (isShooting) {
			//The Robot is shooting.
			if (hackable) {
				backColliderSide.enabled = true;
				backColliderUp.enabled = false;
				backColliderDown.enabled = false;
			}
		} else {
			//The Robot isn't walking or shooting. Stop the animation and set a standing sprite.
			animator.enabled = false;
			if (hackable) {
				backColliderSide.enabled = true;
				backColliderUp.enabled = false;
				backColliderDown.enabled = false;
			}
			GetComponent<SpriteRenderer>().sprite = startSprite;
		}
	}
}
