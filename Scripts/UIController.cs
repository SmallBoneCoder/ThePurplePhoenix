using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour {
    public GameObject Panel_Exit;
    #region Panel_Exit
    private void Btn_OK()
    {
        //Application.Quit();
        SceneManager.LoadScene(0);
    }
    private void Btn_Cancel()
    {
        Panel_Exit.SetActive(false);
    }
    #endregion
    // Use this for initialization
    void Start () {
        Panel_Exit.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(Btn_OK);
        Panel_Exit.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(Btn_Cancel);
    }
    private void OnGUI()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Panel_Exit.SetActive(true);
        }
    }
    // Update is called once per frame
    void Update () {
	
	}
}
