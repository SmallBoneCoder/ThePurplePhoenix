using UnityEngine;
using BehaviorTreeFrame;

public class Action_CloseTo : ActionNode<BossController>
{
    private float _pursuitSpeed;
    public Action_CloseTo(float speed)
    {
        _pursuitSpeed = speed;
    }

    public override void Enter()
    {
        base.Enter();
    }
    public override BTResultStatus Execute()
    {
        Vector3 dir= (myEntity.Target.transform.position - myEntity.transform.position).normalized;
        if (myEntity.transform.up != dir)
        {
            myEntity.transform.up = Vector3.MoveTowards(myEntity.transform.up, dir, 0.05f);
        }
        if (Vector3.Distance(myEntity.transform.position, myEntity.Target.position) > 7f)
        {
            myEntity.transform.position = 
                Vector3.MoveTowards(myEntity.transform.position, 
                myEntity.Target.position, 
                _pursuitSpeed * Time.deltaTime);
        }
        return BTResultStatus.Ended;
    }

    public override void Exit()
    {
        base.Exit();
    }
}
