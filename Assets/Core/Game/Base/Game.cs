using UnityEngine;
using XDEDZL;

/// <summary>
/// 这个类挂在初始场景中,是整个游戏的入口
/// </summary>
public class Game : MonoBehaviour
{
    public static ProcedureManager ProcedureModel;
    public static FsmManager FsmModel;

    void Start()
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
    }
}