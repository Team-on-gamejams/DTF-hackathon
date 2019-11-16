using System;
using UnityEngine;

namespace Mirror.Examples.NetworkRoom {
	[RequireComponent(typeof(CharacterController))]
	public class PlayerController : NetworkBehaviour {
		[SyncVar] [NonSerialized]public int index;

		[Header("Movement Settings")]
		[SerializeField] float moveSpeed = 8f;
		float horizontal = 0f;
		float vertical = 0f;

		int floorMask;
		float camReyLength = 100;

		[Header("Unity refs")]
		[SerializeField] CharacterController characterController;
		Camera mainCamera;

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
				Vector3 clickPos = new Vector3(floorHit.point.x, transform.position.y, floorHit.point.z);
				Debug.DrawLine(transform.position, clickPos, Color.red, 5.0f);
			}
		}

		void FixedUpdate() {
			if (!isLocalPlayer)
				return;

			Vector3 direction = new Vector3(horizontal, 0, vertical);
			direction = Vector3.ClampMagnitude(direction, 1f);
			direction *= moveSpeed;
			characterController.SimpleMove(direction);
		}

		public override void OnStartLocalPlayer() {
			base.OnStartLocalPlayer();

			mainCamera = Camera.main;
			floorMask = LayerMask.GetMask("Floor");
		}
	}
}
