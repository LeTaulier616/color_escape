using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyScript : MonoBehaviour 
{
	[SerializeField]
	public GameObject pathArray;

	[SerializeField]
	public List<Transform> pathWaypoints = new List<Transform>();

	[SerializeField]
	public float[] waypointSpeed;

	[SerializeField]
	public float[] waypointWait;
	
	[SerializeField]
	public bool[] waypointRotate;
	
	[SerializeField]
	public float[] waypointAngle;
	
	[SerializeField]
	public float[] waypointRotateTime;

	[SerializeField]
	public int arraySize;

	[SerializeField]
	public bool showArray;

	[SerializeField]
	public float fieldOfView;

	[SerializeField]
	public float sightDistance;

	[SerializeField]
	public int rayMask;

	private GameObject player;
	
	private bool isMoving;
	private bool isRotating;
	private bool reversePath;

	private float moveTime;
	private int currentWaypoint;

	private RaycastHit hitInfo;

	private float playerDistance;
	private float playerAngle;
	
	private Quaternion toAngle;

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
		isRotating = false;
		reversePath = false;
		
		toAngle = Quaternion.identity;

		transform.position = pathWaypoints[currentWaypoint].position;

		player = GameObject.FindGameObjectWithTag("Player");
	}
	
	// Update is called once per frame
	void Update () 
	{
		playerDistance = Vector3.Distance(transform.position, player.transform.position);
		playerAngle = Vector3.Angle(transform.forward, player.transform.position - transform.position);

		Move();
		Look ();

		if(!isRotating)
		{
			Quaternion waypointLookAt = Quaternion.LookRotation(pathWaypoints[currentWaypoint].position - transform.position);
			RotatePlayer(waypointLookAt);
		}
			
		else
		{
			RotatePlayer(toAngle);
		}
	}

	void Move()
	{
		if(isMoving)
		{
			if( !CheckDistance(transform.position, pathWaypoints[currentWaypoint].position, 0.001f) )
			{
				transform.position = Vector3.MoveTowards(transform.position,
				                                         pathWaypoints[currentWaypoint].position,
				                                         waypointSpeed[currentWaypoint] * Time.deltaTime);

				/*
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
				*/
			}

			else
			{
				//moveTime = 0.0f;
				OnWaypointReached(currentWaypoint);
			}
		}
	}

	void Look()
	{
		if(playerDistance <= sightDistance && playerAngle <= fieldOfView)
		{
			if(Physics.Raycast(transform.position, Camera.main.transform.position - transform.position, out hitInfo, sightDistance, rayMask))
			{				
				if(hitInfo.collider.CompareTag("Player"))
				{
					Debug.LogError("Spotted !");
				}
			}
		}
	}

	void OnDrawGizmos()
	{
		Vector3 origin = Vector3.zero;
		Vector3 end = Vector3.forward * sightDistance;

		Vector3 leftEdge = Quaternion.AngleAxis(-fieldOfView / 2.0f, Vector3.up) * end;
		Vector3 rightEdge = Quaternion.AngleAxis(fieldOfView / 2.0f, Vector3.up) * end;
		Vector3 topEdge = Quaternion.AngleAxis(-fieldOfView / 2.0f, Vector3.right) * end;
		Vector3 bottomEdge = Quaternion.AngleAxis(fieldOfView / 2.0f, Vector3.right) * end;

		Gizmos.matrix = transform.localToWorldMatrix;
		Gizmos.color = Color.red;

		Gizmos.DrawLine(origin, end);

		Gizmos.DrawLine(origin, leftEdge);
		Gizmos.DrawLine(origin, rightEdge);
		Gizmos.DrawLine(origin, topEdge);
		Gizmos.DrawLine(origin, bottomEdge);

		Gizmos.DrawLine(end, leftEdge);
		Gizmos.DrawLine(end, rightEdge);
		Gizmos.DrawLine(end, topEdge);
		Gizmos.DrawLine(end, bottomEdge);

		DrawWireCircle(leftEdge, topEdge);
		DrawWireCircle(leftEdge, bottomEdge);
		DrawWireCircle(rightEdge, topEdge);
		DrawWireCircle(rightEdge, bottomEdge);

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
		
		if(waypointRotate[waypointNumber])
		{
			SetRotationQuaternion(waypointAngle[waypointNumber]);
			if(waypointRotateTime[waypointNumber] > 0.0f)
			{
				Invoke("EnableRotation", waypointRotateTime[waypointNumber]);;
			}
			
			else
			{
				isRotating = true;
			}
		}
		
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
			isRotating = false;
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
			isRotating = false;
			return;
		}
	}
	
	void RotatePlayer(Quaternion angle)
	{
		transform.rotation = Quaternion.RotateTowards(transform.rotation, angle, 700.0f * Time.deltaTime);
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

	void DrawWireCircle(Vector3 startPoint, Vector3 endPoint)
	{
		Vector3 startGizmo = startPoint;
		Vector3 endGizmo   = new Vector3(
			startPoint.x + 1.0f*(endPoint.x - startPoint.x)/8.0f,
			startPoint.y + 3.5f*(endPoint.y - startPoint.y)/8.0f,
			startPoint.z
			);

		Gizmos.DrawLine(startGizmo, endGizmo);

		startGizmo = endGizmo;
		endGizmo = new Vector3(
			startPoint.x + 2.0f*(endPoint.x - startPoint.x)/8.0f,
			startPoint.y + 5.0f*(endPoint.y - startPoint.y)/8.0f,
			startPoint.z
			);

		Gizmos.DrawLine(startGizmo, endGizmo);

		startGizmo = endGizmo;
		endGizmo = new Vector3(
			startPoint.x + 2.5f*(endPoint.x - startPoint.x)/8.0f,
			startPoint.y + 5.5f*(endPoint.y - startPoint.y)/8.0f,
			startPoint.z
			);

		Gizmos.DrawLine(startGizmo, endGizmo);

		startGizmo = endGizmo;
		endGizmo = new Vector3(
			startPoint.x + 3.0f*(endPoint.x - startPoint.x)/8.0f,
			startPoint.y + 6.0f*(endPoint.y - startPoint.y)/8.0f,
			startPoint.z
			);

		Gizmos.DrawLine(startGizmo, endGizmo);

		startGizmo = endGizmo;
		endGizmo = new Vector3(
			startPoint.x + 4.5f*(endPoint.x - startPoint.x)/8.0f,
			startPoint.y + 7.0f*(endPoint.y - startPoint.y)/8.0f,
			startPoint.z
			);

		Gizmos.DrawLine(startGizmo, endGizmo);

		startGizmo = endGizmo;
		endGizmo = new Vector3(
			startPoint.x + 8.0f*(endPoint.x - startPoint.x)/8.0f,
			startPoint.y + 8.0f*(endPoint.y - startPoint.y)/8.0f,
			startPoint.z
			);

		Gizmos.DrawLine(startGizmo, endGizmo);
	}
	
	
	void EnableRotation()
	{
		isRotating = true;
	}
	
	void SetRotationQuaternion(float angle)
	{
		toAngle = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0.0f, angle, 0.0f));
	}
	
		//Turn on the bit using an OR operation
	private void ShowLayer(LayerMask mask, string name) 
	{
		mask |= 1 << LayerMask.NameToLayer(name);
	}
	
		//Turn off the bit using an AND operation with the complement of the shifted int
	private void HideLayer(LayerMask mask, string name) 
	{
		mask &=  ~(1 << LayerMask.NameToLayer(name));
	}
	
		//Toggle the bit using a XOR operation
	private void ToggleLayer(LayerMask mask, string name) 
	{
		mask ^= 1 << LayerMask.NameToLayer(name);
	}
}
