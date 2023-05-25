using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obi;
using DG.Tweening;

[RequireComponent(typeof(ObiSolver))]
public class ObiColliderResolver : MonoBehaviour
{
	ObiSolver solver;

	ObiSolver.ObiCollisionEventArgs collisionEvent;

	ObiColliderWorld world;
	private List<ObiColliderBase> contactsEnter = new List<ObiColliderBase>();
		 
	void Awake()
	{
		solver = GetComponent<ObiSolver>();
		world = ObiColliderWorld.GetInstance();
	}

	void OnEnable()
	{
		solver.OnCollision += Solver_OnCollision;
	}

	void OnDisable()
	{
		solver.OnCollision -= Solver_OnCollision;
	}

	void Solver_OnCollision(object sender, ObiSolver.ObiCollisionEventArgs e)
	{
		foreach (Oni.Contact contact in e.contacts)
		{
			// this one is an actual collision:
			if (contact.distance < 0.01)
			{

				ObiColliderBase collider = world.colliderHandles[contact.other].owner;
				if (collider != null && collider.name.Contains("Grass")  && !contactsEnter.Contains(collider))
				{
					contactsEnter.Add(collider);

					collider.gameObject.SetActive(false);

					return;
				}
			}
		}
	}


}
