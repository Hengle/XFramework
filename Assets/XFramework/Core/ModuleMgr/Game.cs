using UnityEngine;
using XDEDZL;
using XDEDZL.Pool;

/// <summary>
/// 这个类挂在初始场景中,是整个游戏的入口
/// </summary>
public class Game : MonoBehaviour
{
    public static ProcedureManager ProcedureModule { get; private set; }
    public static FsmManager FsmModule { get; private set; }
    public static ObjectPoolManager PoolModule { get; private set; }
    public static ResourceManager ResModule { get; private set; }
    public static GraphicsManager GraphicsModule { get; private set; }
    public static DataSubjectManager ObserverModule { get; private set; }
    public static Messenger MessengerModule { get; private set; }

    void Awake()
    {
        InitModel();
        DontDestroyOnLoad(this);
    }

    void Update()
    {
        GameEntry.ModelUpdate(Time.deltaTime, Time.unscaledDeltaTime);
    }

    private void InitModel()
    {
        ProcedureModule = GameEntry.GetModule<ProcedureManager>();
        FsmModule = GameEntry.GetModule<FsmManager>();
        PoolModule = GameEntry.GetModule<ObjectPoolManager>();
        ResModule = GameEntry.GetModule<ResourceManager>();
        GraphicsModule = GameEntry.GetModule<GraphicsManager>();
        ObserverModule = GameEntry.GetModule<DataSubjectManager>();
        MessengerModule = GameEntry.GetModule<Messenger>();
    }
}