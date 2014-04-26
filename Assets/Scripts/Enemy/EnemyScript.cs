using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyScript : MonoBehaviour {

	[SerializeField]
	public GameObject pathArray;

	[SerializeField]
	public List<Transform> pathWaypoints = new List<Transform>();

	[SerializeField]
	public float[] waypointSpeed;

	[SerializeField]
	public float[] waypointWait;

	[SerializeField]
	public int arraySize;

	[SerializeField]
	public bool showArray;

	private bool isMoving;
	private bool reversePath;

	private float moveTime;
	private int currentWaypoint;

	protected class WaypointComparer : IComparer
	{
		int IComparer.Compare(System.Object x, System.Object y)
		{ return ((new CaseInsensitiveComparer()).Compare(((Transform)x).name, ((Transform)y).name)); }
	}

	// Use this for initialization
	void Start ()
	{
		currentWaypoint = 0;

		isMoving = true;
		reversePath = false;

		transform.position = pathWaypoints[currentWaypoint].position;
	}
	
	// Update is called once per frame
	void Update () 
	{
		Move();

		if(!reversePath)
			transform.LookAt(pathWaypoints[currentWaypoint].position);

		else
			transform.LookAt(pathWaypoints[currentWaypoint].position);
	}

	void Move()
	{
		if(isMoving)
		{
			if( !CheckDistance(transform.position, pathWaypoints[currentWaypoint].position, 0.01f) )
			{
				if(!reversePath)
				{
					transform.position = Vector3.Lerp(pathWaypoints[currentWaypoint].position,
					                                  pathWaypoints[currentWaypoint - 1].position,
					                                  Mathf.Abs((moveTime % 2.0f) - 1.0f));
				}

				else
				{
					transform.position = Vector3.Lerp(pathWaypoints[currentWaypoint].position,
					                                  pathWaypoints[currentWaypoint + 1].position,
					                                  Mathf.Abs((moveTime % 2.0f) - 1.0f));
				}

				moveTime += Time.deltaTime / waypointSpeed[currentWaypoint];
			}

			else
			{
				moveTime = 0.0f;
				OnWaypointReached(currentWaypoint);
			}
		}
	}

	bool CheckDistance(Vector3 a, Vector3 b, float leniency)
	{
		if((a-b).sqrMagnitude <= (a * leniency).sqrMagnitude)
		{
			return true;
		}
		
		else
			return false;
	}

	void OnWaypointReached(int waypointNumber)
	{
		isMoving = false;

		if(waypointWait[waypointNumber] > 0.0f)
		{
			Invoke("ToNextWaypoint", waypointWait[waypointNumber]);
		}

		else
		{
			ToNextWaypoint();
		}
	}

	void ToNextWaypoint()
	{
		if(!reversePath)
		{
			currentWaypoint++;

			if(currentWaypoint >= pathWaypoints.Count)
			{
				currentWaypoint--;
				reversePath = true;
				ToNextWaypoint();
			}

			isMoving = true;
			return;
		}

		else
		{
			currentWaypoint--;

			if(currentWaypoint < 0)
			{
				currentWaypoint++;
				reversePath = false;
				ToNextWaypoint();
			}

			isMoving = true;
			return;
		}
	}

	public void GetWaypoints()
	{
		pathWaypoints.Clear();

		if(pathArray != null)
		{
			foreach (Component component in pathArray.GetComponentsInChildren(typeof(Component)))
			{
				if (component.GetType() != typeof(Transform))
				{
					Object.DestroyImmediate(component);
				}
			}

			foreach(Transform child in pathArray.transform)
			{
				pathWaypoints.Add(child);
				child.gameObject.AddComponent<EnemyWaypointGizmo>();
			}
		}
		
		IComparer comparer = new WaypointComparer();
		pathWaypoints.Sort(comparer.Compare);
	}
}
