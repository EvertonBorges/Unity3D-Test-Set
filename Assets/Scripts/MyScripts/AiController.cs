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
    // When the player reaches this positions he needs to make the move in moveType.
    public float position;
    // Storage the moviment the player needs to do.
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

    private float lastPlayerPositionZ = 0f;

    private ObjectType[,] collisions = { { ObjectType.NONE, ObjectType.NONE, ObjectType.NONE }, { ObjectType.NONE, ObjectType.NONE, ObjectType.NONE } };

    // This list storage every player moviment when AI make a decision.
    private List<Move> moves = new List<Move>();

    private static bool focusToPickCoins = false;
    //private float distanceRaycast = 10f;

    void Awake()
    {
        _playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    void Update()
    {
        // AI need to be detecting collisions in front of the player.
        Vector3 finishPosition = _playerController.transform.position;
        finishPosition.x = 0f;
        finishPosition.y = 0f;
        finishPosition.z += 4f;

        transform.position = finishPosition;

        float playerPositionZ = _playerController.transform.position.z;

        if (playerPositionZ - lastPlayerPositionZ >= 1f) // 1f is to AI make any decision every timme when player run 1f in z axys
        {
            MakeDecision();
            ClearCollisions();
            lastPlayerPositionZ = Mathf.Floor(playerPositionZ);
        }

        MakeMove();
    }

    private void ClearCollisions()
    {
        for (int i = 0; i < collisions.GetLength(0); i++)
        {
            for (int j = 0; j < collisions.GetLength(1); j++)
            {
                collisions[i, j] = ObjectType.NONE;
            }
        }
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

    // Ai collisions call this function to update vector collisions in every collision and informs the ObjectType collision.
    public void UpdateCollisions(int row, int column, ObjectType value)
    {
        collisions[row, column] = value;
    }

    private void MakeDecision()
    {
        float x = Mathf.Round(_playerController.transform.position.x);
        bool playerInLeftLine = x < -0.95f;
        bool playerInMidLine = x >= -0.05f && x <= 0.05f;
        bool playerInRightLine = x > 0.95f;

        // When has a high obstacle, player needs to slide
        if (collisions[0, 0] == ObjectType.OBSTACLE && collisions[0, 1] == ObjectType.OBSTACLE && collisions[0, 2] == ObjectType.OBSTACLE)
        {
            if (moves.Count > 0 && moves[0].moveType == MoveType.SLIDE) return;

            Move move = new Move(transform.position.z - 3f, MoveType.SLIDE);
            moves.Add(move);
        }
        // When has a low obstacle, player needs to jump
        else if (collisions[1, 0] == ObjectType.OBSTACLE && collisions[1, 1] == ObjectType.OBSTACLE && collisions[1, 2] == ObjectType.OBSTACLE)
        {
            if (moves.Count > 0 && moves[0].moveType == MoveType.JUMP) return;

            Move move = new Move(transform.position.z - 3f, MoveType.JUMP);
            moves.Add(move);
        }
        // When a obstacle in left lane and player is in left line, player needs move to right
        else if ((collisions[0, 0] == ObjectType.OBSTACLE || collisions[1, 0] == ObjectType.OBSTACLE) && playerInLeftLine)
        {
            if (moves.Count > 0 && moves[0].moveType == MoveType.RIGHT) return;

            Move move = new Move(transform.position.z - 4f, MoveType.RIGHT);
            moves.Add(move);
        }
        // When a obstacle in right lane and player is in right line, player needs move to left
        else if ((collisions[0, 2] == ObjectType.OBSTACLE || collisions[1, 2] == ObjectType.OBSTACLE) && playerInRightLine)
        {
            if (moves.Count > 0 && moves[0].moveType == MoveType.LEFT) return;

            Move move = new Move(transform.position.z - 4f, MoveType.LEFT);
            moves.Add(move);
        }
        // When a obstacle in mid lane and player is in mid line, player needs move to left or right
        else if ((collisions[0, 1] == ObjectType.OBSTACLE || collisions[1, 1] == ObjectType.OBSTACLE) && playerInMidLine)
        {
            if (moves.Count > 0 && (moves[0].moveType == MoveType.RIGHT || moves[0].moveType == MoveType.RIGHT)) return;

            MoveType moveType = Random.Range(0, 100) < 50 ? MoveType.LEFT : MoveType.RIGHT;

            Move move = new Move(transform.position.z - 4f, moveType);
            moves.Add(move);
        }
        // When AI is focused to get coins (fishes)
        else if (focusToPickCoins)
        {
            // When a coin is in left lane and player is in right line, player needs move to left 2 times
            if (collisions[1, 0] == ObjectType.COIN && playerInRightLine)
            {
                if (moves.Count > 0 && moves[0].moveType == MoveType.LEFT) return;

                Move move1 = new Move(transform.position.z, MoveType.LEFT);
                Move move2 = new Move(transform.position.z, MoveType.LEFT);
                moves.Add(move1);
                moves.Add(move2);
            }
            // When a coin is in right lane and player is in left line, player needs move to right 2 times
            else if (collisions[1, 2] == ObjectType.COIN && playerInLeftLine)
            {
                if (moves.Count > 0 && moves[0].moveType == MoveType.RIGHT) return;

                Move move1 = new Move(transform.position.z, MoveType.RIGHT);
                Move move2 = new Move(transform.position.z, MoveType.RIGHT);
                moves.Add(move1);
                moves.Add(move2);
            }
            // When a coin is in left lane and player is in mid line, player needs move to left
            else if (collisions[1, 0] == ObjectType.COIN && playerInMidLine)
            {
                if (moves.Count > 0 && moves[0].moveType == MoveType.LEFT) return;

                Move move = new Move(transform.position.z, MoveType.LEFT);
                moves.Add(move);
            }
            // When a coin is in right lane and player is in mid line, player needs move to right
            else if (collisions[1, 2] == ObjectType.COIN && playerInMidLine)
            {
                if (moves.Count > 0 && moves[0].moveType == MoveType.RIGHT) return;

                Move move = new Move(transform.position.z, MoveType.RIGHT);
                moves.Add(move);
            }
            // When a coin is in mid lane and player is in right line, player needs move to left
            else if (collisions[1, 1] == ObjectType.COIN && playerInRightLine)
            {
                if (moves.Count > 0 && moves[0].moveType == MoveType.LEFT) return;

                Move move = new Move(transform.position.z, MoveType.LEFT);
                moves.Add(move);
            }
            // When a coin is in mid lane and player is in left line, player needs move to right
            else if (collisions[1, 1] == ObjectType.COIN && playerInLeftLine)
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