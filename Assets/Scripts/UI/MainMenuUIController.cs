using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System.Reflection;
using System;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class MainMenuUIController : MonoBehaviour
{
    const string URL = "http://chess81.x10host.com";

    [SerializeField]
    InputField invitationLink;
    [SerializeField]
    GameObject invitationPanel;
	
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Absolute URL: " + Application.absoluteURL);

        URLParameters.Instance.RegisterOnDone((url) => {
            Debug.Log(" Pathname: " + url.Pathname);
            Debug.Log(" Hostname: " + url.Hostname);
            Debug.Log(" Hash: " + url.Hash);
            Debug.Log(" SearchParameters: " + url.SearchParameters);
        });

        invitationPanel.gameObject.SetActive(false);

    }

    public void Go()
    {

        SceneManager.LoadScene(Scenes.INVITATIONMATCH.ToString());
    }

    public void MatchMaking()
    {
		SceneManager.LoadScene(Scenes.MATCHMAKING.ToString());
    }

    public void VsBot()
    {
        SceneManager.LoadScene(Scenes.INGAME.ToString());
    }
    public void ShowInvitationLink()
    {
        //Initializing Invitation Link
        string link ="";
        NetworkManager.roomName = UserControl.userData.name + Mathf.Round(Time.timeSinceLevelLoad * Random.Range(-2000f, -1000f));
        URLParameters.Instance.RegisterOnDone((url) =>
        {
           link = url.Hostname + "?invitation=" + NetworkManager.roomName;
        });

        //Show URL on Screen
        invitationPanel.gameObject.SetActive(true);
        invitationLink.text = link;

        //Show URL on Notepad, not work on webgl!
        //string filePath = Application.persistentDataPath + "/Chess81_Invitation_Link.txt";
        //Utilities.FileManager.SaveString(link, filePath);
        //Application.ExternalEval("window.open(\""+ filePath + "\")");
        //Application.OpenURL(filePath);
    }


}
