using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : MonoBehaviour {
	Rigidbody rigidbody;

	/* Script options and their default values */
	[SerializeField] Transform camera;
	[SerializeField] float camera_sensitivity = 300.0f;
	[SerializeField] float head_rotation_limit = 90.0f;
	float head_rotation = 0f;

	[SerializeField] KeyCode jump = KeyCode.Space;
	[SerializeField] float jump_force = 4.0f;
	
	[SerializeField] Transform ground_checker;
	[SerializeField] float ground_check_radius = 0.25f;
	[SerializeField] LayerMask ground_layer;

	[SerializeField] float speed = 25.0f;
	[SerializeField] KeyCode sprint;
	[SerializeField] float sprint_multiplier = 1.5f;
	float effective_speed;

	/* Check if we're on the ground */
	bool on_ground()
	{
		Collider[] colliders = Physics.OverlapSphere(ground_checker.position,
							     ground_check_radius, ground_layer);

		if (colliders.Length > 0)
			return true;
		
		return false;
	}

	/* Called before the first frame update */
	void Start()
	{

		/* Get our rigidbody */
		rigidbody = GetComponent<Rigidbody>();

		/* Hide and lock our cursor */
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
	}

	/* Called once per frame */
	void Update()
	{
		float cam_x;
		float cam_y;

		float pos_x;
		float pos_z;

		/* Process camera movement */
		cam_x = Input.GetAxis("Mouse X") * camera_sensitivity * Time.deltaTime;
		cam_y = Input.GetAxis("Mouse Y") * camera_sensitivity * Time.deltaTime * -1.0f;

		/* Rotate the player */
		transform.Rotate(0f, cam_x, 0f);

		/* Rotate the camera */
		head_rotation += cam_y;
		head_rotation = Mathf.Clamp(head_rotation, -head_rotation_limit, head_rotation_limit);
		camera.localEulerAngles = new Vector3(head_rotation, 0f, 0f);

		/* Process movement */
		pos_x = Input.GetAxisRaw("Horizontal");
		pos_z = Input.GetAxisRaw("Vertical");

		/* Calculate our transform vector */
		Vector3 new_pos = transform.right * pos_x + transform.forward * pos_z;

		/* Calculate our effective speed */
		effective_speed = speed;
		if (Input.GetKeyDown(sprint)) {
			effective_speed *= sprint_multiplier;
			Console.Write("Sprinting\n");
		}

		/* Move our character */
		rigidbody.MovePosition(transform.position + new_pos.normalized * effective_speed * Time.deltaTime);

		/* Now handle jumping */
		if (Input.GetKeyDown(jump) && on_ground())
			rigidbody.AddForce(Vector3.up * jump_force, ForceMode.Impulse);
	}
}
