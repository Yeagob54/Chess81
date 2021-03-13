// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Launcher.cs" company="Exit Games GmbH">
//   Part of: Photon Unity Networking Demos
// </copyright>
// <summary>
//  Used in "PUN Basic tutorial" to connect, and join/create room automatically
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using Photon.Realtime;
using System;
using System.Collections.Generic;

namespace Photon.Pun.Demo.PunBasics
{
#pragma warning disable 649


	/// <summary>
	/// Launch manager. Connect, join a random room or create one if none or all full.
	/// </summary>
	public class Launcher : MonoBehaviourPunCallbacks
    {

		List<RoomInfo> roomList = new List<RoomInfo>();

        #region Private Serializable Fields

        [Tooltip("If we arrived here from an invitation option")]
        [SerializeField]
        bool invitationMode;


        [Tooltip("The Ui Panel to let the user enter name, connect and play")]
		[SerializeField]
		private GameObject controlPanel;

		[Tooltip("The Ui Text to inform the user about the connection progress")]
		[SerializeField]
		private Text feedbackText;

		[Tooltip("The maximum number of players per room")]
		[SerializeField]
		private byte maxPlayersPerRoom = 2;

		[Tooltip("The UI Loader Anime")]
		[SerializeField]
		private LoaderAnime loaderAnime;
		int i = 0;
		string newRoomName { get { return (i++).ToString(); } }

		#endregion

		#region Private Fields
		/// <summary>
		/// Keep track of the current process. Since connection is asynchronous and is based on several callbacks from Photon, 
		/// we need to keep track of this to properly adjust the behavior when we receive call back by Photon.
		/// Typically this is used for the OnConnectedToMaster() callback.
		/// </summary>
		bool isConnecting;

		/// <summary>
		/// This client's version number. Users are separated from each other by gameVersion (which allows you to make breaking changes).
		/// </summary>
		string gameVersion = "1";

		#endregion

		#region MonoBehaviour CallBacks

		/// <summary>
		/// MonoBehaviour method called on GameObject by Unity during early initialization phase.
		/// </summary>
		void Awake()
		{

            if (loaderAnime==null)
			{
				Debug.LogError("<Color=Red><b>Missing</b></Color> loaderAnime Reference.",this);
			}

			// #Critical
			// this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
			PhotonNetwork.AutomaticallySyncScene = true;

			// Start COnection if we are in Scene Invitation
			ConnectToRoom();

        }

        #endregion

        #region Private Methods
        private void ConnectToRoom()
        {
            // we want to make sure the log is clear everytime we connect, we might have several failed attempted if connection failed.
            feedbackText.text = "";

            // keep track of the will to join a room, because when we come back from the game we will get a callback that we are connected, so we need to know what to do then
            isConnecting = true;

            // hide the Play button for visual consistency
            controlPanel.SetActive(false);

            // start the loader animation for visual effect.
            if (loaderAnime != null)
            {
                loaderAnime.StartLoaderAnimation();
            }

            // we check if we are connected or not, we join if we are , else we initiate the connection to the server.
            //if (PhotonNetwork.IsConnected)
            //{
            //    DoConnect();
            //}
            //else
            //{

                LogFeedback("Connecting...");

                // #Critical, we must first and foremost connect to Photon Online Server.
                PhotonNetwork.GameVersion = this.gameVersion;
                PhotonNetwork.ConnectUsingSettings();
            //}
        }

        private void DoConnect()
        {
            if (invitationMode)
            {
                LogFeedback("Joining Invitation Room...");
                RoomOptions room = new RoomOptions { MaxPlayers = 2, EmptyRoomTtl = 0 };
                room.IsVisible = false;
                PhotonNetwork.JoinOrCreateRoom(NetworkManager.roomName, room, TypedLobby.Default);
            }
            else
            {
                // #Critical: The first we try to do is to join a potential existing room. If there is, good, else, we'll be called back with OnJoinRandomFailed()
				foreach(RoomInfo room in roomList)
				{
					if(room.PlayerCount < 2 && !room.Name.Contains("-")) //If not full and is not an invitation room.
					{

						LogFeedback("Trying to joining Room: " + room.Name);
						PhotonNetwork.JoinRoom(room.Name);
						return;
					}
				}
				string roomName = newRoomName;
				LogFeedback("Joining Room: " + roomName);
				// #Critical we need at this point to attempt joining a Random Room. If it fails, we'll get notified in OnJoinRandomFailed() and we'll create one.
				PhotonNetwork.JoinOrCreateRoom(roomName, new RoomOptions { MaxPlayers = 2, EmptyRoomTtl = 0 }, TypedLobby.Default);

            }
        }

		#endregion

		#region Public Methods

		public override void OnJoinedLobby()
		{
			base.OnJoinedLobby();
			Debug.Log("Lobby In!");
		}

		public override void OnRoomListUpdate(List<RoomInfo> p_roomList)
		{
			//base.OnRoomListUpdate(p_roomList);
			Debug.LogError("Room list updated, total: " + p_roomList.Count);

			roomList = p_roomList;
		}

		public void Cancel()
        {
            PhotonNetwork.LeaveRoom();
            SceneManager.LoadScene("MainMenu");
        }

		public override void OnJoinRoomFailed(short returnCode, string message)
		{
			Debug.LogError("Join failed, " + message);

			string room = newRoomName;
			LogFeedback("Creating " + room + " Room...");
			// #Critical: The first we try to do is to join a potential existing room. If there is, good, else, we'll be called back with OnJoinRandomFailed()
			PhotonNetwork.JoinOrCreateRoom(room, new RoomOptions { MaxPlayers = 2, EmptyRoomTtl = 0 }, TypedLobby.Default);
		}


		/// <summary>
		/// Start the connection process. 
		/// - If already connected, we attempt joining a random room
		/// - if not yet connected, Connect this application instance to Photon Cloud Network
		/// </summary>
		public void Connect()
		{
			// we want to make sure the log is clear everytime we connect, we might have several failed attempted if connection failed.
			feedbackText.text = "";

			// keep track of the will to join a room, because when we come back from the game we will get a callback that we are connected, so we need to know what to do then
			isConnecting = true;

			// hide the Play button for visual consistency
			controlPanel.SetActive(false);

			// start the loader animation for visual effect.
			if (loaderAnime!=null)
			{
				loaderAnime.StartLoaderAnimation();
			}

			// we check if we are connected or not, we join if we are , else we initiate the connection to the server.
			if (PhotonNetwork.IsConnected)
			{
				string room = newRoomName;
				LogFeedback("Joining Room: " + room);
				// #Critical we need at this point to attempt joining a Random Room. If it fails, we'll get notified in OnJoinRandomFailed() and we'll create one.
				PhotonNetwork.JoinOrCreateRoom(room, new RoomOptions { MaxPlayers = 2, EmptyRoomTtl = 0 }, TypedLobby.Default);
			}
            else
            {

				LogFeedback("Connecting...");
				
				// #Critical, we must first and foremost connect to Photon Online Server.
			    PhotonNetwork.GameVersion = this.gameVersion;
				PhotonNetwork.ConnectUsingSettings();
			}
		}

		/// <summary>
		/// Logs the feedback in the UI view for the player, as opposed to inside the Unity Editor for the developer.
		/// </summary>
		/// <param name="message">Message.</param>
		void LogFeedback(string message)
		{
			// we do not assume there is a feedbackText defined.
			if (feedbackText == null) {
				return;
			}

            Debug.Log(message);

            // add new messages as a new line and at the bottom of the log.
            feedbackText.text += System.Environment.NewLine+message;
		}

        #endregion


        #region MonoBehaviourPunCallbacks CallBacks
        // below, we implement some callbacks of PUN
        // you can find PUN's callbacks in the class MonoBehaviourPunCallbacks


        /// <summary>
        /// Called after the connection to the master is established and authenticated
        /// </summary>
        public override void OnConnectedToMaster()
		{
            // we don't want to do anything if we are not attempting to join a room. 
			// this case where isConnecting is false is typically when you lost or quit the game, when this level is loaded, OnConnectedToMaster will be called, in that case
			// we don't want to do anything.
			if (isConnecting)
			{
				LogFeedback("Connected to Master... ");
				Debug.Log("PUN Basics Tutorial/Launcher: OnConnectedToMaster() was called by PUN. Now this client is connected and could join a room.\n Calling: PhotonNetwork.JoinRandomRoom(); Operation will fail if no room found");

                DoConnect();

            }
		}
	
        /// <summary>
        /// Called after disconnecting from the Photon server.
        /// </summary>
        public override void OnDisconnected(DisconnectCause cause)
		{
			LogFeedback("<Color=Red>Disconnected</Color> "+cause);
			Debug.LogError("PUN Basics: Disconnected");

			// #Critical: we failed to connect or got disconnected. There is not much we can do. Typically, a UI system should be in place to let the user attemp to connect again.
			loaderAnime.StopLoaderAnimation();

			isConnecting = false;
			controlPanel.SetActive(true);

		}

       
        /// <summary>
        /// Called when entering a room (by creating or joining it). Called on all clients (including the Master Client).
        /// </summary>
        /// <remarks>
        /// This method is commonly used to instantiate player characters.
        /// If a match has to be started "actively", you can call an [PunRPC](@ref PhotonView.RPC) triggered by a user's button-press or a timer.
        ///
        /// When this is called, you can usually already access the existing players in the room via PhotonNetwork.PlayerList.
        /// Also, all custom properties should be already available as Room.customProperties. Check Room..PlayerCount to find out if
        /// enough players are in the room to start playing.
        /// </remarks>
        public override void OnJoinedRoom()
		{
			LogFeedback("<Color=Green>Current Room: </Color> "+ PhotonNetwork.CurrentRoom.Name);
			Debug.Log("PUN Basics Tutorial/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.\nFrom here on, your game would be running.");
		
			// #Critical: We only load if we are the first player, else we rely on PhotonNetwork.AutomaticallySyncScene to sync our instance scene.
			if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
			{
				Debug.Log("We load the scene for player 1. PLayer 2 will bve auto sync.");

				// #Critical
				// Load the Room Level. 
				PhotonNetwork.LoadLevel("InGame");
			}
		}

		#endregion
		
	}
}