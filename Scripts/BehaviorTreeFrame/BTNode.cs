

namespace BehaviorTreeFrame
{
     public class BTNode<Entity>
    {

        protected ConditionNode<Entity> condition_;
        public BTNode()
        {
            blackBoard = BlackBoard<Entity>.getInstance();
        }



        public Entity myEntity;
        
        /// <summary>
        /// 父节点
        /// </summary>
        public BTNode<Entity> parentNode;

        /// <summary>
        /// 黑板
        /// </summary>
        protected BlackBoard<Entity> blackBoard;

      

        /// <summary>
        /// 运行
        /// </summary>
        /// <returns>运行结果</returns>
        public virtual BTResultStatus DoTick() { return BTResultStatus.Ended; }
        /// <summary>
        /// 表示节点执行的状态
        /// </summary>
        public  enum BTResultStatus
        {
            /// <summary>
            /// 表示还在执行中
            /// </summary>
            Running=1,
            /// <summary>
            /// 相当于TRUE，也可以表示执行结束
            /// </summary>
            Ended=2,
            /// <summary>
            /// 相当于FALSE，执行失败
            /// </summary>
            Error=3
        }
    }
}
