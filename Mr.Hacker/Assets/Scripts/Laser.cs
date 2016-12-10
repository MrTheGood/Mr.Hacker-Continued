using UnityEngine;
using System.Collections;

public class Laser : MonoBehaviour {
	public float speed;			//Movement speed of the laser.
	public GameObject parent;	//The robot that fired the laser.
	public int damage;			//The damage the laser gives.

	void Update () {
		//Move the laser.
		transform.Translate(Vector2.right * speed * Time.deltaTime);
	}

	void OnTriggerEnter(Collider col) {
		//If the player hit it's parent,
		if ((col.gameObject.tag == "Robot" || col.gameObject.tag == "RobotBack") && (col.gameObject == parent || col.transform.parent.gameObject == parent)) 
			//Stop right there.
			return;

		//Destroy the laser.
		Destroy(gameObject);
	}
	void OnCollisionEnter(Collision col) {
		//If the player hit it's parent,
		if ((col.gameObject.tag == "Robot" || col.gameObject.tag == "RobotBack") && col.gameObject == parent) 
			//Stop right there.
			return;

		//Destroy the laser.
		Destroy(gameObject);
	}
}
