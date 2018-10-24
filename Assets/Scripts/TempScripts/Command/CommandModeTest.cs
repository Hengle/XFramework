using UnityEngine;
using UnityEngine.UI;

public class CommandModeTest : MonoBehaviour
{
    public Camera MainCamera;
    public GameObject UIPanel;

    public InputField userName;
    public InputField passWord;
    public Player player;

    private CommandManager _commandManager;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        //定义一个命令收集者
        _commandManager = new CommandManager();

        //命令收集者：监听userName的值改变操作
        //userName.onEndEdit.AddListener((string value) =>
        //{
        //    _commandManager.ExecutiveCommand(new InputFieldCommand(userName, value, "修改用户名输入框的值为 " + value));
        //});

        ////命令收集者：监听passWord的值改变操作
        //passWord.onEndEdit.AddListener((string value) =>
        //{
        //    _commandManager.ExecutiveCommand(new InputFieldCommand(passWord, value, "修改密码输入框的值为 " + value));
        //});

        //命令收集者：监听player的移动操作
        player.MoveEvent += (Vector3 pos) =>
        {
            _commandManager.ExecutiveCommand(new PlayerCommand(player, pos, "角色移动到 " + pos));
        };
    }

    private void Update()
    {
        PlayerMove();
        RevocationCommand();
    }

    /// <summary>
    /// 点击地面移动角色
    /// </summary>
    private void PlayerMove()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = MainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit)/* && hit.transform.name == "Root"*/)
            {
                player.Move(hit.point);
            }
        }
    }

    /// <summary>
    /// 撤销操作
    /// </summary>
    private void RevocationCommand()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _commandManager.RevocationCommand();
        }
    }

    /// <summary>
    /// 进入场景
    /// </summary>
    public void Goin()
    {
        UIPanel.SetActive(false);
        player.transform.parent.gameObject.SetActive(true);
    }
}

