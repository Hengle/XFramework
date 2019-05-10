using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XDEDZL;

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