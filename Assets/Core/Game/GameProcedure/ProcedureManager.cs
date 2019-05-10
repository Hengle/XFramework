using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XDEDZL
{
    public class ProcedureManager : GameModule
    {
        /// <summary>
        /// 流程状态机
        /// </summary>
        private ProcedureFsm fsm;

        /// <summary>
        /// 开启一个流程
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        public void StartProcedure<TState>() where TState : ProcedureBase
        {
            fsm.StartFsm<TState>();
        }

        /// <summary>
        /// 流程切换
        /// </summary>
        /// <typeparam name="TProcedure"></typeparam>
        public void ChangeProcedure<TProcedure>() where TProcedure : ProcedureBase
        {
            fsm.ChangeState<TProcedure>();
        }

        internal override void Update(float elapseSeconds, float realElapseSeconds)
        {

        }

        internal override void Init()
        {

        }

        internal override void Shutdown()
        {

        }
    }
}