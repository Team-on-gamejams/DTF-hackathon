using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Experimental.VFX;

public class Shield : Attacker {
	public override void BeforeStopEffect() {
		visualEffect.playRate = 15.0f; // 15.0f is max liferime random
		visualEffect.SetFloat("AttractionSpeed", 0);
	}

	public override void OnReleaseMouse(Vector3 pos) {
		RpcOnAttack();
	}

	void Update() {
		if (!isClient)
			return;

		visualEffect.SetVector3("SphereCentre", transform.position);
	}
}
