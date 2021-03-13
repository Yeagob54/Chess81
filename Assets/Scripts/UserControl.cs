using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UserControl : MonoBehaviour
{

    private static UserProfile _userData = null;
    public static UserProfile userData
    {
        get
        {
            if (_userData == null)
                _userData = Utilities.FileManager.Load();

            return _userData;
        }
        set
        {
            _userData = value;
        }
    }

    [SerializeField]
    GameObject conectionPanel;

    [SerializeField]
    Button enterButton;

    [SerializeField]
    InputField inputName;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(Application.persistentDataPath);

        //Link Invitation controller
        if (Application.absoluteURL.Contains("invitation"))
        {
            NetworkManager.roomName = Application.absoluteURL.Split('=')[1];

            if (NetworkManager.roomName.Contains("&"))
                NetworkManager.roomName = Application.absoluteURL.Split('&')[0];

            Go();
        }
        else
        {
            UserExist();
        }

        enterButton.onClick.AddListener(SaveUser);

    }
    public void Go()
    {
        SceneManager.LoadScene(Scenes.INVITATIONMATCH.ToString());
    }

    private void UserExist()
    {
        if (userData.name == "Anonimous")//TODO: mover este valor a un archivo de Settings
        {
            conectionPanel.SetActive(true);
        }
        else//Ya existe un profile de usuario
        {
            SceneManager.LoadScene(Scenes.MAINMENU.ToString());
        }
    }

    private void SaveUser()
    {
        userData = new UserProfile(inputName.text);

		Save();

        SceneManager.LoadScene(Scenes.MAINMENU.ToString());

    }

	internal static void Save()
	{
		Utilities.FileManager.Save(userData);
	}
}
