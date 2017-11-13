

namespace BehaviorTreeFrame
{
    /// <summary>
    /// 它完成具体的一次(或一个step)的行为，视需求返回值。
    ///而当行为需要分step/Node间进行时，可引入Blackboard进行简单数据交互。
    /// </summary>
    public class ActionNode<Entity>:BTNode<Entity>
    {

        
        /// <summary>
        /// 默认无前提条件
        /// </summary>
        public ActionNode():base()
        {
            condition_ = new ConditionNode<Entity>();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="condition">前提条件</param>
        public ActionNode(ConditionNode<Entity> condition) : base()
        {
            condition_ = condition;//判断节点
        }

        /// <summary>
        ///记录Action节点的状态
        /// </summary>
        private ActionNodeStatus thisActionNode_Status = ActionNodeStatus.READY;

        public  override BTResultStatus DoTick()
        {
            //throw new NotImplementedException();
            //BTResultStatus result=BTResultStatus.Running;
            if (condition_.DoTick() == BTResultStatus.Error)//不符合前提条件
            {
                //最后一次运行
                thisActionNode_Status = ActionNodeStatus.READY;
                Exit();//调用退出方法
                return BTResultStatus.Error;//退出执行返回Error，相当于FALSE
            }
            if (thisActionNode_Status == ActionNodeStatus.READY)//第一次运行
            {
                Enter();//调用进入方法
                thisActionNode_Status = ActionNodeStatus.Running;//标记为运行中
            }
            if (thisActionNode_Status == ActionNodeStatus.Running)
            {
                Execute();
                
            }
            return BTResultStatus.Ended;//相当于TRUE

        }
        /// <summary>
        /// 初进入方法
        /// </summary>
        public virtual void Enter()
        {
            //throw new NotImplementedException();
        }
        /// <summary>
        /// 执行方法
        /// </summary>
        /// <returns></returns>
        public virtual BTResultStatus Execute()
        {
            //throw new NotImplementedException();
            return BTResultStatus.Ended;

        }
        /// <summary>
        /// 退出方法
        /// </summary>
        public virtual void Exit()
        {
            //throw new NotImplementedException();
        }
        /// <summary>
        /// 行为节点的状态
        /// </summary>
        private enum ActionNodeStatus{
            READY=1,
            Running=2
        }
    }
}
