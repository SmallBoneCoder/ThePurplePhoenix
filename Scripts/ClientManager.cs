using UnityEngine;
using System.Collections.Generic;
using TestClient;
using UnityEngine.UI;
using System;

public delegate void CMDHandle(string cmd);
public delegate void SyncHandle();
public delegate void UpdateHandle();
public class ClientManager : MonoBehaviour {
    public string IPAddr;//默认服务器IP地址
    public string Port;//默认服务器端口号
    public InputField InputText;//消息输入
    public Text Text_MessageBox;//消息显示
    private Client _client;//客户端
    public GameObject player;//玩家预制体
    public GameObject Joystick;//摇杆
    private GameObject _JoystickInstance;//摇杆实例（已无意义）
    public Dictionary<string,GameObject> AllInstanceObject;//所有的玩家对象
    public GameObject[] skills;//技能触发（施放）器
    private string _ownerName;//本机外网ip
    public UpdateHandle GameUpdate;//委托——更新方法
    public PatrolController[] patrolControlleres;//怪物生成点
    //delegate void Handle();
    //Handle handle;
    // Use this for initialization
    void Start () {
        AllInstanceObject = new Dictionary<string, GameObject>();
        //_client = new Client(IPAddr, Port);
        //_client.StartConnect();
        _client = Login.MyClient;
        _ownerName = Login.ownerIPName;
        _JoystickInstance = Joystick;
        //_JoystickInstance.transform.SetParent(GameObject.Find("Canvas").transform);
        //_JoystickInstance.transform.position = new Vector3(125f, 82f, 0);
        //注册委托
        _JoystickInstance.GetComponent<JoyStickEvent>().cmdHandle += SendMoveMessage;
        for (int i = 0; i < skills.Length; i++)
        {
            skills[i].GetComponent<SkillController>().cmdHandle += SendSkillMessage;
        }
        //_JoystickInstance.GetComponent<JoyStickEvent>().syncHandle += SendSynPosition;
        if (Login.CurrentMode == PlayMode.Mutiplayer)
        {
           
            _client.SendMsg(_ownerName + "#syncGame");
            GameUpdate = MutiplayerUpdate;
        }
        else
        {
            GameUpdate = SingleUpdate;
            CreatePlayer("127.0.0.1", "(0,0,0)", "龙傲天");
        }
        //注册生成怪物事件
        for(int i = 0; i < patrolControlleres.Length; i++)
        {
            patrolControlleres[i].SendMsgHandle = (msg) =>
            {
                _client.SendMsg(msg);
            };
        }
        //SendCreateMessage("player",_client.GetIPName());
        //_client.txtHandle = showText;
        //handle = _client.RecieveMsg;
        //InvokeRepeating("RecieveMsg", 0, 0.02f);
	}
	void RecieveMsg()
    {
        //_client.RecieveMsg();
    }
    // Update is called once per frame
    float time = 0;
    string temp;
    private void FixedUpdate()
    {
        //SendSynPosition();
    }
    private void SingleUpdate()
    {
        //print("single");
    }
    private void MutiplayerUpdate()
    {
        if (_client.MessageBox.Count > 0)
        {

            lock (_client.MessageBox)
            {
                temp = _client.MessageBox.Dequeue();
            }
            print(temp);
            string[] data = temp.Split('#');

            switch (data[1])
            {
                case "name":
                    {
                        _ownerName = data[2];
                    }
                    break;
                case "synPos":
                    {
                        SendSynPosition();
                    }
                    break;
                case "destroy":
                    {
                        DestroyEntity(data[2]);
                    }
                    break;
                case "player":
                    {

                        CreatePlayer(data[2], data[3], data[4]);
                    }
                    break;
                case "enemy":
                    {
                        patrolControlleres[int.Parse(data[2])].CreateEnemy();
                    }
                    break;
                case "boss":
                    {
                        patrolControlleres[int.Parse(data[2])].CreateBoss();
                    }
                    break;
                case "text":
                    {
                        Text_MessageBox.text += AllInstanceObject[data[0]].
                            GetComponent<PlayerController>().NickName + ":" + data[2] + "\r\n";
                    }
                    break;
                case "move":
                    {
                        if (AllInstanceObject.ContainsKey(data[0]))
                            AllInstanceObject[data[0]].
                            GetComponent<PlayerController>().SetCurrentDir(data[2], data[3]);
                    }
                    break;
                case "skill":
                    {
                        if (AllInstanceObject.ContainsKey(data[0]))
                            AllInstanceObject[data[0]].
                            GetComponent<PlayerController>().ExecuteSkill(data[2]);
                    }
                    break;
                case "BeDamaged":
                    {
                        if (AllInstanceObject.ContainsKey(data[0]))
                            AllInstanceObject[data[0]].
                            GetComponent<PlayerController>().CauseDamage(int.Parse(data[2]));
                    }
                    break;
            }
        }
    }

    void Update () {

        GameUpdate.Invoke();
    }
    #region SyncMethods
    private void SendSynPosition()
    {
        //_client.GetIPName()
        if (!AllInstanceObject.ContainsKey(_ownerName))
        {
            _client.SendMsg( _ownerName+ "#" +
            "synPos#(0,0,0)");
        }
        else
        {
            _client.SendMsg(_ownerName + "#" +
            "synPos#" + AllInstanceObject[_ownerName].transform.position.ToString("F6"));
        }
    }
    /// <summary>
    /// 发送销毁信息
    /// </summary>
    /// <param name="name"></param>
    private void SendDestroyMessage(string name)
    {
        _client.SendMsg(
            _ownerName + "#destroy#" + name);
    }
    /// <summary>
    /// 销毁实体
    /// </summary>
    /// <param name="name"></param>
    private void DestroyEntity(string name)
    {
        AllInstanceObject[name].SetActive(true);
        Destroy(AllInstanceObject[name]);
    }

    private void CreateEnemy(string name)
    {
        
    }

    public void SendTextMessage()
    {
        if (InputText.text == "") return;
        _client.SendMsg(
            _ownerName+"#"+"text#"+InputText.text);
        Text_MessageBox.text += 
            Login.ownerNickName+":"+ InputText.text+ "\r\n";
        InputText.text = "";

    }
    /// <summary>
    /// 创建玩家
    /// </summary>
    public void CreatePlayer(string name,string pos,string nickname)
    {
        if (AllInstanceObject.ContainsKey(name)) return;
        //玩家
        string[] v3 = pos.Substring(1, pos.Length - 2).Split(',');
        Vector3 _pos = new Vector3(
            float.Parse(v3[0]),
            float.Parse(v3[1]),
            float.Parse(v3[2]));
        //print(pos);

        GameObject _palyer = Instantiate(player);
        _palyer.transform.GetComponent<PlayerController>().IPName = name;
        _palyer.transform.GetComponent<PlayerController>().NickName = nickname;
        _palyer.transform.position = _pos;
        for (int i = 0; i < skills.Length; i++)
        {
            //print(skills[i]);
            //GameObject _skill = Instantiate(skills[i]);
            //skills[i].GetComponent<SkillController>().cmdHandle += SendSkillMessage;

            _palyer.GetComponent<PlayerController>().AddSkill(skills[i]);
        }
        //_palyer.tag = "Player";
        //如果这个玩家是自己
        if (name.Equals(_ownerName))
        {
            if (Login.CurrentMode == PlayMode.Mutiplayer)
            {
                _palyer.GetComponent<PlayerController>().syncHandle = SendSynPosition;
            }
            //_palyer.transform.tag = "Player";
            Camera.main.GetComponent<CameraFollow>().OwnerPlayer = _palyer;
            _palyer.GetComponent<PlayerController>().SendMsgHandle = (msg) =>
            {
                _client.SendMsg(msg);
            };
            //Camera.main.transform.SetParent(_palyer.transform);
            //Camera.main.transform.localPosition = new Vector3(0, 0, -10);
            
        }
        //_palyer.transform.tag = name;
        //添加进字典
        AllInstanceObject.Add(name, _palyer);
        print(Joystick);
    }
    /// <summary>
    /// 发送移动消息
    /// </summary>
    /// <param name="dir_v3"></param>
    public void SendMoveMessage(string dir_v3)
    {
        float temp = Time.time - time;
        if (temp >= 0.1f) temp = 0;
        if (!AllInstanceObject[_ownerName].GetComponent<PlayerController>().IsAlive) return;
        if (Login.CurrentMode == PlayMode.Mutiplayer)
        {
            _client.SendMsg(
            _ownerName + "#" + "move#" + dir_v3
            + "#" + (temp).ToString("F3"));
        }
        //print(temp);
        time = Time.time;
        
        AllInstanceObject[_ownerName].
            GetComponent<PlayerController>().SetCurrentDir(dir_v3, (temp).ToString("F3"));
        //SendSynPosition();

    }
    /// <summary>
    /// 发送创建信息
    /// </summary>
    /// <param name="entity_type"></param>
    public void SendCreateMessage(string type,string name)
    {
        _client.SendMsg(
            _ownerName + "#" + type+"#" + _ownerName);
    }
    /// <summary>
    /// 发送技能消息
    /// </summary>
    /// <param name="skillID"></param>
    public void SendSkillMessage(string skillID)
    {
        if (!AllInstanceObject[_ownerName].GetComponent<PlayerController>().IsAlive) return;
        if (Login.CurrentMode == PlayMode.Mutiplayer)
        {
            _client.SendMsg(
                _ownerName + "#" + "skill#" + skillID);
        }
        print(11);
        AllInstanceObject[_ownerName].
                GetComponent<PlayerController>().ExecuteSkill(skillID);
    }
    #endregion
    
    /// <summary>
    /// 程序退出时
    /// </summary>
    private void OnDestroy()
    {
        print("Exit");
        SendDestroyMessage(_ownerName);
        _client.CloseClient();
    }
}
