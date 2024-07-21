using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyTracking : MonoBehaviour
{
    // Start is called before the first frame update
    public enum ChangeAngle
    {
        One = 90,
        Two = 180,
        Tree = 270
    }
    [Header("巡回時の角度調節")]
    [SerializeField] private ChangeAngle GetAngle = ChangeAngle.One;//デフォルトは９０度回転させる
    RaycastHit2D GetRay;//自分の視線
    RaycastHit2D ObstacleRay;//障害物を見分ける視線
    private Vector2 PlayerVec;//プレイヤーの位置を取得する
    private Vector2 MyVector;//自分の向き
    private Vector2 MoveDirection;
    private float _initialPosDistance;//初期位置と自分の距離
    private float _rayAngle;
    [Header("追跡時間")]
    [SerializeField] private float _trackingTime = 0;//追跡時間
    [Header("警戒時間")]
    [SerializeField] private float _alertTime = 0;//警戒時間
    [Header("プレイヤーと自分の距離")]
    [SerializeField] private float _playerDistance;//プレイヤーと自分の距離 
    [Header("レイの距離  ")]
    [SerializeField] private float _rayDistance = 5f;
    [Header("プレイヤーのレイヤー")]
    [SerializeField] private LayerMask TargetLayer;
    [Header("障害物のレイヤー")]
    [SerializeField] private LayerMask ObstacleLayer;    
    [Header("プレイヤーの位置")]
    [SerializeField] Transform TargetTrans;//プレイヤーの位置
    [Header("自分の追跡速度")]
    [SerializeField] private float _trackingSpeed = 5;//敵のスピード
    [Header("追跡フラグ")]
    [SerializeField] private bool TrackingFlag = false;//追跡フラグ
    Transform MyTrans;//自分の位置
    EnemyMove GetMove;//自分の動きを取得する
    EnemyVisionScript GetEnemyVision;//自分の視線を取得   
    NavMeshAgent2D GetAgent2D;
    void Start()
    {
       
        GetMove = this.GetComponent<EnemyMove>();//自分の動きを取得
        GetAgent2D = this.GetComponent<NavMeshAgent2D>();//じぶんのNavMeshAgent2Dを取得
        MyTrans = GetMove.MyTrans;//自分のTransformを取得
        GetEnemyVision = GetComponentInChildren<EnemyVisionScript>();//子オブジェクトからEnemyVisionScriptを取得
        //GetAgent = this.GetComponent<NavMeshAgent>();
        //GetAgent.updateRotation = false;
        //GetAgent.updateUpAxis = false;
    }

    // Update is called once per frame
    void Update()
    {
        //_rayAngle = Mathf.Atan2(PlayerVec.y, PlayerVec.x) * Mathf.Rad2Deg;
        //_rayAngle = MyTrans.eulerAngles.z * Mathf.Deg2Rad;
        MyVector = GetMove.MyTrans.position;//自分の向きを取得
        //ローテーションをヴェクターに突っ込めばいいかもしれない
        GetRay = Physics2D.Raycast(MyTrans.position, GetEnemyVision.VisionVec, _rayDistance, TargetLayer);//レイキャストを実行
        ObstacleRay = Physics2D.Raycast(MyTrans.position,GetEnemyVision.VisionVec, _rayDistance, ObstacleLayer);//障害物を識別するレイキャストを実行
        Debug.DrawRay(MyTrans.position, GetEnemyVision.VisionVec * _rayDistance, Color.red);//レイを可視化

        if (ObstacleRay&&GetEnemyVision.isPatrol)//巡回時、障害物に当たったら
        {
            GetEnemyVision._myRotation += (int)GetAngle;//視線をGetAngleで指定した角度に傾かせる
        }


        if (GetRay)//プレイヤーがレイに触れたら
        {
            TrackingFlag = true;
            _trackingTime = 0;
            _alertTime = 0;
        }

        else if (!GetRay && TrackingFlag)//レイにヒットしていないが、追いかけてる最中だったら
        {
            print("のがすな");
            _trackingTime += Time.deltaTime;          
        }


        if (_trackingTime >= 10 && _playerDistance >= 20)//プレイヤーを見失ったら  場合によってはorにする
        {
            TrackingFlag = false;          
            _alertTime += Time.deltaTime;
            print("どこ？");
            if (_alertTime >= 10)
            {
                GetAgent2D.SetDestination(GetMove.InitialPosition);
                _initialPosDistance = Vector2.Distance(MyVector, GetMove.InitialPosition);
                if (_initialPosDistance <= 0.01f)
                {
                    GetEnemyVision.isPatrol = true;
                    print("警備再開");
                }
               
                print("つかれた");
            }
        }

        if (TrackingFlag)//追跡フラグがオンだったら
        {
            print("みいつけた！！");
            _playerDistance = Vector2.Distance(PlayerVec, MyVector);//自分とプレイヤーの距離を計算
            PlayerVec = TargetTrans.position;//プレイヤーの位置を取得
            GetAgent2D.SetDestination(TargetTrans.position);//プレイヤーを追い掛け回す
            //MyTrans.position = Vector2.MoveTowards(MyTrans.position, new Vector2(TargetTrans.position.x, TargetTrans.position. y), _trackingSpeed * Time.deltaTime); //プレイヤーを追い掛け回す
            GetEnemyVision.isPatrol = false;
            GetEnemyVision.VisionVec = (TargetTrans.position - MyTrans.position).normalized;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))//プレイヤーと追突したら追跡開始
        {
            TrackingFlag = true;          
        }
    }
}
