using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PartyMemberController : MonoBehaviour {
    private Text memberName;
    // Use this for initialization
    void Start () {
        memberName = transform.GetChild(1).GetComponent<Text>();
    }
    public void BtnDelMember()
    {
        Login.MyClient.SendMsg(Login.ownerIPName +
            "#delMember#"
            + memberName.text);
    }
    // Update is called once per frame
    void Update () {
	
	}
}
