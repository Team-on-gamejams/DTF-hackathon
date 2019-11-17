using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Experimental.VFX;

public class Fireball : Attacker {
	[SyncVar] float currHoldTime = 0;

	[SerializeField] float holdTimeMax = 10;
	[SerializeField] float sizeMin = 0.1f;
	[SerializeField] float sizeMax = 2;
	[SerializeField] float spawnRateMin = 150;
	[SerializeField] float spawnRateMax = 3000;
	[SerializeField] float speedMin = 20;
	[SerializeField] float speedMax = 5;
	[SerializeField] float moveDistanceMin = 20;
	[SerializeField] float moveDistanceMax = 5;
	[SerializeField] float damageMin = 5;
	[SerializeField] float damageMax = 50;

	[SerializeField] float moveToPosAccurasity = 0.25f;
	Vector3 moveToPos;

	[SerializeField] SphereCollider collider;

	void Awake() {
		moveToPosAccurasity *= moveToPosAccurasity;
	}

	void Update() {
		if (!isServer)
			return;

		if(currHoldTime < holdTimeMax) {
			currHoldTime += Time.deltaTime;
			if (currHoldTime > holdTimeMax)
				currHoldTime = holdTimeMax;
			RpcOnUpdate();
		}
	}

	[ClientRpc]
	protected void RpcOnUpdate() {
		visualEffect.SetInt("SpawnRate", Mathf.RoundToInt(Mathf.Lerp(spawnRateMin, spawnRateMax, currHoldTime / holdTimeMax)));
		float radius = Mathf.Lerp(sizeMin, sizeMax, currHoldTime / holdTimeMax);
		visualEffect.SetFloat("SphereRadius", radius);
		collider.radius = radius;
		damage = Mathf.RoundToInt(Mathf.Lerp(damageMin, damageMax, currHoldTime / holdTimeMax));
	}

	public override void OnReleaseMouse(Vector3 pos) {
		moveToPos = (pos - transform.position).normalized * Mathf.Lerp(moveDistanceMin, moveDistanceMax, currHoldTime / holdTimeMax) + transform.position;
		StartCoroutine(MoveCoroutine());
	}

	IEnumerator MoveCoroutine() {
		transform.rotation = Quaternion.LookRotation(moveToPos - transform.position);
		float currSpeed = Mathf.Lerp(speedMin, speedMax, currHoldTime / holdTimeMax);

		while ((transform.position - moveToPos).sqrMagnitude > moveToPosAccurasity) {
			transform.Translate(Vector3.forward * currSpeed * Time.deltaTime, Space.Self);
			yield return null;
		}

		RpcOnAttack();
	}
}
