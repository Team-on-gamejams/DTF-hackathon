using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Experimental.VFX;

public class Attacker : NetworkBehaviour {
	[SyncVar] public bool isDoDamage = true;
	public int damage = 10;

	[ClientRpc]
	public void RpcOnAttack() {
		isDoDamage = false;
		GetComponent<VisualEffect>().Stop();
		LeanTween.delayedCall(3.0f, () => NetworkServer.Destroy(gameObject));
	}
}
