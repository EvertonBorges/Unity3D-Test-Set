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

public class Move
{
    public float position;
    public MoveType moveType;

    public Move() {}

    public Move(float position, MoveType moveType)
    {
        this.position = position;
        this.moveType = moveType;
    }

    public override string ToString()
    {
        return "(" + position + ", " + moveType + ")";
    }
}

public class AiController : MonoBehaviour
{

    private PlayerController _playerController;

    private float lastPosition = 0f;

    private ObjectType[,] collisions = { { ObjectType.NONE, ObjectType.NONE, ObjectType.NONE }, { ObjectType.NONE, ObjectType.NONE, ObjectType.NONE } };

    private List<Move> moves = new List<Move>();

    private static bool focusToPickCoins = false;
    //private float distanceRaycast = 10f;

    void Awake()
    {
        _playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    void Update()
    {
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

        MakeMove();
    }

    /*
    void FixedUpdate()
    {
        Vector3 playerPosition = _playerController.transform.position;
        playerPosition.y = 0;
        Vector3 playerHighPosition = playerPosition + Vector3.up * 1.15f;
        Vector3 playerLowPosition = playerPosition + Vector3.up * 0.3f;
        RaycastHit hitUp;
        RaycastHit hitDown;
        Vector3 half = new Vector3(0.85f, 0.5f, 1f);
        bool isHitingUp = Physics.BoxCast(playerHighPosition, half / 2, Vector3.forward, out hitUp, Quaternion.identity, distanceRaycast);
        bool isHitingDown = Physics.BoxCast(playerLowPosition, half / 2, Vector3.forward, out hitDown, Quaternion.identity, distanceRaycast);

        if (isHitingDown || isHitingUp)
        {
            if (isHitingUp && hitUp.collider.CompareTag("Obstacle"))
            {
                Debug.Log("isHitingUp: " + isHitingUp);
            }
            if (isHitingDown && hitDown.collider.CompareTag("Obstacle"))
            {
                Debug.Log("isHitingDown: " + isHitingDown);
            }

            if (isHitingUp && !isHitingDown && !_playerController.IsSliding() &&
                hitUp.collider.CompareTag("Obstacle") && (moves.Count == 0 || moves[0].moveType != MoveType.SLIDE))
            {
                Move move = new Move(hitUp.point.z - 3f, MoveType.SLIDE);
                moves.Add(move);

                Debug.Log(_playerController.transform.position.z + " - " + move.ToString());
            }
            else if (!isHitingUp && isHitingDown && !_playerController.IsJumping() &&
                hitDown.collider.CompareTag("Obstacle") && (moves.Count == 0 || moves[0].moveType != MoveType.JUMP))
            {
                Move move = new Move(hitDown.point.z - 3f, MoveType.JUMP);
                moves.Add(move);

                Debug.Log(_playerController.transform.position.z + " - " + move.ToString());
            }
            else if (isHitingUp && isHitingDown && !_playerController.IsMovingLeft() && !_playerController.IsMovingRigth() &&
                hitUp.collider.CompareTag("Obstacle") && (moves.Count == 0 || (moves[0].moveType != MoveType.LEFT && moves[0].moveType != MoveType.RIGHT)))
            {
                float x = Mathf.Round(_playerController.transform.position.x);
                bool isInLeftLine = x < -0.95f;
                bool isInMidLine = x >= -0.05f && x <= 0.05f;
                bool isInRightLine = x > 0.95f;

                Move move = new Move();

                if (isInLeftLine)
                {
                    move.moveType = MoveType.RIGHT;
                }
                else if (isInRightLine)
                {
                    move.moveType = MoveType.LEFT;
                }
                else if (isInMidLine)
                {
                    move.moveType = Random.Range(0, 100) < 50 ? MoveType.LEFT : MoveType.RIGHT;
                }

                move.position = hitDown.point.z - 3f;

                moves.Add(move);

                Debug.Log(_playerController.transform.position.z + " - " + move.ToString());
            }
        }
    }
    */

    private void MakeMove()
    {
        if (moves.Count > 0)
        {
            float zPosition = moves[0].position;
            if (_playerController.transform.position.z > zPosition)
            {
                MoveType move = moves[0].moveType;
                bool madeMove = false;
                switch (move)
                {
                    case MoveType.JUMP: madeMove = _playerController.Jump(); break;
                    case MoveType.SLIDE: madeMove = _playerController.Slide(); break;
                    case MoveType.LEFT: _playerController.MovePlayerHorizontal(false); madeMove = true; break;
                    case MoveType.RIGHT: _playerController.MovePlayerHorizontal(true); madeMove = true; break;
                }

                if (madeMove)
                {
                    moves.RemoveAt(0);
                }
            }

        }
    }

    public void UpdateCollisions(int row, int column, ObjectType value)
    {
        collisions[row, column] = value;
    }

    private void MakeDecision()
    {
        float x = Mathf.Round(_playerController.transform.position.x);
        bool isInLeftLine = x < -0.95f;
        bool isInMidLine = x >= -0.05f && x <= 0.05f;
        bool isInRightLine = x > 0.95f;

        if (collisions[0, 0] == ObjectType.OBSTACLE && collisions[0, 1] == ObjectType.OBSTACLE && collisions[0, 2] == ObjectType.OBSTACLE)
        {
            if (moves.Count > 0 && moves[0].moveType == MoveType.SLIDE) return;

            Move move = new Move(transform.position.z - 3f, MoveType.SLIDE);
            moves.Add(move);
        }
        else if (collisions[1, 0] == ObjectType.OBSTACLE && collisions[1, 1] == ObjectType.OBSTACLE && collisions[1, 2] == ObjectType.OBSTACLE)
        {
            if (moves.Count > 0 && moves[0].moveType == MoveType.JUMP) return;

            Move move = new Move(transform.position.z - 3f, MoveType.JUMP);
            moves.Add(move);
        }
        else if ((collisions[0, 0] == ObjectType.OBSTACLE || collisions[1, 0] == ObjectType.OBSTACLE) && isInLeftLine)
        {
            if (moves.Count > 0 && moves[0].moveType == MoveType.RIGHT) return;

            Move move = new Move(transform.position.z - 4f, MoveType.RIGHT);
            moves.Add(move);
        }
        else if ((collisions[0, 2] == ObjectType.OBSTACLE || collisions[1, 2] == ObjectType.OBSTACLE) && isInRightLine)
        {
            if (moves.Count > 0 && moves[0].moveType == MoveType.LEFT) return;

            Move move = new Move(transform.position.z - 4f, MoveType.LEFT);
            moves.Add(move);
        }
        else if ((collisions[0, 1] == ObjectType.OBSTACLE || collisions[1, 1] == ObjectType.OBSTACLE) && isInMidLine)
        {
            if (moves.Count > 0 && (moves[0].moveType == MoveType.RIGHT || moves[0].moveType == MoveType.RIGHT)) return;

            MoveType moveType = Random.Range(0, 100) < 50 ? MoveType.LEFT : MoveType.RIGHT;

            Move move = new Move(transform.position.z - 4f, moveType);
            moves.Add(move);
        }
        else if (focusToPickCoins)
        {
            if (collisions[1, 0] == ObjectType.COIN && isInRightLine)
            {
                if (moves.Count > 0 && moves[0].moveType == MoveType.LEFT) return;

                Move move1 = new Move(transform.position.z, MoveType.LEFT);
                Move move2 = new Move(transform.position.z, MoveType.LEFT);
                moves.Add(move1);
                moves.Add(move2);
            }
            else if (collisions[1, 2] == ObjectType.COIN && isInLeftLine)
            {
                if (moves.Count > 0 && moves[0].moveType == MoveType.RIGHT) return;

                Move move1 = new Move(transform.position.z, MoveType.RIGHT);
                Move move2 = new Move(transform.position.z, MoveType.RIGHT);
                moves.Add(move1);
                moves.Add(move2);
            }
            else if (collisions[1, 0] == ObjectType.COIN && isInMidLine)
            {
                if (moves.Count > 0 && moves[0].moveType == MoveType.LEFT) return;

                Move move = new Move(transform.position.z, MoveType.LEFT);
                moves.Add(move);
            }
            else if (collisions[1, 2] == ObjectType.COIN && isInMidLine)
            {
                if (moves.Count > 0 && moves[0].moveType == MoveType.RIGHT) return;

                Move move = new Move(transform.position.z, MoveType.RIGHT);
                moves.Add(move);
            }
            else if (collisions[1, 1] == ObjectType.COIN && isInRightLine)
            {
                if (moves.Count > 0 && moves[0].moveType == MoveType.LEFT) return;

                Move move = new Move(transform.position.z, MoveType.LEFT);
                moves.Add(move);
            }
            else if (collisions[1, 1] == ObjectType.COIN && isInLeftLine)
            {
                if (moves.Count > 0 && moves[0].moveType == MoveType.RIGHT) return;

                Move move = new Move(transform.position.z, MoveType.RIGHT);
                moves.Add(move);
            }
        }
    }

    public static void SetFocusCoin(bool focus)
    {
        focusToPickCoins = focus;
    }

}