//--------------------------------------------------------------------------------
/// <summary>
///  游戏的缓存池管理
/// </summary>
//--------------------------------------------------------------------------------
public class GCPoolManager : Singleton<GCPoolManager>
{
    #region 请使用属性
    private GCPublicPool publicsPool;
    private GCCharacterPool characterPool;
    private GCSkillPool skillPool;
    #endregion

    public GCPublicPool PublicPool
    {
        get
        {
            if (publicsPool == null)
            {
                publicsPool = new GCPublicPool();
                publicsPool.Init();
            }
            return publicsPool;
        }
    }

    public GCCharacterPool CharacterPool
    {
        get
        {
            if (characterPool == null)
            {
                characterPool = new GCCharacterPool();
                characterPool.Init();
            }
            return characterPool;
        }
    }

    public GCSkillPool SkillPool
    {
        get
        {
            if (skillPool == null)
            {
                skillPool = new GCSkillPool();
                skillPool.Init();
            }
            return skillPool;
        }
    }



    private bool bIsStaticPublicPool = false;         //推荐是动态池
    private bool bIsStaticCharacterPool = false;
    private bool bIsStaticSkillPool = false;


    public bool IsStataicPool(GCPoolType poolType)
    {
        switch (poolType)
        {
            case GCPoolType.PUBLICPOOL:
                return bIsStaticPublicPool;
            case GCPoolType.CHARACTERPOOL:
                return bIsStaticCharacterPool;
            case GCPoolType.SKILLPOOL:
                return bIsStaticSkillPool;
            default: return false;
        }
    }


    public void OnDispose()
    {
        publicsPool = null;
        characterPool = null;
        skillPool = null;
    }

}

public class PrefabNameDefinition
{
    public const string Prefab_DropOver = "DropOver";
    public const string Prefab_Character = "Character";
    public const string Prefab_SkillPrefab = "SkillPrefab";
    public const string Prefab_HUD = "HUD";
    public const string Prefab_ImageNormal = "ImageNormal"; //一张image
    private PrefabNameDefinition() { }
}