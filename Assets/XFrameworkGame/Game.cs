using UnityEngine;
using XDEDZL;
using XDEDZL.Pool;

/// <summary>
/// 这个类挂在初始场景中,是整个游戏的入口
/// </summary>
public class Game : MonoBehaviour
{
    // 框架模块
    public static ProcedureManager ProcedureModule { get; private set; }
    public static FsmManager FsmModule { get; private set; }
    public static ObjectPoolManager PoolModule { get; private set; }
    public static ResourceManager ResModule { get; private set; }
    public static GraphicsManager GraphicsModule { get; private set; }
    public static DataSubjectManager ObserverModule { get; private set; }
    public static Messenger MessengerModule { get; private set; }


    // 业务模块
    public static UIHelper UIModule { get; private set; }

    void Awake()
    {
        InitModel();
        InitCustomModule();
        DontDestroyOnLoad(this);
    }

    void Update()
    {
        GameEntry.ModelUpdate(Time.deltaTime, Time.unscaledDeltaTime);
    }

    /// <summary>
    /// 初始化框架模块
    /// </summary>
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
    
    /// <summary>
    /// 初始化业务模块
    /// </summary>
    private void InitCustomModule()
    {
        UIModule = GameEntry.GetModule<UIHelper>();
    }
}