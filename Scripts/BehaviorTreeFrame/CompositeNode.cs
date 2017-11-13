
using System.Collections.Generic;


namespace BehaviorTreeFrame
{
    class CompositeNode<Entity> : BTNode<Entity>
    {
        public BTNode<Entity> CurrentRunningNode;
        public CompositeNode() : base()
        {
            condition_ = new ConditionNode<Entity>();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="condition">前提条件</param>
        public CompositeNode(ConditionNode<Entity> condition) : base()
        {
            condition_ = condition;//判断节点
        }



        /// <summary>
        /// 保存所有子结点
        /// </summary>
        private List<BTNode<Entity>> childNodes=new List<BTNode<Entity>>();

        
        /// <summary>
        /// 子节点集合
        /// </summary>
        public List<BTNode<Entity>> ChildNodes
        {
            get
            {
                return childNodes;
            }

            set
            {
                childNodes = value;
            }
        }
        /// <summary>
        /// 添加子节点
        /// </summary>
        /// <param name="child"></param>
        public void AddChild(BTNode<Entity> child)
        {
            
            child.myEntity = myEntity;//绑定父节点实体
            child.parentNode = this;//设定字节点的父节点
            childNodes.Add(child);//添加到子节点列表
        }
       
    }
}
