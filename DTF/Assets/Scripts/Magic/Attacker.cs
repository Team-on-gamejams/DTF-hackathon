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

	[SerializeField] protected VisualEffect visualEffect;

	public virtual void OnReleaseMouse(Vector3 pos) { }

	[Server]
	public void OnCollideWithHealth() {
		isCanDamage = false;
		RpcOnAttack();
	}

	[ClientRpc]
	protected void RpcOnAttack() {
		StopAllCoroutines();
		visualEffect.Stop();
		foreach (var collider in GetComponents<Collider>())
			collider.enabled = false;
		LeanTween.delayedCall(gameObject, 3.0f, () => NetworkServer.Destroy(gameObject));
	}

	public override void OnNetworkDestroy() {
		base.OnNetworkDestroy();
		LeanTween.cancel(gameObject, false);
		StopAllCoroutines();
	}
}
