using UnityEngine;
using BehaviorTreeFrame;

public class Condition_HP_Is:ConditionNode<BossController> {
    float _healthPointMin;
    float _healthPointMax;
    public Condition_HP_Is(float hpmin,float hpmax)
    {
        _healthPointMin = hpmin;
        _healthPointMax = hpmax;
    }

    public override bool doEvaluate()
    {
        //Debug.Log(myEntity.Health / myEntity.MaxHealth);
        if (((float)myEntity.Health/myEntity.MaxHealth) >= _healthPointMin&&
            ((float)myEntity.Health / myEntity.MaxHealth)<= _healthPointMax)
        {
            return true;
        }
        return false;
    }
}
