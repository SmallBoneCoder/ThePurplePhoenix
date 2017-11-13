using UnityEngine;
using BehaviorTreeFrame;

public class BevTreeFactory  {
    private static BevTreeFactory instance;
    protected BevTreeFactory()
    {

    }
    public static BevTreeFactory GetInstance()
    {
        if (instance == null)
        {
            instance = new BevTreeFactory();
        }
        return instance;
    }

    public BTNode<BossController> CreateBevTree_Boss(BossController boss)
    {
        SelectorNode<BossController> root = new SelectorNode<BossController>
        {
            myEntity = boss
        };//选择节点__根节点
        root.myEntity = boss;
        Action_Patrol patrol = new Action_Patrol();//行为__巡逻
        Action_NormalAttack normalAtt_stage1 = new Action_NormalAttack(3, 2f);//行为__普通攻击__阶段1
        Action_NormalAttack normalAtt_stage2 = new Action_NormalAttack(4, 2f);//行为__普通攻击__阶段2
        Action_NormalAttack normalAtt_stage3 = new Action_NormalAttack(5, 2f);//行为__普通攻击__阶段3
        Action_Lazer1 lazer1 = new Action_Lazer1(10f);//行为__发射激光1
        Action_Lazer2 lazer2 = new Action_Lazer2(15f);//行为__发射激光2
        Action_CloseTo closeTo_stage1 = new Action_CloseTo(1f);//行为__靠近目标__阶段1
        Action_CloseTo closeTo_stage2 = new Action_CloseTo(2f);//行为__靠近目标__阶段2
        Action_CloseTo closeTo_stage3 = new Action_CloseTo(3f);//行为__靠近目标__阶段3
        SequenceNode<BossController> query = new SequenceNode<BossController>();//选择节点__查询
        SelectorNode<BossController> battle = new SelectorNode<BossController>();//选择节点__战斗
        SequenceNode<BossController> stage1 = new SequenceNode<BossController>();//顺序节点__阶段1
        SequenceNode<BossController> stage2 = new SequenceNode<BossController>();//顺序节点__阶段2
        SequenceNode<BossController> stage3 = new SequenceNode<BossController>();//顺序节点__阶段3
        Condition_HasTarget hasEnemy = new Condition_HasTarget();//条件节点__是否有敌人
        Condition_HP_Is hp_stage1 = new Condition_HP_Is(0.7f,1f);//条件节点__血量大于80%
        Condition_HP_Is hp_stage2 = new Condition_HP_Is(0.4f,0.7f);//条件节点__血量大于50%
        Condition_HP_Is hp_stage3 = new Condition_HP_Is(0,0.4f);//条件节点__血量大于20%
        Decorator_Timer timer = new Decorator_Timer(1f);//装饰节点__定时器_1秒更新一次结果
        //root
        {
            root.AddChild(query);
            root.AddChild(patrol);
        }
        //query
        {
            query.AddChild(timer);
            query.AddChild(battle);
        }
        //timer
        {
            timer.AddChild(hasEnemy);
        }
        //battle
        {
            battle.AddChild(stage1);
            battle.AddChild(stage2);
            battle.AddChild(stage3);
        }
        //stage1
        {
            stage1.AddChild(hp_stage1);
            stage1.AddChild(closeTo_stage1);
            stage1.AddChild(normalAtt_stage1);
        }
        //stage2
        {
            stage2.AddChild(hp_stage2);
            stage2.AddChild(closeTo_stage2);
            stage2.AddChild(normalAtt_stage2);
            stage2.AddChild(lazer1);
        }
        //stage3
        {
            stage3.AddChild(hp_stage3);
            stage3.AddChild(closeTo_stage3);
            stage3.AddChild(normalAtt_stage3);
            stage3.AddChild(lazer2);
        }
        return root;//返回根节点
    }
	
}
