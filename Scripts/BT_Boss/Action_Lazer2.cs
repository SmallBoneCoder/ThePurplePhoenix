using UnityEngine;
using BehaviorTreeFrame;
public class Action_Lazer2 : ActionNode<BossController>
{
    private float _cdTime;
    public Action_Lazer2(float time)
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
        GameObject lazer2 = ObjectPool.GetInstance().CreateObject(myEntity.Lazer2, "enemylazer2");
        lazer2.GetComponent<EnemyBulletController>().InitState(myEntity.transform, "2");
        return BTResultStatus.Ended;
    }
    public override void Exit()
    {

    }
}
