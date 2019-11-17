using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Experimental.VFX;

public class Shield : Attacker {
	void Update() {
		if (!isClient)
			return;

		visualEffect.SetVector3("SphereCentre", transform.position);

	}
}
