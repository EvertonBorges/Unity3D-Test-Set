using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    private Transform target;

    [SerializeField]
    [Range(2f, 5f)]
    private float maxYPosition;

    [SerializeField]
    [Range(1f, 5f)]
    private float animationDuration;

    private float transition = 0f;

    private Vector3 startOffset;
    private Vector3 animationOffset = new Vector3(0f, 5f, 5f);

    // Start is called before the first frame update
    void Start()
    {
        startOffset = transform.position - target.position;
    }

    // Update is called once per frame
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
            transform.LookAt(target.position + Vector3.up);
        }
    }

    public float GetAnimationDuration()
    {
        return animationDuration;
    }

}