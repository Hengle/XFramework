//=====================================================================================/
///<summary>
///观察者接口
///<summary>
//=====================================================================================/
namespace JM
{
    public interface JMUIObserver
    {
        void OnDataChange(JMBaseData eventData, int type, object obj);
    }
}