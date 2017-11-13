using UnityEngine;
using BehaviorTreeFrame;

public class Action_Lazer1 : ActionNode<BossController>
{
    private float _cdTime;
    public Action_Lazer1(float time)
    {
        _cdTime = time;
        this.time = _cdTime;
    }
    float time = 10f;

    public override void Enter()
    {

    }
    public override BTResultStatus Execute()
    {
        if (time >= 0)
        {
            time -= Time.deltaTime;
            return BTResultStatus.Ended;
            
        }
        time = _cdTime;
        GameObject lazer1 = ObjectPool.GetInstance().CreateObject(myEntity.Lazer1, "enemylazer1");
        lazer1.GetComponent<EnemyBulletController>().InitState(myEntity.Head, "1");
        return BTResultStatus.Ended;
    }
    public override void Exit()
    {

    }
}
