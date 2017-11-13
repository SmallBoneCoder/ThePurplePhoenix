using UnityEngine;
using BehaviorTreeFrame;

public class Condition_HasTarget : ConditionNode<BossController>
{

    public override bool doEvaluate()
    {
        if (myEntity.Target != null&& myEntity.Target.gameObject.activeSelf)
        {
            float x = myEntity.Target.position.x;
            float y = myEntity.Target.position.y;
            float a = myEntity.transform.position.x;
            float b = myEntity.transform.position.y;
            //原目标在视野范围内继续追踪
            if ((x - a) * (x - a) + (y - b) * (y - b) <= myEntity.Sight * myEntity.Sight)
            {
                return true;
            }
        }
        Collider2D[] cos = Physics2D.OverlapCircleAll(myEntity.transform.position, myEntity.Sight);
        //Debug.Log("11");
        for (int i = 0; i < cos.Length; i++)
        {
            //Debug.Log(cos[i].gameObject);
            if (cos[i].gameObject.tag == "Player")
            {
                myEntity.Target = cos[i].transform;
                return true;//只追踪看到的第一个目标
            }
        }
        myEntity.Target = null;
        return false;
    }
}
