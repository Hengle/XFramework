///<summary>
///观察者接口
///<summary>
namespace XDEDZL
{
    public interface Observer
    {
        void OnDataChange(BaseData eventData, int type, object obj);
    }
}