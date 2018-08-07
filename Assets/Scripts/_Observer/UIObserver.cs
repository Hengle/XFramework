///<summary>
///观察者接口
///<summary>
namespace XDEDZL
{
    public interface UIObserver
    {
        void OnDataChange(BaseData eventData, int type, object obj);
    }
}