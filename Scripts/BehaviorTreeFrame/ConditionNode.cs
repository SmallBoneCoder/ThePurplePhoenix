


namespace BehaviorTreeFrame
{
    /// <summary>
    /// Condition Node，它仅当满足Condition时返回True。
    /// </summary>
    public class ConditionNode<Entity> : BTNode<Entity>
    {
        public override BTResultStatus DoTick()
        {
            //throw new NotImplementedException();
            if (Evaluate())
            {
                return BTResultStatus.Ended;
            }
            return BTResultStatus.Error;
        }
        /// <summary>
        ///节点是否能执行
        /// </summary>
        public bool activated = true;


        /// <summary>
        /// 验证
        /// </summary>
        /// <returns></returns>
        public bool Evaluate()
        {
            return activated && doEvaluate();
        }

        /// <summary>
        /// 做自己的验证
        /// </summary>
        /// <returns></returns>
        public virtual bool doEvaluate()
        {
            return true;
        }

    }
}
