using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slingshot : MonoBehaviour
{
	static private Slingshot S;
	// Fields set in Unity Inspector
	[Header("Set in Inspector")]
	//public GameObject prefabProjectile;
	public float velocityMult = 8f;
	public GameObject[] projectiles;
	public int currentPrefabIndex;



	// Fields set dynamically
	[Header("Set Dynamically")]
	public GameObject launchPoint;
	public Vector3 launchPos;
	public GameObject projectile;
	public bool aimingMode;

	private Rigidbody projectileRigidbody;

	static public Vector3 LAUNCH_POS
	{
		get
		{
			if (S == null) return Vector3.zero;
			return S.launchPos;
		}
	}

	void Awake()
	{
		S = this;
		Transform launchPointTrans = transform.Find("LaunchPoint");
		launchPoint = launchPointTrans.gameObject;
		launchPoint.SetActive(false);
		launchPos = launchPointTrans.position;
	}


	void OnMouseEnter()
	{
		// print ("Slingshot:OnMouseEnter()");
		launchPoint.SetActive(true);
	}

	void OnMouseExit()
	{
		// print ("Slingshot:OnMouseExit()");
		launchPoint.SetActive(false);
	}

	public void TogglePrefab()
	{
		currentPrefabIndex = (currentPrefabIndex + 1) % projectiles.Length;
	}

	void OnMouseDown()
	{
		// The player has pressed the mouse button while over the Slingshot
		aimingMode = true;
		// Create a projectile
		if (currentPrefabIndex == 0)
		{
			//Debug.Log("its 0");
			projectile = Instantiate(projectiles[0]) as GameObject;
		}
		else if (currentPrefabIndex == 1)
		{
			//Debug.Log("its 1");
			projectile = Instantiate(projectiles[1]) as GameObject;

		}
		// Set its position to the launchPoint

		projectile.transform.position = launchPos;
		// Make it kinematic
		projectileRigidbody = projectile.GetComponent<Rigidbody>();
		projectileRigidbody.isKinematic = true;
	}

	void Update()
	{
		// If the slingshot is not in aiming mode, don't execute this code
		if (!aimingMode) return;

		// Get the current mouse position in 2D screen coordinates
		Vector3 mousePos2D = Input.mousePosition;
		mousePos2D.z = -Camera.main.transform.position.z;
		Vector3 mousePos3D = Camera.main.ScreenToWorldPoint(mousePos2D);

		// Find the delta from the launchPos to the mousePos3D
		Vector3 mouseDelta = mousePos3D - launchPos;
		// Limit mouseDelta to the radius of the Slingshot's SphereCollider
		float MaxMagnitude = this.GetComponent<SphereCollider>().radius;
		if (mouseDelta.magnitude > MaxMagnitude)
		{
			mouseDelta.Normalize();
			mouseDelta *= MaxMagnitude;
		}

		// Move the projectile to a new position
		Vector3 projPos = launchPos + mouseDelta;
		projectile.transform.position = projPos;
		if (Input.GetMouseButtonUp(0))
		{
			// The mouse button has been released
			aimingMode = false;
			projectileRigidbody.isKinematic = false;
			projectileRigidbody.velocity = -mouseDelta * velocityMult;
			FollowCam.POI = projectile;
			projectile = null;
			MissionDemolition.ShotFired();
			ProjectileLine.S.poi = projectile;
		}
	}
}
