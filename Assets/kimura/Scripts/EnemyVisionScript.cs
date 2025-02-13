using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class EnemyVisionScript : MonoBehaviour
{
    public enum MoveDirection
    {
        Up,
        Down,
        Left,
        Right
    }
    public enum ChangeAngle
    {
        One = 90,
        Two = 180,
        Tree = 270
    }
    [Header("巡回時の移動方向")]
    [SerializeField] private MoveDirection _direction = MoveDirection.Right;
    [Header("巡回時の角度調節")]
    [SerializeField] private ChangeAngle GetAngle = ChangeAngle.One;//デフォルトは９０度回転させる
    [Header("障害物のレイヤー")]
    [SerializeField] private LayerMask ObstacleLayer;
    [Header("レイの距離  ")]
    [SerializeField] private float _rayDistance = 5f;
     private GameObject ParentObject;//親オブジェクトを格納する場所
    public Vector2 VisionVec;//視線の向き
    public float _myRotation;//視線の角度
    public Transform VisionTrans;//自分の位置
    [Header("巡回させるか制御する")]
    public bool isPatrol = true;//パトロール中かどうか制御する
    private float _radians;//角度を向きに変換するための数値
    
    RaycastHit2D ObstacleRay;//障害物を見分ける視線
    // Start is called before the first frame update
    void Start()
    {
        ParentObject = transform.parent.gameObject;
        print(ParentObject.name);
        VisionTrans = this.GetComponent<Transform>();
        _myRotation = VisionTrans.rotation.z;//視線の角度を取得
        if (isPatrol)
        {
            switch (_direction)//最初に移動する方向
            {
                case MoveDirection.Up:
                    _myRotation += 90;
                    break;
                case MoveDirection.Down:
                    _myRotation += 270;
                    break;
                case MoveDirection.Right:
                    _myRotation += 0;
                    break;
                case MoveDirection.Left:
                    _myRotation += 180;
                    break;
              
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        VisionControl();
        ObstacleRay = Physics2D.Raycast(VisionTrans.position, VisionVec, _rayDistance, ObstacleLayer);
        if (isPatrol && ObstacleRay.collider!=null && ObstacleRay.collider.gameObject!=ParentObject)//巡回時、障害物に当たったら
        {
            print("あたった");
            _myRotation += (int)GetAngle;//視線をGetAngleで指定した角度に傾かせる
        }

    }

    void VisionControl()
    {
        _radians = _myRotation * Mathf.Deg2Rad;//視線の角度を向きに変換
        VisionVec = new Vector2(Mathf.Cos(_radians), Mathf.Sin(_radians));//_radiansから視線の向きを取得
    }
}
