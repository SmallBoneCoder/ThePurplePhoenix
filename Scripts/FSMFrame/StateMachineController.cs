
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace FSMFrame
{
    class StateMachineController
    {

        public static StateMachineController smc;

        public static StateMachineController GetInstance()
        {
            if (smc == null)
            {
                smc = new StateMachineController();
            }
            return smc;
        }

    public StateMachine<EnemyController> createEnemyFSM(EnemyController entity)
    {
        StateMachine<EnemyController> FSM = new StateMachine<EnemyController>(entity);
        FSM.AddState("巡逻", new State_Patrol());
        FSM.AddState("追击", new State_Pursue());
        FSM.SetDefaultState(FSM.allStates["巡逻"]);
        return FSM;
    }

}
}
