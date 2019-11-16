using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class Health : NetworkBehaviour {
	[SyncVar(hook = nameof(ChangeHpMaxFill))] public int healthMax;
	[SyncVar(hook = nameof(ChangeHpCurrFill))] public int healthCurr;

	[SerializeField] Image hpFill;

	public override void OnStartServer() {
		base.OnStartServer();
		healthCurr = healthMax;
	}

	void ChangeHpMaxFill(int newVal) {
		hpFill.fillAmount = (float)(healthCurr) / (healthMax = newVal);
	}

	void ChangeHpCurrFill(int newVal) {
		hpFill.fillAmount = (float)((healthCurr = newVal)) / healthMax;
	}

	public void GetGamage(Attacker attacker) {
		if (attacker.isDoDamage) {
			healthCurr -= attacker.damage;
			attacker.RpcOnAttack();
		}
	}

	private void OnTriggerEnter(Collider other) {
		if (isServer && other.gameObject.tag == "Attacker") {
			Attacker attacker = other.gameObject.GetComponent<Attacker>();
			GetGamage(attacker);
		}
	}
}