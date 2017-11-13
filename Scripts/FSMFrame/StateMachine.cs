using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FSMFrame
{
    public class StateMachine<Entity_Type>
    {
        private Entity_Type owner;//状态机的所有者
        public Dictionary<string, BaseState<Entity_Type>> allStates;//存储所有状态
        private BaseState<Entity_Type> currentState;//当前状态
        private BaseState<Entity_Type> previousState;//上一个状态
        private BaseState<Entity_Type> defaultState;//默认状态
        /// <summary>
        /// 属性，当前状态
        /// </summary>
        public BaseState<Entity_Type> CurrentState
        {
            get
            {
                return currentState;
            }

            set
            {
                //currentState = value;
                currentState = value;
                currentState.entity = owner;
                currentState.Enter(owner);
            }
        }
        /// <summary>
        /// 属性，上一个状态
        /// </summary>
        public BaseState<Entity_Type> PreviousState
        {
            get
            {
                return previousState;
            }

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="owner"></param>
        public StateMachine(Entity_Type owner )
        {
            this.owner = owner;
            currentState = null;
            previousState = null;
            allStates = new Dictionary<string, BaseState<Entity_Type>>();
        }

        public StateMachine()
        {
        }

        /// <summary>
        /// 更新
        /// </summary>
        public void doTick()
        {
            if (defaultState!=null)
            {
                changeState(defaultState);//切换为默认状态
                defaultState = null;//只执行一次
            }
            if (currentState != null)
            {
                currentState.Execute(owner);//调用执行方法
            }
        }
        /// <summary>
        /// 状态转移
        /// </summary>
        /// <param name="newState">新状态</param>
        public void changeState(BaseState<Entity_Type> newState)
        {
            if (newState==null)
            {
                return;
            }
            
            if(currentState!=null)currentState.Exit(owner);//退出当前状态
            previousState = currentState;//保存上一个状态
            currentState = newState;//切换状态
            currentState.entity = owner;//传递实体
            currentState.Enter(owner);//进入新状态

        }
        /// <summary>
        /// 切换回上一个状态
        /// </summary>
        public void RevertToPreviousState()
        {
            changeState(previousState);
        }
        /// <summary>
        /// 添加状态
        /// </summary>
        /// <param name="name"></param>
        /// <param name="state"></param>
        public void AddState(string name,BaseState<Entity_Type> state)
        {
            allStates.Add(name, state);//添加到状态列表
            state.parent = this;//指定状态机
        }

        public void SetDefaultState(BaseState<Entity_Type> state)
        {
            defaultState = state;
        }
    }
}
