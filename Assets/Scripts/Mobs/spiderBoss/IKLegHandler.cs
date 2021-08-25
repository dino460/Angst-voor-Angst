using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKLegHandler : MonoBehaviour
{
	[SerializeField] private Rigidbody2D rootRigidbody2D;
	[SerializeField] private IKLegHandler legToSync;
	[SerializeField] private IKLegHandler legToRestrict;
	[SerializeField] private bool showGizmos;
	[SerializeField] private int centreTransformUp_mod;


	[SerializeField] private LayerMask groundLayer;
	[SerializeField] private Transform centreTransform;
	[SerializeField] private Transform   handTransform;


	[SerializeField] private float maxDist;
	[SerializeField] private float minDist;
	[SerializeField] private float distanceFromRoot;
	[SerializeField] private float raycastAngularStep;
					 private float handToCentreDistance;


	private void Start()
	{
		distanceFromRoot = ((maxDist - minDist) / 2f) + minDist;
	}


	private void Update()
	{
		handToCentreDistance = Vector2.Distance(centreTransform.position, handTransform.position);
		
		if ((handToCentreDistance > maxDist || 
			 handToCentreDistance < minDist ||
			 legToSync.handToCentreDistance > legToSync.maxDist || 
			 legToSync.handToCentreDistance < legToSync.minDist))
		{
			handTransform.position = GetNewTargetPosition(distanceFromRoot);
		}
	}


	/* Code adapted from https://torchinsky.me/kwad-devlog-5 */
	private Vector2 GetNewTargetPosition(float distanceFromRoot)
	{
		// Determine start and end point for LineCast
		var linecastStartPos = centreTransformUp_mod * centreTransform.up * distanceFromRoot;
		var linecastEndPos = linecastStartPos;

		// Rotate end point to specified angle around root
		var rot = Quaternion.AngleAxis(raycastAngularStep, centreTransform.forward);
		var steps = Mathf.CeilToInt(180 / Mathf.Abs(raycastAngularStep));

		// Looping through LineCasts until it finds any point
		for (int i = 0; i < steps; i++)
		{
			linecastEndPos = rot * linecastStartPos;
			
			RaycastHit2D hit = Physics2D.Linecast(
				centreTransform.position + linecastStartPos, 
				centreTransform.position + linecastEndPos,
				groundLayer);

			if (hit)
			{
				// Point found, exiting function
				Vector2 _newTargetPosition = hit.point;
				return _newTargetPosition;
			}
			linecastStartPos = linecastEndPos;
		}

		return handTransform.position;
	}


	private void OnDrawGizmos()
	{
		if (showGizmos)
		{
		distanceFromRoot = ((maxDist - minDist) / 2f) + minDist;
	
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireSphere(centreTransform.position, distanceFromRoot);
	
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(centreTransform.position, minDist);
			Gizmos.DrawWireSphere(centreTransform.position, maxDist);
		}
	}
}
