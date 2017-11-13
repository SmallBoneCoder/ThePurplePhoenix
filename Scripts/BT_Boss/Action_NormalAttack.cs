using UnityEngine;
using BehaviorTreeFrame;

public class Action_NormalAttack : ActionNode<BossController>
{
    private float _cdTime;
    private float _cdRateTime = 0.4f;
    public Action_NormalAttack(int num,float time):base()
    {
        _attackNum = num;
        _cdTime = time;
        rateTime = _cdRateTime;
        this.time = _cdTime;
    }
    private int _attackNum = 3;
    private int _currentNum = 0;
    public override void Enter()
    {
        Debug.Log("boss：普通攻击"+_attackNum+"连击");
    }
    float time = 2f;
    float rateTime = 0.4f;
    public override BTResultStatus Execute()
    {
        if (time >= 0)//2秒攻击一轮
        {
            time -= Time.deltaTime;
            return BTResultStatus.Ended;
        }
        if (rateTime >= 0)//0.5秒攻击一次
        {
            rateTime -= Time.deltaTime;
            return BTResultStatus.Ended;
        }
        rateTime = _cdRateTime;
        if (_currentNum >= _attackNum)
        {
            time = _cdTime;
            _currentNum = 0;
        }
        //Vector3 dir = (myEntity.Target.transform.position - myEntity.transform.position).normalized;
        //myEntity.transform.up = dir;//头对着玩家
        
        GameObject bullet1 = ObjectPool.GetInstance().CreateObject(myEntity.NormalBullet, "enemybullet0");
        bullet1.GetComponent<EnemyBulletController>().InitState(myEntity.Head, "0");
        _currentNum++;
        return BTResultStatus.Ended;
    }
    public override void Exit()
    {

    }
}
