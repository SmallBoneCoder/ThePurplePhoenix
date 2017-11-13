

namespace BehaviorTreeFrame
{
    /// <summary>
    /// 顺序结点，一个子节点返回FALSE，执行结束
    /// 
    /// </summary>
    class SequenceNode<Entity> : CompositeNode<Entity>
    {


        public SequenceNode():base()
        {

        }
        public SequenceNode(ConditionNode<Entity> condition) : base(condition)
        {

        }

        /// <summary>
        ///当执行本类型Node时，它将从begin到end迭代执行自己的Child Node：
        ///如遇到一个Child Node执行后返回False，那停止迭代，
        ///本Node向自己的Parent Node也返回False；否则所有Child Node都返回True，
        ///那本Node向自己的Parent Node返回True。
        /// </summary>
        /// <returns></returns>
        public override BTResultStatus DoTick()
        {
            //throw new NotImplementedException();
            //顺序执行所有子结点
            for(int i = 0; i < this.ChildNodes.Count; i++)
            {
                if (ChildNodes[i].DoTick()==BTResultStatus.Error)//相当于FALSE
                {
                    return BTResultStatus.Error;//相当于FALSE
                }
                    
            }

            return BTResultStatus.Ended;//TRUE
        }

       
    }
}
