using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Experimental.VFX;

public class Attacker : NetworkBehaviour {
	[SyncVar] public bool isCanDamage = true;
	[SyncVar] [NonSerialized] public int playerIndex;
	public int damage = 10;
	public int speed = 10;
	public float moveToPosAccurasity = 0.25f;
	public float maxMoveDistance = 20;
	Vector3 moveToPos;

	private void Awake() {
		moveToPosAccurasity *= moveToPosAccurasity;
	}

	public virtual void OnReleaseMouse(Vector3 pos) {
		moveToPos = (pos - transform.position).normalized * maxMoveDistance + transform.position;
		StartCoroutine(MoveCoroutine());
	}

	[Server]
	public void OnCollideWithHealth() {
		isCanDamage = false;
		RpcOnAttack();
	}

	[ClientRpc]
	void RpcOnAttack() {
		StopAllCoroutines();
		GetComponent<VisualEffect>().Stop();
		foreach (var collider in GetComponents<Collider>())
			collider.enabled = false;
		LeanTween.delayedCall(gameObject, 3.0f, () => NetworkServer.Destroy(gameObject));
	}

	public override void OnNetworkDestroy() {
		base.OnNetworkDestroy();
		LeanTween.cancel(gameObject, false);
		StopAllCoroutines();
	}

	IEnumerator MoveCoroutine() {
		transform.rotation = Quaternion.LookRotation(moveToPos - transform.position);

		while ((transform.position - moveToPos).sqrMagnitude > moveToPosAccurasity) {
			transform.Translate(Vector3.forward * speed * Time.deltaTime, Space.Self);
			yield return null;
		}

		RpcOnAttack();
	}
}
