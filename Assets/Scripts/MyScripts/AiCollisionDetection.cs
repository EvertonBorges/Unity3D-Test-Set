using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiCollisionDetection : MonoBehaviour
{

    [SerializeField]
    private AiController aiController;

    [SerializeField]
    private int row;

    [SerializeField]
    private int column;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            aiController.UpdateCollisions(row, column, ObjectType.OBSTACLE);
        }
        if (other.CompareTag("Coin"))
        {
            aiController.UpdateCollisions(row, column, ObjectType.COIN);
        }
    }

}
