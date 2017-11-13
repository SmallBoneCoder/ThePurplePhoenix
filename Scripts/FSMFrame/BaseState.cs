using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FSMFrame
{
    public  class BaseState<Entity_Type>
    {
        public StateMachine<Entity_Type> parent;//所属状态机
        public Entity_Type entity;
        public virtual void Enter(Entity_Type entity)
        {

        }
        public virtual void Execute(Entity_Type entity)
        {

        }
        public virtual void Exit(Entity_Type entity)
        {

        }

    }
}
