using UnityEngine;
using XDEDZL;
using XDEDZL.Pool;

/// <summary>
/// 这个类挂在初始场景中,是整个游戏的入口
/// </summary>
public class Game : MonoBehaviour
{
    public static ProcedureManager ProcedureModel { get; private set; }
    public static FsmManager FsmModel { get; private set; }
    public static ObjectPoolManager PoolModel { get; private set; }
    public static ResourceManager ResModel { get; private set; }
    public static GraphicsManager GraphicsModel { get; private set; }
    public static DataSubjectManager ObserverModel { get; private set; }
    public static Messenger MessengerModel { get; private set; }

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
        MessengerModel = GameEntry.GetModule<Messenger>();
    }
}