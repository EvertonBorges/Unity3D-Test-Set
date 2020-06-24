using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MoveType
{
    JUMP, SLIDE, LEFT, RIGHT
}

public enum ObjectType
{
    NONE, OBSTACLE, COIN
}

public class AiController : MonoBehaviour
{

    private PlayerController _playerController;

    private float lastPosition = 0f;

    private ObjectType[,] collisions = { { ObjectType.NONE, ObjectType.NONE, ObjectType.NONE }, { ObjectType.NONE, ObjectType.NONE, ObjectType.NONE } };

    private List<MoveType> moves = new List<MoveType>();
    private float zPositionToMakeMove = 0f;

    // Start is called before the first frame update
    void Awake()
    {
        _playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        MakeMove();

        Vector3 finishPosition = _playerController.transform.position;
        finishPosition.x = 0f;
        finishPosition.y = 0f;
        finishPosition.z += 4f;

        transform.position = finishPosition;

        float actualPosition = _playerController.transform.position.z;

        if (actualPosition - lastPosition >= 1f)
        {
            MakeDecision();

            for (int i = 0; i < collisions.GetLength(0); i++)
            {
                for (int j = 0; j < collisions.GetLength(1); j++)
                {
                    collisions[i, j] = ObjectType.NONE;
                }
            }

            lastPosition = Mathf.Floor(actualPosition);
        }
    }

    public void UpdateCollisions(int row, int column, ObjectType value)
    {
        collisions[row, column] = value;
    }

    private void MakeMove()
    {
        if (moves.Count > 0 && _playerController.transform.position.z >= zPositionToMakeMove)
        {
            MoveType move = moves[0];
            bool madeMove = false;
            switch (move)
            {
                case MoveType.JUMP: madeMove = _playerController.Jump(); break;
                case MoveType.SLIDE: madeMove = _playerController.Slide(); break;
                case MoveType.LEFT: _playerController.MovePlayerHorizontal(false); madeMove = true; break;
                case MoveType.RIGHT: _playerController.MovePlayerHorizontal(true); madeMove = true; break;
            }

            if (madeMove) moves.RemoveAt(0);
        }
    }

    private void MakeDecision()
    {
        float x = Mathf.Round(_playerController.transform.position.x);

        if (collisions[0, 0] == ObjectType.OBSTACLE && collisions[0, 1] == ObjectType.OBSTACLE && collisions[0, 2] == ObjectType.OBSTACLE)
        {
            moves.Add(MoveType.SLIDE);
        }
        else if (collisions[1, 0] == ObjectType.OBSTACLE && collisions[1, 1] == ObjectType.OBSTACLE && collisions[1, 2] == ObjectType.OBSTACLE)
        {
            moves.Add(MoveType.JUMP);
        }
        else if ((collisions[0, 0] == ObjectType.OBSTACLE || collisions[1, 0] == ObjectType.OBSTACLE) && x < -0.5f)
        {
            moves.Add(MoveType.RIGHT);
        }
        else if ((collisions[0, 2] == ObjectType.OBSTACLE || collisions[1, 2] == ObjectType.OBSTACLE) && x > 0.5f)
        {
            moves.Add(MoveType.LEFT);
        }
        else if ((collisions[0, 1] == ObjectType.OBSTACLE || collisions[1, 1] == ObjectType.OBSTACLE) && x >= -0.5f && x <= 0.5f)
        {
            moves.Add(Random.Range(0, 100) < 50 ? MoveType.LEFT : MoveType.RIGHT);
        }

        zPositionToMakeMove = transform.position.z - 4f;
    }

}