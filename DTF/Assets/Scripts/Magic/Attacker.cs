using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Experimental.VFX;

public class Attacker : NetworkBehaviour {
	[SyncVar] public bool isCanDamage = true;
	[SyncVar] [NonSerialized] public int playerIndex = -1;
	public int damage = 10;
	[SerializeField] protected float delayBeforeDestroy = 1;

	[SerializeField] protected VisualEffect visualEffect;

	public virtual void OnReleaseMouse(Vector3 pos) { }
	[Client]
	public virtual void BeforeStopEffect() { }

	[Server]
	public void OnCollideWithHealth() {
		isCanDamage = false;
		RpcOnAttack();
	}

	[ClientRpc]
	protected void RpcOnAttack() {
		BeforeStopEffect();
		LeanTween.cancel(gameObject, false);
		StopAllCoroutines();
		visualEffect.Stop();

		foreach (var collider in GetComponents<Collider>())
			collider.enabled = false;

		LeanTween.delayedCall(gameObject, delayBeforeDestroy, () => NetworkServer.Destroy(gameObject));
	}

	public override void OnNetworkDestroy() {
		base.OnNetworkDestroy();
		LeanTween.cancel(gameObject, false);
		StopAllCoroutines();
	}
}
