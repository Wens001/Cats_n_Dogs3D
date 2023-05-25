using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Obi;

[RequireComponent(typeof(ObiSolver))]
public class WrapRopeGameController : MonoBehaviour
{

	ObiSolver solver;
	[Header("切割")]
	private ObiRope rope;
	bool haveSplit = false;


	[Header("缠绕")]
	public bool HideColumns = false;
	public Wrappable[] wrappables;
	public ObiRope theRope;
	public UnityEvent onFinish = new UnityEvent();

	private void Awake()
	{
		solver = GetComponent<ObiSolver>();
		rope = GetComponentInChildren<ObiRope>();

        if (HideColumns)
        {
            foreach (var wrappable in wrappables)
            {
				wrappable.GetComponent<Renderer>().enabled = false;
            }
        }
	}

	// Start is called before the first frame update
	void OnEnable()
	{
		solver.OnCollision += Solver_OnCollision;
	}

	private void OnDisable()
	{
		solver.OnCollision -= Solver_OnCollision;
	}

	private void Update()
	{
		//缠绕
        if (wrappables.Length > 0)
        {
			bool allWrapped = true;

			// Test our win condition: all pegs must be wrapped.
			foreach (var wrappable in wrappables)
			{
				if (!wrappable.IsWrapped())
				{
					allWrapped = false;
					break;
				}
			}

			if (allWrapped)
			{
				onFinish.Invoke();
                //Messenger.Broadcast(StringMgr.GetWinCondition);

                if (theRope)
                {
					theRope.gameObject.SetActive(false);
                }
				Messenger.Broadcast(StringMgr.ShowTheResult);
			}
		}
		
	}

	private void Solver_OnCollision(ObiSolver s, ObiSolver.ObiCollisionEventArgs e)
	{
		// reset to unwrapped state:
		foreach (var wrappable in wrappables)
			wrappable.Reset();


		var world = ObiColliderWorld.GetInstance();
		foreach (Oni.Contact contact in e.contacts)
		{
			// look for actual contacts only:
			if (contact.distance < 0.025f)
			{
				var col = world.colliderHandles[contact.other].owner;
				if (col != null)
				{
					//缠绕判定
					var wrappable = col.GetComponent<Wrappable>();
                    if (wrappable != null)
						wrappable.SetWrapped();

                    //断裂判定
                    if (!haveSplit && col.CompareTag("Thorn") && GameControl.Instance.GameProcess == GameProcess.InGame)
                    {
						haveSplit = true;
						rope.ApplyTearingRightNow();

						GameControl.Instance.GameFail();
                    }
				}
			}
		}
	}


}
