using System;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : NetworkBehaviour {
	[SyncVar] [NonSerialized] public int index = -1;

	[Header("Movement Settings")]
	[SerializeField] float moveSpeed = 8f;
	float horizontal = 0f;
	float vertical = 0f;

	int floorMask;
	float camReyLength = 100;

	[Header("Unity refs")]
	[SerializeField] CharacterController characterController;
	[SerializeField] GameObject rightHand;
	[SerializeField] GameObject leftHand;
	[SerializeField] GameObject cometPrefab;
	Camera mainCamera;
	GameObject rightHandMagic;
	GameObject leftHandMagic;

	void Update() {
		if (!isLocalPlayer)
			return;

		horizontal = Input.GetAxis("Horizontal");
		vertical = Input.GetAxis("Vertical");

		Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit floorHit;

		if (Physics.Raycast(camRay, out floorHit, camReyLength, floorMask)) {
			Vector3 playerToMouse = floorHit.point - transform.position;
			playerToMouse.y = 0;

			transform.rotation = Quaternion.LookRotation(playerToMouse);
		}

		if (Input.GetMouseButtonDown(0)) {
			CmdSpawnMagicPrefab(false);
		}

		if (Input.GetMouseButtonUp(0)) {
			Vector3 clickPos = new Vector3(floorHit.point.x, transform.position.y, floorHit.point.z);
			CmdReleaseMagicPrefab(false, clickPos);
		}
	}

	void FixedUpdate() {
		if (rightHandMagic)
			rightHandMagic.transform.position = rightHand.transform.position;
		if (leftHandMagic)
			leftHandMagic.transform.position = leftHand.transform.position;

		if (!isLocalPlayer)
			return;

		Vector3 direction = new Vector3(horizontal, 0, vertical);
		direction = Vector3.ClampMagnitude(direction, 1f);
		direction *= moveSpeed;
		characterController.SimpleMove(direction);
	}

	public override void OnStartServer() {
		base.OnStartServer();
		GetComponent<Health>().playerIndex = index;
	}

	public override void OnStartLocalPlayer() {
		base.OnStartLocalPlayer();

		mainCamera = Camera.main;
		floorMask = LayerMask.GetMask("Floor");
	}

	[Command]
	void CmdSpawnMagicPrefab(bool isLeft) {
		rightHandMagic = Instantiate(cometPrefab, rightHand.transform.position, Quaternion.identity);

		Attacker attacker = rightHandMagic.GetComponent<Attacker>();
		attacker.playerIndex = index;

		Health health = rightHandMagic.GetComponent<Health>();
		health.playerIndex = index;

		NetworkServer.Spawn(rightHandMagic);
	}

	[Command]
	void CmdReleaseMagicPrefab(bool isLeft, Vector3 releasePos) {
		rightHandMagic.GetComponent<Attacker>().OnReleaseMouse(releasePos);
		rightHandMagic = null;
	}
}
