using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XFramework
{
    /// <summary>
    /// 流程的优先级应比状态机低
    /// </summary>
    public class ProcedureManager : IGameModule
    {
        /// <summary>
        /// 流程状态机
        /// </summary>
        private ProcedureFsm m_Fsm;

        public ProcedureManager()
        {
            m_Fsm = GameEntry.GetModule<FsmManager>().GetFsm<ProcedureFsm>();
        }

        /// <summary>
        /// 开启一个流程
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        public void StartProcedure<TState>() where TState : ProcedureBase
        {
            m_Fsm.StartFsm<TState>();
        }

        /// <summary>
        /// 流程切换
        /// </summary>
        /// <typeparam name="TProcedure"></typeparam>
        public void ChangeProcedure<TProcedure>() where TProcedure : ProcedureBase
        {
            m_Fsm.ChangeState<TProcedure>();
        }

        public int Priority { get { return 1; } }

        public void Update(float elapseSeconds, float realElapseSeconds)
        {

        }

        public void Init()
        {

        }

        public void Shutdown()
        {

        }
    }
}