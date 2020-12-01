using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : base_living {
	void Start()
	{
		base_start();
	}

	void Update()
	{
		base_update();

		/* Not all things will necessarily die, but in this case we just want to respawn */
		if (health <= 0) {
			health = max_health;

			/* TODO: Once we have the server side stuff, the player should be returned to the spawn point */
		}
	}
}
