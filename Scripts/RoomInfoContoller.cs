using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RoomInfoContoller : MonoBehaviour {
    private Text RoomId;
	// Use this for initialization
	void Start () {
        RoomId = transform.GetChild(0).GetComponent<Text>();
	}
	public void BtnJoinRoom()
    {
        Login.isRoomMaster = false;
        Login.MyClient.SendMsg(Login.ownerIPName +
            "#joinRoom#" 
            + RoomId.text+"#"+Login.ownerNickName);
    }
	// Update is called once per frame
	void Update () {
	
	}
}
