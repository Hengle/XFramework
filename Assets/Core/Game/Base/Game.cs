using UnityEngine;
using XDEDZL;
using XDEDZL.Pool;

/// <summary>
/// 这个类挂在初始场景中,是整个游戏的入口
/// </summary>
public class Game : MonoBehaviour
{
    public static ProcedureManager ProcedureModel;
    public static FsmManager FsmModel;
    public static ObjectPoolManager PoolModel;
    public static ResourceManager ResModel;
    public static GraphicsManager GraphicsModel;
    public static DataSubjectManager ObserverModel;

    void Awake()
    {
        InitModel();
    }

    void Update()
    {
        GameEntry.ModelUpdate(Time.deltaTime, Time.unscaledDeltaTime);
    }

    private void InitModel()
    {
        ProcedureModel = GameEntry.GetModule<ProcedureManager>();
        FsmModel = GameEntry.GetModule<FsmManager>();
        PoolModel = GameEntry.GetModule<ObjectPoolManager>();
        ResModel = GameEntry.GetModule<ResourceManager>();
        GraphicsModel = GameEntry.GetModule<GraphicsManager>();
        ObserverModel = GameEntry.GetModule<DataSubjectManager>();
    }
}