using UnityEngine;
using System.Collections;

[RequireComponent (typeof(CharacterController))]
public class ThirdPerson : MonoBehaviour {

	public GameObject cam;
	public GameObject model;
	public GameObject center;
	public float speed = 3.0f;
	public float jumpSpeed = 5.0f;
	public float gravityScale = 1.0f;

	public float camClose = 5.0f;
	public float camFar = 7.5f;
	public float camHeight = 5.0f;

	private CharacterController cc;
	private float vSpeed = 0;

	// Use this for initialization
	void Start () {
		cc = GetComponent<CharacterController> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown ("Quit")) {
			Application.Quit ();
		}

		float hor = Input.GetAxis ("Horizontal");
		float ver = Input.GetAxis ("Vertical");
		float amount = hor * hor + ver * ver;
		if (amount > 1) {
			amount = 1;
		}

		if (cc.isGrounded) {
			if(Input.GetButton("Jump"))
				vSpeed = jumpSpeed;
			else
				vSpeed = -0.01f;
		} else {
			vSpeed += Physics.gravity.y * Time.deltaTime * gravityScale;
		}

		if (hor == 0 && ver == 0) {
			//Only do gravity calculations
			cc.Move (new Vector3(0, vSpeed, 0) * Time.deltaTime);
			return;
		}

		float ang = Mathf.Rad2Deg * Mathf.Atan2 (ver, hor);
		if (model != null) {
			model.transform.localEulerAngles = new Vector3(0, -ang, 0);
		}
		ang -= transform.localEulerAngles.y;

		cc.Move (new Vector3(speed * amount * Mathf.Cos (Mathf.Deg2Rad * ang), vSpeed,
		                     speed * amount * Mathf.Sin (Mathf.Deg2Rad * ang)) * Time.deltaTime);
	}

	void LateUpdate() {
		if (cam != null) {
			float xLook = (center != null) ? center.transform.position.x : transform.position.x;
			float yLook = (center != null) ? center.transform.position.y : transform.position.y;
			float zLook = (center != null) ? center.transform.position.z : transform.position.z;

			float xRot = Mathf.Rad2Deg * Mathf.Atan2 (xLook - cam.transform.position.x,
			                                          zLook - cam.transform.position.z);
			float dist = new Vector2(zLook - cam.transform.position.z,
			                         xLook - cam.transform.position.x).magnitude;
			float xPos = cam.transform.position.x;
			float yPos = yLook + camHeight;
			float zPos = cam.transform.position.z;
			if(dist < camClose) {
				dist = camClose;
				zPos = zLook - dist * Mathf.Cos (Mathf.Deg2Rad * xRot);
				xPos = xLook - dist * Mathf.Sin (Mathf.Deg2Rad * xRot);
			}
			if(dist > camFar) {
				dist = camFar;
				zPos = zLook - dist * Mathf.Cos (Mathf.Deg2Rad * xRot);
				xPos = xLook - dist * Mathf.Sin (Mathf.Deg2Rad * xRot);
			}
			cam.transform.position = new Vector3 (xPos, yPos, zPos);
			float yRot = Mathf.Rad2Deg * Mathf.Atan2 (-camHeight, dist);
			cam.transform.localEulerAngles = new Vector3(-yRot, xRot, 0);

			transform.localEulerAngles = new Vector3(0, xRot, 0);
		}
	}
}
