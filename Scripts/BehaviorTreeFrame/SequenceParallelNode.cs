


namespace BehaviorTreeFrame
{
    /// <summary>
    /// 并发执行它的所有Child Node。
    ///而向Parent Node返回的值和Parallel Node所采取的具体策略相关：
    ///Parallel Selector Node: 一False则返回False，全True才返回True。
    ///Parallel Sequence Node: 一True则返回True，全False才返回False。
    ///Parallel Hybird Node: 指定数量的Child Node返回True或False后才决定结果。
    ///Parallel Node提供了并发，提高性能。
    ///不需要像Selector/Sequence那样预判哪个Child Node应摆前，哪个应摆后，
    ///常见情况是：
    ///(1)用于并行多棵Action子树。
    ///(2)在Parallel Node下挂一棵子树，并挂上多个Condition Node，
    ///以提供实时性和性能。
    /// </summary>
    class SequenceParallelNode<Entity>:CompositeNode<Entity>
    {



        public SequenceParallelNode():base()
        {

        }
        public SequenceParallelNode(ConditionNode<Entity> condition) : base(condition)
        {

        }

        /// <summary>
        /// Parallel Sequence Node 顺序并行
        /// </summary>
        /// <returns></returns>
        public override BTResultStatus DoTick()
        {
            BTResultStatus result = BTResultStatus.Error;//默认为FALSE
            for (int i = 0; i < ChildNodes.Count; i++)//不管怎么样都会执行所有的子节点
            {
                if (ChildNodes[i].DoTick() == BTResultStatus.Ended)
                {
                    result = BTResultStatus.Ended;//设定返回值，一TRUE都TRUE
                }
            }

            return result;
        }

    }
}
