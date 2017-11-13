

namespace BehaviorTreeFrame
{
    
    /// <summary>
    /// Decorator Node，它的功能正如它的字面意思：它将它的Child Node执行
    ///后返回的结果值做额外处理后，再返回给它的Parent Node。
    /// </summary>
    public class DecoratorNode<Entity> : BTNode<Entity>
    {
        public BTNode<Entity> childNode;
        public override BTResultStatus DoTick()
        {
            
            return DecoratorMethod();
            //throw new NotImplementedException();
        }
        public virtual BTResultStatus DecoratorMethod()
        {
            return BTResultStatus.Ended;
        }
        public void AddChild(BTNode<Entity> node)
        {
            childNode = node;
            node.parentNode = this;
            node.myEntity = myEntity;
        }
    }

    
}
