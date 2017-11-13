


namespace BehaviorTreeFrame
{
    /// <summary>
    /// 
    /// </summary>
    class SelectorNode<Entity> : CompositeNode<Entity>
    {

        public SelectorNode():base()
        {

        }
        public SelectorNode(ConditionNode<Entity> condition) : base(condition)
        {

        }


        /// <summary>
        /// 当执行本类型Node时，它将从begin到end迭代执行自己的Child Node：
        ///如遇到一个Child Node执行后返回True，那停止迭代，
        ///本Node向自己的Parent Node也返回True；否则所有Child Node都返回False，
        ///那本Node向自己的Parent Node返回False。
        /// </summary>
        /// <returns></returns>
        public override BTResultStatus DoTick()
        {
            for(int i = 0; i < ChildNodes.Count; i++)
            {
                if (ChildNodes[i].DoTick()==BTResultStatus.Ended)//子节点返回TRUE
                {
                    return BTResultStatus.Ended;//本Node也返回TRUE,并退出
                }
            }
            return BTResultStatus.Error;
        }

       
    }
}
