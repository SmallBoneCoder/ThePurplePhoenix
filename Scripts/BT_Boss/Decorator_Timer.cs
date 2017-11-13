using UnityEngine;
using BehaviorTreeFrame;

public class Decorator_Timer : DecoratorNode<BossController> {

    public Decorator_Timer(float time) : base()
    {
        _cdTime = time;
        this.time = _cdTime;
    }
    private float _cdTime;
    BTResultStatus result=BTResultStatus.Error;
    //float allTime = 0;
    float time = 1f;
    public override BTResultStatus DecoratorMethod()
    {
        
        if (time<=0)
        {
            time = _cdTime;
            result = childNode.DoTick();
        }
        time -= Time.deltaTime;
        return result;
    }
}
