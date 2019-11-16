using System.Collections;
using System.Collections.Generic;
using Mirror;
using System;
using UnityEngine;
using UnityEngine.UI;

public class Health : NetworkBehaviour {
	[SyncVar] [NonSerialized] public int playerIndex;
	[SyncVar(hook = nameof(ChangeHpMaxFill))] public int healthMax;
	[SyncVar(hook = nameof(ChangeHpCurrFill))] public int healthCurr;

	[SerializeField] Image hpFill;

	public override void OnStartServer() {
		base.OnStartServer();
		healthCurr = healthMax;
	}

	void ChangeHpMaxFill(int newVal) {
		healthMax = newVal;
		if(hpFill)
			hpFill.fillAmount = (float)(healthCurr) / healthMax;
	}

	void ChangeHpCurrFill(int newVal) {
		healthCurr = newVal;
		if(hpFill)
			hpFill.fillAmount = (float)(healthCurr) / healthMax;

		 if (healthCurr <= 0) {
			Attacker attacker = GetComponent<Attacker>();
			if (attacker) {
				attacker.OnAttack();
			}
			else {
				NetworkServer.Destroy(gameObject);
			}
		}
	}

	public void GetGamage(Attacker attacker) {
		if (attacker.isCanDamage) {
			healthCurr -= attacker.damage;
			attacker.OnAttack();
		}
	}

	private void OnTriggerEnter(Collider other) {
		if (isServer && other.gameObject.tag == "Attacker") {
			Attacker attacker = other.gameObject.GetComponent<Attacker>();
			if(attacker.playerIndex != playerIndex)
				GetGamage(attacker);
		}
	}
}