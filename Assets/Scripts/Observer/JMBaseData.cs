//=====================================================================================/
///<summary>
///���������,ֻ��һ��
///<summary>
//=====================================================================================/
namespace JM
{
    using System;
    using System.Collections.Generic;
    public enum JMDataType
    {
        /// <summary>
        /// ����
        /// </summary>
        TASK_LIST = 1,
        /// <summary>
        /// �����б�
        /// </summary>
        ITEM = 2,

        EVERYDAYTASK_LIST = 3,
        RANKING_LIST = 5,
        PUB = 6,
        ROLE = 7,
        ACTIVITY = 8,
        /// <summary>
        /// ������
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
        /// ��ɫ��Ϣ
        /// </summary>
        PLAYER = 19,
        /// <summary>
        /// ��װ
        /// </summary>
        CHANGE_SKINS = 20,
        /// <summary>
        /// ��¼����
        /// </summary>
        LOGIN_RESULT = 21,
        /// <summary>
        /// ����
        /// </summary>
        UPDATE_MONEY = 22,
        /// <summary>
        /// �л�����
        /// </summary>
        HANGE_SCENE = 23,
        /// <summary>
        /// �ʼ�ϵͳ
        /// </summary>
        MAIL = 24,
        /// <summary>
        /// GM����
        /// </summary>
        GM_COMMAND = 25,
        /// <summary>
        /// ��ļ(�鿨)
        /// </summary>
        DRAWCARD = 26,
        /// <summary>
        /// ���
        /// </summary>
        SERVANT = 27,
        /// <summary>
        /// ͼ��
        /// </summary>
        MANUAL = 28,
        /// <summary>
        /// ��Ƭ�̵�(�һ�)
        /// </summary>
        PIECESTORE = 29,
        /// <summary>
        /// ����
        /// </summary>
        ADVENTURE = 30,
        /// <summary>
        /// �½�
        /// </summary>
        CHAPTER = 31,
        /// <summary>
        /// �ؿ�
        /// </summary>
        LEVEL = 32,
        /// <summary>
        /// ����
        /// </summary>   
        CHAT = 33,
        /// <summary>
        /// ���а�
        /// </summary>
        RANKING = 34,
        /// <summary>
        /// ����
        /// </summary>   
        SETTING = 35,
        /// <summary>
        /// �û�Э��
        /// </summary>   
        USERAGREEMENGT = 36,
        /// <summary>
        /// ��˽����
        /// </summary>        
        PRIVACYPOLICY = 37,
        /// <summary>
        /// �̳�
        /// </summary>
        SHOP = 38,
        /// <summary>
        /// �̳�:��������
        /// </summary>
        BUYNUM = 39,
        /// <summary>
        /// VIP
        /// </summary>
        VIP = 40,
        /// <summary>
        /// ����
        /// </summary>
        BIOGRAPHY = 41,
        /// <summary>
        /// �ƺ�
        /// </summary>
        TITLE = 42,
        ///<summary>
        ///�ճ�
        ///</summary>>
        SCHEDULE=43,
        /// <summary>
        /// ��ֵ
        /// </summary>
        PAY = 45,
        /// <summary>
        /// �ɾ�
        /// </summary>
        ACHIEVEMENT = 46,

        ///<summary>
        ///��ϵ
        ///</summary>
        FACTIONS = 47,

        /// <summary>
        /// ˽��
        /// </summary>
        PRIVATECHAT =49,
        /// <summary>
        /// NPC����
        /// </summary>
        NPC_INTERACTION = 50,
        /// <summary>
        /// ��԰
        /// </summary>
        GARDEN = 51,
        /// <summary>
        /// ����
        /// </summary>
        HAREMFIGHT = 52,
        /// <summary>
        /// ����
        /// </summary>
        CREATEORSELECT = 53,
        /// <summary>
        /// ���º��ѻ�԰
        /// </summary>
        FRIENDGARDEN = 54,
        /// <summary>
        /// �һ�
        /// </summary>
        CONVERSION=55,
        /// <summary>
        /// �廨
        /// </summary>
        FLOWERARRANGEMENT = 56,

        /// <summary>
        /// �ʻ�����
        /// </summary>
        FLOWERPRESENT = 57,

        /// <summary>
        /// ��Ƭ
        /// </summary>
        PERSONINFO=58,

        /// <summary>
        /// ͨ��RoleDataɢֵ
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
                throw new ArgumentException(string.Format("����ת��ʧ��:Դ����: {0} Ŀ������:{1}", data.GetType(), typeof(T)));
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