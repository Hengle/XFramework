using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;
using Debug = UnityEngine.Debug;

public class VideoPro : MonoBehaviour
{
    private string imagePath;
    private string ffmpegPath;
    private string videoPath;
    // Start is called before the first frame update
    void Start()
    {
        imagePath = @Application.streamingAssetsPath + "/Images/";
        ffmpegPath = @Application.streamingAssetsPath + "/ffmpeg/ffmpeg.exe";
        videoPath = @Application.streamingAssetsPath + "/test.mp4";

        if (!Directory.Exists(imagePath.TrimEnd('/')))
        {
            Directory.CreateDirectory(imagePath.TrimEnd('/'));
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            StartCoroutine(RecordVideo());
        }
    }

    private IEnumerator RecordVideo()
    {
        yield return 0;
        Debug.Log("Start");
        //建立外部调用进程
        Process p = new Process();
        p.StartInfo.FileName = "D:/迅雷下载" + "/[电影天堂www.dy2018.com]蚁人2：黄蜂女现身BD国英双语中英双字.mp4";
        string args = "-f image2 -i " + imagePath + "%d.jpg -vcodec libx264 -r 25 " + videoPath;
        //p.StartInfo.Arguments = args;
        //p.StartInfo.UseShellExecute = false;       //不使用操作系统外壳程序启动线程(一定为FALSE,详细的请看MSDN)
        //p.StartInfo.RedirectStandardError = true;  //把外部程序错误输出写到StandardError流中(这个一定要注意,FFMPEG的所有输出信息,都为错误输出流,用StandardOutput是捕获不到任何消息的...)
        //p.StartInfo.CreateNoWindow = false;         //不创建进程窗口
        //p.ErrorDataReceived += new DataReceivedEventHandler(Output);//外部程序(这里是FFMPEG)输出流时候产生的事件,这里是把流的处理过程转移到下面的方法中,详细请查阅MSDN
        p.Start();                                 //启动线程
        //p.BeginErrorReadLine();                    //开始异步读取
        //p.WaitForExit();                           //阻塞等待进程结束
        //p.Close();                                 //关闭进程
        //p.Dispose();                               //释放资源

        DirectoryInfo dir = new DirectoryInfo(imagePath);
        dir.Delete(true);
        Directory.CreateDirectory(imagePath.TrimEnd('/'));
    }
}
