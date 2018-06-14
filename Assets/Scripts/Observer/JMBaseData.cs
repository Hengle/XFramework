//=====================================================================================/
///<summary>
///数据类基类,只有一份
///<summary>
//=====================================================================================/
namespace JM
{
    using System;
    using System.Collections.Generic;
    public enum JMDataType
    {
        /// <summary>
        /// 任务
        /// </summary>
        TASK_LIST = 1,
        /// <summary>
        /// 背包列表
        /// </summary>
        ITEM = 2,

        EVERYDAYTASK_LIST = 3,
        RANKING_LIST = 5,
        PUB = 6,
        ROLE = 7,
        ACTIVITY = 8,
        /// <summary>
        /// 竞技场
        /// </summary>
        ARENA = 9,
        BATTLE = 10,
        ENTRUST = 11,
        RECHARGEABLE = 12,
        FIRSTACTIVITY = 13,
        FRIEND = 14,
        FUND = 15,
        SOCIAL = 16,
        TRIAL = 17,
        MATERIAL = 18,
        /// <summary>
        /// 角色信息
        /// </summary>
        PLAYER = 19,
        /// <summary>
        /// 换装
        /// </summary>
        CHANGE_SKINS = 20,
        /// <summary>
        /// 登录返回
        /// </summary>
        LOGIN_RESULT = 21,
        /// <summary>
        /// 货币
        /// </summary>
        UPDATE_MONEY = 22,
        /// <summary>
        /// 切换场景
        /// </summary>
        HANGE_SCENE = 23,
        /// <summary>
        /// 邮件系统
        /// </summary>
        MAIL = 24,
        /// <summary>
        /// GM命令
        /// </summary>
        GM_COMMAND = 25,
        /// <summary>
        /// 招募(抽卡)
        /// </summary>
        DRAWCARD = 26,
        /// <summary>
        /// 随从
        /// </summary>
        SERVANT = 27,
        /// <summary>
        /// 图鉴
        /// </summary>
        MANUAL = 28,
        /// <summary>
        /// 碎片商店(兑换)
        /// </summary>
        PIECESTORE = 29,
        /// <summary>
        /// 奇遇
        /// </summary>
        ADVENTURE = 30,
        /// <summary>
        /// 章节
        /// </summary>
        CHAPTER = 31,
        /// <summary>
        /// 关卡
        /// </summary>
        LEVEL = 32,
        /// <summary>
        /// 聊天
        /// </summary>   
        CHAT = 33,
        /// <summary>
        /// 排行榜
        /// </summary>
        RANKING = 34,
        /// <summary>
        /// 设置
        /// </summary>   
        SETTING = 35,
        /// <summary>
        /// 用户协议
        /// </summary>   
        USERAGREEMENGT = 36,
        /// <summary>
        /// 隐私条款
        /// </summary>        
        PRIVACYPOLICY = 37,
        /// <summary>
        /// 商城
        /// </summary>
        SHOP = 38,
        /// <summary>
        /// 商城:购买数量
        /// </summary>
        BUYNUM = 39,
        /// <summary>
        /// VIP
        /// </summary>
        VIP = 40,
        /// <summary>
        /// 传记
        /// </summary>
        BIOGRAPHY = 41,
        /// <summary>
        /// 称号
        /// </summary>
        TITLE = 42,
        ///<summary>
        ///日程
        ///</summary>>
        SCHEDULE=43,
        /// <summary>
        /// 充值
        /// </summary>
        PAY = 45,
        /// <summary>
        /// 成就
        /// </summary>
        ACHIEVEMENT = 46,

        ///<summary>
        ///派系
        ///</summary>
        FACTIONS = 47,

        /// <summary>
        /// 私聊
        /// </summary>
        PRIVATECHAT =49,
        /// <summary>
        /// NPC互动
        /// </summary>
        NPC_INTERACTION = 50,
        /// <summary>
        /// 花园
        /// </summary>
        GARDEN = 51,
        /// <summary>
        /// 宫斗
        /// </summary>
        HAREMFIGHT = 52,
        /// <summary>
        /// 创角
        /// </summary>
        CREATEORSELECT = 53,
        /// <summary>
        /// 更新好友花园
        /// </summary>
        FRIENDGARDEN = 54,
        /// <summary>
        /// 兑换
        /// </summary>
        CONVERSION=55,
        /// <summary>
        /// 插花
        /// </summary>
        FLOWERARRANGEMENT = 56,

        /// <summary>
        /// 鲜花赠送
        /// </summary>
        FLOWERPRESENT = 57,

        /// <summary>
        /// 名片
        /// </summary>
        PERSONINFO=58,

        /// <summary>
        /// 通用RoleData散值
        /// </summary>
        COMMONROLEDATAHASHVALUE = 60,
    }



    public abstract class JMBaseData
    {
        private static Dictionary<Type, JMBaseData> s_uiDataByInstanceTypeDic = new Dictionary<Type, JMBaseData>();

        protected JMBaseData()
        { }

        public static T DataConvert<T>(JMBaseData data) where T : class
        {
            T tDate = data as T;
            if (tDate == null)
                throw new ArgumentException(string.Format("类型转换失败:源类型: {0} 目标类型:{1}", data.GetType(), typeof(T)));
            return tDate;
        }

        private static bool ContainsKey(Type type)
        {
            return s_uiDataByInstanceTypeDic.ContainsKey(type);
        }

        private static void AddData(JMBaseData data)
        {
            s_uiDataByInstanceTypeDic[data.GetType()] = data;
        }

        public static T GetData<T>() where T : JMBaseData, new()
        {
            T data = null;
            Type type = typeof(T);
            if (ContainsKey(type))
            {
                data = s_uiDataByInstanceTypeDic[type] as T;
            }
            else
            {
                data = new T();
                s_uiDataByInstanceTypeDic[type] = data;
            }
            return data;
        }

        public abstract JMDataType dataType
        {
            get;
        }
    }
}