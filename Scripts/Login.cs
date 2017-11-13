using UnityEngine;
using System.Collections;
using TestClient;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
public enum PlayMode
{
    Single=1,
    Mutiplayer=2
}
public class Login : MonoBehaviour {
    public GameObject Panel_Begining;//开场画面
    public GameObject RoomInfo;//房间信息
    public GameObject PlayerInfo;//成员信息
    public GameObject Panel_Mode;//选择模式
    public GameObject Panel_RoomList;//房间列表界面
    private Transform content_RoomList;//房间列表
    private Transform content_Master;//成员列表-房主
    private Transform content_Player;//成员列表-成员
    public GameObject Panel_Loading;//载入画面
    private Slider slider_Progress;//加载进度条
    public GameObject Panel_Start;//开始界面
    public GameObject Panel_RoomList_Master;//房主界面
    public GameObject Panel_ConnectFail;//连接失败
    public GameObject Panel_RoomList_Player;//玩家界面
    public Transform LoadingImage;//环形进度条
    public InputField Text_IPAddr;//服务器ip地址文本
    public InputField Text_IPPort;//服务器端口号文本
    public InputField Text_NickName;//昵称文本
    public string IPAddr;//ip地址
    public string Port;//端口号
    public static Client MyClient;//全局客户端
    public static string ownerIPName;//全局本机外网ip
    public static string ownerNickName;//全局昵称
    private Client _client;//客户端引用
    private AsyncOperation asyncScene;//异步画面载入
    public static PlayMode CurrentMode = PlayMode.Single;//当前选择模式
    public static bool isRoomMaster = false;
    // Use this for initialization
    void Start () {
        LoadingImage.gameObject.SetActive(false);
        content_RoomList = Panel_RoomList.transform.GetChild(1).GetChild(0);
        content_Master = Panel_RoomList_Master.transform.GetChild(1).GetChild(0);
        content_Player = Panel_RoomList_Player.transform.GetChild(1).GetChild(0);
        slider_Progress = Panel_Loading.transform.GetChild(0).GetComponent<Slider>();
        MyClient = new Client(IPAddr, Port);
        _client = MyClient;
        Panel_ConnectFail.transform.GetChild(0).
            GetComponent<Button>().onClick.AddListener(
            ()=>
            {
                Panel_Start.transform.GetChild(0).GetComponent<Button>().interactable = true;
                Panel_ConnectFail.SetActive(false);
            });
        
	}
    private void OnEnable()
    {
        StartCoroutine(Begining());
    }
    #region Panel_Begining
    private IEnumerator Begining()
    {
        while (Panel_Begining.GetComponent<Image>().color.a > 0)
        {
            Panel_Begining.GetComponent<Image>().color -= 
                new Color(0, 0, 0, 0.01f);
            yield return 0;
        }
        Panel_Begining.SetActive(false);
    }
    #endregion
    #region Panel_Mode
    public void BtnSinglePlayer()
    {
        isRoomMaster = true;
        ownerIPName = IPAddr;
        CurrentMode = PlayMode.Single;
        Panel_Loading.SetActive(true);
        StartCoroutine(AsyncLoadingScene());
        StartCoroutine(SceneLoadingProgress());
    }
    public void BtnMutiPlayer()
    {
        CurrentMode = PlayMode.Mutiplayer;
        Panel_Mode.SetActive(false);
        Panel_Start.SetActive(true);
    }
    public void BtnQuitGame()
    {
        Application.Quit();
    }
    #endregion

     
    #region Panel_Start
    /// <summary>
    /// 连接服务器
    /// </summary>
    public void BtnConnect()
    {
        _client.InitClient(Text_IPAddr.text.Trim(), Text_IPPort.text.Trim());
        ownerNickName = Text_NickName.text.Trim();
        _client.StartConnect();
        StopAllCoroutines();
        StartCoroutine(Loading());
        Panel_Start.transform.GetChild(0).GetComponent<Button>().interactable = false;
    }
    public void BtnBackToMode()
    {
        Panel_Mode.SetActive(true);
        Panel_Start.SetActive(false);
        Panel_Start.transform.GetChild(0).GetComponent<Button>().interactable = true;
        StopAllCoroutines();
        _client.CloseClient();
    }
    /// <summary>
    /// 等待连接
    /// </summary>
    /// <returns></returns>
    float time = 0;
    private IEnumerator Loading()
    {
        time = 0;
        LoadingImage.gameObject.SetActive(true);
        while (isLoading)//加载中
        {
            //旋转进度条
            //time += Time.deltaTime;
            //if (time > 5f) break;
            if (_client.ConnectFail)
            {
                Panel_ConnectFail.SetActive(true);
                LoadingImage.gameObject.SetActive(false);
                _client.ConnectFail = false;
                yield break;
            }
            LoadingImage.RotateAround(LoadingImage.position, Vector3.forward, -5);
            yield return 0;
        }
        LoadingImage.gameObject.SetActive(false);
        Panel_Start.transform.GetChild(0).GetComponent<Button>().interactable = true;
        Panel_Start.SetActive(false);
        Panel_RoomList.SetActive(true);
    }
    #endregion
    #region Panel_RoomList
    public void BtnReFresh()
    {
        ClearChild(content_RoomList);
        _client.SendMsg(ownerIPName + "#refresh");
    }
    public void BtnBackToStart()
    {
        
        Panel_Start.SetActive(true);
        Panel_RoomList.SetActive(false);
    }
    public void BtnCreateRoom()
    {
        ClearChild(content_Master);
        _client.SendMsg(ownerIPName + "#createRoom");
    }
    
    #endregion
    #region Panel_RoomList_Master
    public void BtnDetachRoom()
    {
        _client.SendMsg(ownerIPName + "#detachRoom");
    }
    public void BtnStartGame()
    {
        _client.SendMsg(ownerIPName + "#startGame");
    }
    #endregion
    #region Panel_RoomList_Player
    public void BtnReady()
    {

    }
    public void BtnBackToRoomList()
    {
        //Panel_RoomList.SetActive(true);
        //Panel_RoomList_Player.SetActive(false);
        _client.SendMsg(ownerIPName + "#leaveRoom#"+ownerNickName);
    }
    #endregion
    string temp;
    bool isLoading = true;
    bool isStartGame = false;
    List<string> playerOfRoomIndex = new List<string>();
    // Update is called once per frame
    void Update()
    {
        if (_client.MessageBox.Count > 0)
        {

            lock (_client.MessageBox)
            {
                temp = _client.MessageBox.Dequeue();
            }
            //print(temp);
            string[] data = temp.Split('#');

            switch (data[1])
            {
                case "name":
                    {
                        ownerIPName = data[2];
                        _client.SendMsg(ownerIPName + "#login#" + ownerNickName);
                        isLoading = false;
                    }
                    break;
                case "createRoom":
                    {
                        isRoomMaster = true;
                        playerOfRoomIndex.Add(data[0]);//把自己添加进房间信息
                        Panel_RoomList_Master.transform.GetChild(0).GetChild(0).
                            GetChild(0).GetComponent<Text>().text = data[2]+"号房间";
                        GameObject player = Instantiate(PlayerInfo);
                        player.transform.GetChild(0).GetComponent<Text>().text = "房主";
                        player.transform.GetChild(1).GetComponent<Text>().text =ownerNickName;
                        player.transform.GetChild(3).GetComponent<Button>().interactable = false;
                        player.transform.SetParent(content_Master);
                        Panel_RoomList_Master.SetActive(true);
                        Panel_RoomList.SetActive(false);
                    }
                    break;
                case "refresh":
                    {
                        GameObject room = Instantiate(RoomInfo);
                        room.transform.GetChild(0).GetComponent<Text>().text =data[2];
                        room.transform.GetChild(1).GetComponent<Text>().text = data[3];
                        room.transform.GetChild(2).GetComponent<Text>().text = data[4];
                        if (data[5].Equals("True"))
                        {
                            room.transform.GetChild(3).GetComponent<Text>().text = "游戏中";
                            room.transform.GetChild(4).GetComponent<Button>().interactable = false;
                        }
                        else
                        {
                            room.transform.GetChild(3).GetComponent<Text>().text = "准备中";
                        }
                        room.transform.SetParent(content_RoomList);
                    }
                    break;
                case "startGame":
                    {
                        isStartGame = true;
                        Panel_RoomList_Master.SetActive(false);
                        Panel_RoomList_Player.SetActive(false);
                        Panel_Loading.SetActive(true);
                        StartCoroutine(AsyncLoadingScene());
                        StartCoroutine(SceneLoadingProgress());
                    }
                    break;
                case "joinRoom":
                    {
                        
                        //如果是自己
                        if (data[0] == ownerIPName)
                        {
                            //清除之前的列表
                            ClearChild(content_Player);
                            Panel_RoomList_Player.transform.GetChild(0).GetChild(0).
                            GetChild(0).GetComponent<Text>().text = data[2] + "号房间";
                            Panel_RoomList_Player.SetActive(true);
                            Panel_RoomList.SetActive(false);
                            _client.SendMsg(ownerIPName + "#roomMembers");
                        }
                        else//其他人加入房间
                        {
                            playerOfRoomIndex.Add(data[3]);
                            GameObject player = Instantiate(PlayerInfo);
                            player.transform.GetChild(0).GetComponent<Text>().text = "";
                            //名称
                            player.transform.GetChild(1).GetComponent<Text>().text = data[3];
                            player.transform.SetParent(content_Master);
                        }
                    }
                    break;
                case "roomMembers":
                    {
                        playerOfRoomIndex.Add(data[2]);//添加进房间列表
                        GameObject player = Instantiate(PlayerInfo);
                        //权限
                        player.transform.GetChild(0).GetComponent<Text>().text = data[3];
                        //名称
                        player.transform.GetChild(1).GetComponent<Text>().text = data[2];
                        player.transform.GetChild(3).GetComponent<Button>().interactable = false;
                        player.transform.SetParent(content_Player);
                    }
                    break;
                case "leaveRoom":
                    {
                        
                        //自己离开房间
                        if (data[0] == ownerIPName)
                        {
                            playerOfRoomIndex.Clear();//清除所有成员信息
                            ClearChild(content_RoomList);
                            Panel_RoomList.SetActive(true);
                            Panel_RoomList_Player.SetActive(false);
                        }
                        else//他人离开
                        {
                            //删除信息
                            Destroy(content_Master.GetChild(playerOfRoomIndex.IndexOf(data[2])).gameObject);
                            playerOfRoomIndex.Remove(data[2]);//清除单个
                        }
                       
                    }
                    break;
                case "detachRoom":
                    {
                        playerOfRoomIndex.Clear();//清除所有
                        ClearChild(content_RoomList);
                        Panel_RoomList.SetActive(true);
                        Panel_RoomList_Master.SetActive(false);
                        Panel_RoomList_Player.SetActive(false);
                    }
                    break;
                case "delMember":
                    {
                        if (data[0] == ownerIPName)
                        {
                            Destroy(content_Master.GetChild(playerOfRoomIndex.IndexOf(data[2])).gameObject);
                            playerOfRoomIndex.Remove(data[2]);
                        }
                        else
                        {
                            ClearChild(content_RoomList);
                            playerOfRoomIndex.Clear();//清除所有成员信息
                            Panel_RoomList.SetActive(true);
                            Panel_RoomList_Master.SetActive(false);
                            Panel_RoomList_Player.SetActive(false);
                        }
                    }
                    break;

            }
        }
    }
    private void ClearChild(Transform parent)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            Destroy(parent.GetChild(i).gameObject);
        }
    }
    private IEnumerator AsyncLoadingScene()
    {
        asyncScene = SceneManager.LoadSceneAsync(1);
        asyncScene.allowSceneActivation = false;
        yield return asyncScene;
    }
    private IEnumerator SceneLoadingProgress()
    {
        if (asyncScene == null) yield return 0;
        while (asyncScene.progress < 0.9f)
        {
            slider_Progress.value = asyncScene.progress;
            yield return 0;
        }
        slider_Progress.value = 1;
        asyncScene.allowSceneActivation = true;

    }
    /// <summary>
    /// 程序退出时
    /// </summary>
    private void OnDestroy()
    {
        print("Exit");
        if(!isStartGame) _client.CloseClient();
        //SendDestroyMessage(_ownerName);
        //
    }
}
