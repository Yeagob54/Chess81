using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    #region Atributtes
    /// <summary>
    /// In Game UI Controller
    /// </summary>
    public UIGameController uiController;

    /// <summary>
    /// Network Manager reference
    /// </summary>
    public NetworkManager networkManager;

	/// <summary>
	/// Chess Board Script reference
	/// </summary>
	public cgChessBoardScript boardController;
    
	/// <summary>
    /// Mini Singleton
    /// </summary>
    public static GameController instance;

    #endregion

    #region Unity Callbacks
    private void Awake()
    {
        instance = this;

        Application.runInBackground = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    #endregion

    #region Methods
    internal void VolverAlMenu()
    {
        if (boardController.Mode != BoardMode.PlayerVsEngine)
        {
            networkManager.LeaveRoom();
            PhotonNetwork.Disconnect();
            boardController.Mode = BoardMode.PlayerVsEngine;
        }
        SceneManager.LoadScene(Scenes.MAINMENU.ToString());
    }

    internal void RecargarPartida()
    {
        SceneManager.LoadScene(Scenes.INGAME.ToString());
    }

    internal void MatchLosed(string razon)
    {
        GameController.instance.uiController.popupPanel.ShowModalMode("You have Lost this match by "+razon+".", uiController.EndGameMode);

        //Send RPC to other... 
        if (boardController.Mode == BoardMode.OnlineWhite)
            networkManager.photonView.RPC("SendEndGame", networkManager.rpcTarget, (int)EndGameStates.BLACK_WIN, razon);
        else
            networkManager.photonView.RPC("SendEndGame", networkManager.rpcTarget, (int)EndGameStates.WHITE_WIN, razon);

        AddLose();
    }

    private void AddLose()
    {
        if (boardController.Mode == BoardMode.OnlineBlack)
            UserControl.userData.blackLose++;
        else
            UserControl.userData.whiteLose++;

        UserControl.Save();
    }

    public void AddMatchPlayed()
    {
        if (boardController.Mode == BoardMode.OnlineBlack)
            UserControl.userData.blackPlayed++;
        else
            UserControl.userData.whitePlayed++;

        UserControl.Save();
    }

    public void AddMatchWin(string razon)
    {
        if (boardController.Mode == BoardMode.OnlineBlack)
            UserControl.userData.blackWins++;
        else
            UserControl.userData.whiteWins++;

        UserControl.Save();

        uiController.popupPanel.ShowModalMode("You Win this match by "+razon+"!!!", uiController.EndGameMode);
    }

    public void MatchDraw()
    {
        if (boardController.Mode == BoardMode.OnlineBlack)
            UserControl.userData.blackDraw++;
        else
            UserControl.userData.whiteDraw++;

        UserControl.Save();

        uiController.popupPanel.ShowModalMode("This match ended in a Draw.", uiController.EndGameMode);
    }

    public void FlipBoard()
    {
        boardController.FlipBoard();
        uiController.upClock.colorClock = ClockType.WHITE;
        uiController.downClock.colorClock = ClockType.BLACK;
    }
    #endregion

}
