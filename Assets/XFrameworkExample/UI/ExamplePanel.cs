using XDEDZL.UI;

/*
 * 代码中需要用到的UI组件才需挂载GUxxx
 * Button,Slider一类的组件一般只需要注册函数，不用缓存
 */

public class ExamplePanel : BasePanel
{

    GUImage xxx;
    public override void Reg()
    {
        xxx = this["xxx"] as GUImage;

        (this["xxxBtn"] as GUButton).AddListener(() =>
        {

        });


        (this["xxxDp"] as GUDropdown).AddListener((value) =>
        {

        });
    }
}