using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Target of the camera")]
    private Transform target;

    [SerializeField]
    [Range(2f, 5f)]
    [Tooltip("Camera don´t go below this position")]
    private float maxYPosition;

    [SerializeField]
    [Range(1f, 5f)]
    [Tooltip("Duration of the camera animation")]
    private float animationDuration;

    // To concludes if finished animation
    private float transition = 0f;

    // Addictional position to camera finish the animation and after this it always be in this position
    private Vector3 startOffset;

    // Addictional coordenates to start the camera
    private Vector3 animationOffset = new Vector3(0f, 4f, 5f);

    void Start()
    {
        startOffset = transform.position - target.position;
    }

    void Update()
    {
        Vector3 finishPosition = target.position + startOffset;
        finishPosition.x = 0f;
        finishPosition.y = Mathf.Clamp(finishPosition.y, startOffset.y, maxYPosition);

        if (transition >= 1f)
        {
            transform.position = finishPosition;
        }
        else
        {
            transform.position = Vector3.Lerp(finishPosition + animationOffset, finishPosition, transition);
            transition += Time.deltaTime * 1f / animationDuration;
            transform.LookAt(target.position + Vector3.up * 2.8f);
        }
    }

    public float GetAnimationDuration()
    {
        return animationDuration;
    }

}