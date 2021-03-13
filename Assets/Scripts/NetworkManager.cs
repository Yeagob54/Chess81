using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;
using System;

[RequireComponent(typeof(PhotonView))]
public class NetworkManager : MonoBehaviourPunCallbacks
{
    #region Attributes
    /// <summary>
    /// Static instance of NetworkManager
    /// </summary>
    public static NetworkManager instance;

	/// <summary>
	/// Static room name for invitation mode
	/// </summary>
	public static string roomName = "";

	public RpcTarget rpcTarget;

	[SerializeField]
	GameObject panelWaitingForPlayer;
	[SerializeField]
	Button cancelRoom;

	[SerializeField]
	cgChessBoardScript boardController;

	private PhotonView photonView;
	
    #endregion

    #region Unity Callbacks
    /// <summary>
    /// Simples singleton.
    /// </summary>
    private void Awake()
	{
		instance = this;
		photonView = GetComponent<PhotonView>();
		cancelRoom.onClick.AddListener(GameController.instance.VolverAlMenu);

	}

	/// <summary>
	/// MonoBehaviour method called on GameObject by Unity during initialization phase.
	/// </summary>
	void Start()
	{

        if (PhotonNetwork.IsConnected)
            boardController.Mode = BoardMode.Online;
        else
#if UNITY_EDITOR
            if (FindObjectsOfType<ClockController>().Length == 2)
        {
            FindObjectsOfType<ClockController>()[0].enabled = false;
            FindObjectsOfType<ClockController>()[1].enabled = false;

        }
#else
            boardController.Mode = BoardMode.PlayerVsEngine;
#endif


        // in case we started this demo with the wrong scene being active, simply load the menu scene
        if (!PhotonNetwork.IsConnected && boardController.Mode == BoardMode.Online)
		{
			UnityEngine.SceneManagement.SceneManager.LoadScene(Scenes.MATCHMAKING.ToString());

			return;
		}

        if (PhotonNetwork.IsConnected)
        {
		    if (PhotonNetwork.IsMasterClient)
		    {
			    boardController.Mode = BoardMode.OnlineWhite;

            }
            else
		    {
			    boardController.Mode = BoardMode.OnlineBlack;
                GameController.instance.FlipBoard();
            }
            panelWaitingForPlayer.SetActive(true);
        }

		//START GAME!!
		if (PhotonNetwork.PlayerList.Length == 2)
		{
            photonView.RPC("SendName", NetworkManager.instance.rpcTarget, UserControl.userData.name);

            panelWaitingForPlayer.SetActive(false);

            GameController.instance.AddMatchPlayed();
		}
	}

	/// <summary>
	/// MonoBehaviour method called on GameObject by Unity on every frame.
	/// </summary>
	void Update()
	{

		//// "back" button of phone equals "Escape". quit app if that's pressed
		//if (Input.GetKeyDown(KeyCode.Escape))
		//{
		//    QuitApplication();
		//}
	}
#endregion

#region Photon Callbacks

    /// <summary>
    /// Called when a Photon Player got connected. We need to then load a bigger scene.
    /// </summary>
    /// <param name="other">Other.</param>
    public override void OnPlayerEnteredRoom(Player other)
	{
		Debug.Log("OnPlayerEnteredRoom() " + other.NickName); // not seen if you're the player connecting

		if (PhotonNetwork.IsMasterClient)
		{
			Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom

		}

		//START GAME!!
		if (PhotonNetwork.PlayerList.Length == 2)
		{
			panelWaitingForPlayer.SetActive(false);
            GameController.instance.AddMatchPlayed();
        }
    }

	/// <summary>
	/// Called when a Photon Player got disconnected. We need to load a smaller scene.
	/// </summary>
	/// <param name="other">Other.</param>
	public override void OnPlayerLeftRoom(Player other)
	{
		Debug.Log("OnPlayerLeftRoom() " + other.NickName); // seen when other disconnects

		if (PhotonNetwork.IsMasterClient)
		{
			Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom
		}
	}

	/// <summary>
	/// Called when the local player left the room. We need to load the launcher scene.
	/// </summary>
	public override void OnLeftRoom()
	{
		UnityEngine.SceneManagement.SceneManager.LoadScene(Scenes.MAINMENU.ToString());
	}


#endregion

#region Methods

	public void LeaveRoom()
	{
		PhotonNetwork.LeaveRoom();
	}


	public void QuitApplication()
	{
		Application.Quit();
	}

    private void DrawReject()
    {
        GameController.instance.uiController.popupPanel.ShowModalMode("Your opponent has rejected your draw offer.");
    }

    private void DrawAcepted()
    {
        photonView.RPC("DrawAceptedRPC", NetworkManager.instance.rpcTarget);
        GameController.instance.MatchDraw();
    }

#endregion

#region RPC Methods

    [PunRPC]
	public void SendMove(byte from, byte to, byte passantMove, bool queened)
	{
        if (passantMove != 0)
        {
            cgEnPassantMove movement = new cgEnPassantMove(from, to, 0, passantMove);
            movement.queened = queened;
		    boardController.OnlineMovement(movement);
        }
        else
        {
		    cgSimpleMove movement = new cgSimpleMove(from, to);
            movement.queened = queened;
            boardController.OnlineMovement(movement);
        }
	}

    [PunRPC]
    public void SendCastlingMove(byte from, byte to, byte secondFrom, byte secondTo)
    {
            cgCastlingMove movement = new cgCastlingMove(from, to, 0, secondFrom, secondTo);
            boardController.OnlineMovement(movement);
    }

    [PunRPC]
	public void SendName(string oponentName)
	{
        GameController.instance.uiController.SetOtherPlayerName(oponentName);
        photonView.RPC("SendNameBack", rpcTarget, UserControl.userData.name);
    }

    [PunRPC]
    public void SendNameBack(string oponentName)
    {
        GameController.instance.uiController.SetOtherPlayerName(oponentName);
    }

    [PunRPC]
    public void SendEndGame(int gameState, string razon)
    {
        switch ((EndGameStates)gameState)
        {
            case EndGameStates.BLACK_WIN:
                if (boardController.Mode == BoardMode.OnlineBlack)
                    GameController.instance.AddMatchWin(razon);
                break;
            case EndGameStates.WHITE_WIN:
                if (boardController.Mode == BoardMode.OnlineWhite)
                    GameController.instance.AddMatchWin(razon);
                break;
            case EndGameStates.DRAW:
                GameController.instance.MatchDraw();
                break;
        }
    }

    [PunRPC]
    public void DrawPropositon()
    {
        GameController.instance.uiController.popupPanel.ShowYesNoMode("Your opponent offers you Draw.\nYou want to accept?", DrawAcepted, DrawReject); ;
    }

    [PunRPC]
    public void DrawAceptedRPC()
    {
        GameController.instance.MatchDraw();
    }

#endregion
}
