using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class base_living : MonoBehaviour
{
	Rigidbody rigidbody;

	/* Script options and their default values */
	[SerializeField] public float max_health = 100.0f;
	[SerializeField] public float health;

	[SerializeField] Transform camera;
	[SerializeField] float camera_sensitivity = 300.0f;
	[SerializeField] public float head_rotation_limit = 90.0f;
	float head_rotation = 0f;

	[SerializeField] public bool can_jump = true;
	[SerializeField] KeyCode jump = KeyCode.Space;
	[SerializeField] public float jump_force = 4.0f;

	[SerializeField] Transform ground_checker;
	[SerializeField] public float ground_check_radius = 0.25f;
	[SerializeField] LayerMask ground_layer;

	[SerializeField] public float speed = 10.0f;

	[SerializeField] public bool can_sprint = true;
	[SerializeField] KeyCode sprint = KeyCode.LeftShift;
	[SerializeField] public float sprint_multiplier = 1.5f;
	
	float effective_speed;

	[SerializeField] KeyCode crouch = KeyCode.LeftControl;
	[SerializeField] public float crouch_height_modifier = -1.0f;
	float height;

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
	public void base_start()
	{
		/* Set our health */
		health = max_health;

		/* Get our rigidbody */
		rigidbody = GetComponent<Rigidbody>();

		/* Determine our height */
		height = transform.localScale.y;

		/* Hide and lock our cursor */
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
	}

	/* Called once per frame */
	public void base_update()
	{
		float cam_x;
		float cam_y;

		float pos_x;
		float pos_z;

		/* Reset our capabilities */
		bool local_can_jump = can_jump;
		bool local_can_sprint = can_sprint;
		float local_height = height;

		/* Process camera movement */
		cam_x = Input.GetAxis("Mouse X") * camera_sensitivity * Time.deltaTime;
		cam_y = Input.GetAxis("Mouse Y") * camera_sensitivity * Time.deltaTime * -1.0f;

		/* Rotate the player */
		transform.Rotate(0f, cam_x, 0f);

		/* Rotate the camera */
		head_rotation += cam_y;
		head_rotation = Mathf.Clamp(head_rotation, -head_rotation_limit, head_rotation_limit);
		camera.localEulerAngles = new Vector3(head_rotation, 0f, 0f);

		/* Process crouching */
		if (Input.GetKey(crouch)) {
			local_height += crouch_height_modifier;
			local_can_jump = false;
			local_can_sprint = false;
		}

		/* Process movement */
		pos_x = Input.GetAxisRaw("Horizontal");
		pos_z = Input.GetAxisRaw("Vertical");

		/* Calculate our transform vector */
		Vector3 new_pos = transform.right * pos_x + transform.forward * pos_z;

		/* Calculate our effective speed */
		effective_speed = speed;
		if (Input.GetKey(sprint) && local_can_sprint)
			effective_speed *= sprint_multiplier;

		/* Move our character */
		rigidbody.MovePosition(transform.position + new_pos.normalized * effective_speed * Time.deltaTime);

		/* Now handle jumping */
		if (Input.GetKeyDown(jump) && on_ground() && local_can_jump)
			rigidbody.AddForce(Vector3.up * jump_force, ForceMode.Impulse);

		transform.localScale = new Vector3(transform.localScale.x, local_height, transform.localScale.z);
	}
}
