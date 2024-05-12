using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] Animator _animator;

    [Header("GeneralSettings")]
    public Direct direct;
    [SerializeField] Transform modelTransform;
    [SerializeField] float speed;
    [SerializeField] Transform frontRayTransform;

    [Header("Status")]
    [SerializeField] bool isMoving;
    [SerializeField] bool isWin;
    [SerializeField] bool isStop;

    [Header("BrickSettings")]
    public int brickCount;
    [SerializeField] LayerMask brickLayer;
    [SerializeField] List<GameObject> BricksList;
    public int brickLeft => BricksList.Count;
    [SerializeField] GameObject brickAddPrefab;
    [SerializeField] float brickOffset = 0.5f;
    [SerializeField] float offsetGiveBackBrick;
    float brickWallHeight;

    [Header("DiamondSettings")]
    public int diamondCount;

    // Private property
    Vector3 startPoint;
    Vector3 endPoint;
    Vector3 targetPosition;
    RaycastHit hitFront;

    // AnimationID
    int aniIDSetInteger;

    private void Awake()
    {
        OnInit();
        aniIDSetInteger = Animator.StringToHash("AniFloat");
    }

    private void Start()
    {
        OnInit();
    }

    private void Update()
    {
        modelTransform.forward = Vector3.forward;

        if (isStop) return;
        if (isWin) return;

        FindDirection();
        Control();     
    }

    private void OnInit()
    {
        direct = Direct.None;
        brickCount = 0;
        brickWallHeight = -0.5f;
        isWin = false;
        isStop = false;
        diamondCount = 0;
        UIManager.instance.SetDiamond(diamondCount);
    }

    void OnDespawn()
    {

    }

    void FindDirection()
    {
        bool start = true;
        if (Input.GetMouseButtonDown(0))
        {
            startPoint = Input.mousePosition;
            start = true;
        }
        if (Input.GetMouseButtonUp(0) && start)
        {
            endPoint = Input.mousePosition;
            Vector3 tempVector = (endPoint - startPoint).normalized;
            float angle = Vector3.Angle(tempVector, Vector3.up);
            if (angle > 0 && angle < 45)
            {
                direct = Direct.Forward;
            }
            else if (angle > 45 && angle < 135 && tempVector.x > 0)
            {
                direct = Direct.Right;
            }
            else if (angle > 45 && angle < 135 && tempVector.x < 0)
            {
                direct = Direct.Left;
            }
            else if (angle > 135)
            {
                direct = Direct.Back;
            }
            Invoke(nameof(ResetDirection), 0.06f);
            start = false;
        }
    }

    void Control()
    {
        if (FrontCheck())
        {
            if (direct != Direct.None)
            {
                isMoving = true;
            }
            targetPosition = new Vector3(hitFront.collider.transform.position.x, transform.position.y, hitFront.collider.transform.position.z);
        }
        else
        {
            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                isMoving = false;
            }
        }

        if (!isMoving)
        {
            switch (direct)
            {
                case Direct.Forward:
                    transform.forward = Vector3.forward;
                    break;
                case Direct.Back:
                    transform.forward = -Vector3.forward;
                    break;
                case Direct.Right:
                    transform.forward = Vector3.right;
                    break;
                case Direct.Left:
                    transform.forward = -Vector3.right;
                    break;
                case Direct.None:
                    break;
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * speed);
        }
    }

    void ResetDirection()
    {
        direct = Direct.None;
    }

    bool FrontCheck()
    {
        Physics.Raycast(frontRayTransform.position, Vector3.down, out hitFront, 10f, brickLayer);
        return hitFront.collider != null;
    }

    void AddBrick()
    {
        brickCount++;
        GameObject brickClone = Instantiate(brickAddPrefab, transform);
        brickClone.transform.localPosition = new Vector3(0, brickWallHeight, 0);
        BricksList.Add(brickClone);
        brickWallHeight += brickOffset;
        modelTransform.localPosition = new Vector3(0, brickWallHeight, 0);
    }

    void RemoveBrick(Transform target)
    {
        GameObject returnBrick = Instantiate(brickAddPrefab, new Vector3(target.position.x, target.position.y + offsetGiveBackBrick, target.position.z), Quaternion.identity);
        returnBrick.transform.localRotation = Quaternion.Euler(-90, 0, 0);
        if (BricksList.Count > 0)
        {
            Destroy(BricksList[BricksList.Count - 1]);
            BricksList.Remove(BricksList[BricksList.Count - 1]);
        }
        brickWallHeight -= brickOffset;
        modelTransform.localPosition = new Vector3(0, brickWallHeight, 0);
    }

    void ClearBrick()
    {
        isWin = true;
        for(int i = BricksList.Count -1; i >= 0; i--)
        {
            Destroy(BricksList[i]);
            BricksList.Remove(BricksList[i]);
        }
        modelTransform.localPosition = new Vector3(0, -0.5f, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("BrickBlock"))
        {
            AddBrick();
            UIManager.instance.SetCoin(brickCount);
            Destroy(other.gameObject);
        }
        if (other.gameObject.CompareTag("UnBrickBlock"))
        {
            RemoveBrick(other.transform);
            other.gameObject.tag = "Untagged";
            if (BricksList.Count == 0)
            {
                isStop = true;
                ScenesManager.instance.Lose();
            }
        }
        if (other.gameObject.CompareTag("FinishBox"))
        {
            ClearBrick();
            _animator.SetInteger(aniIDSetInteger, (int)Ani.Win);
            ScenesManager.instance.Win();
        }

        if (other.gameObject.CompareTag("Diamond"))
        {
            isStop = true;
            int diamondThisReceive = ScenesManager.instance.DiamondPoint();
            diamondCount += diamondThisReceive;
            UIManager.instance.SetDiamond(diamondCount);
            UIManager.instance.SetNumber(diamondThisReceive);
            UIManager.instance.DiamondUIAni();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Diamond"))
        {
            Destroy(other.gameObject);
        }
    }

    public void ContinuePlayer()
    {
        isStop = false;
    }

    public enum Direct
    {
        Forward,
        Back,
        Right,
        Left,
        None,
    }
    public enum Ani
    {
        Idle = 1,
        Win = 2
    }
}



