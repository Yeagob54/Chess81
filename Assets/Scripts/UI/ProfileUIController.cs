using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ProfileUIController : MonoBehaviour
{
	[SerializeField]
	GameObject profilePanel;

	[SerializeField]
	TextMeshProUGUI nameText;

	[SerializeField]
	TextMeshProUGUI whiteWins;

	[SerializeField]
	TextMeshProUGUI whiteLose;

	[SerializeField]
	TextMeshProUGUI whitePlayed;

	[SerializeField]
	TextMeshProUGUI blackWins;

	[SerializeField]
	TextMeshProUGUI blackLose;

	[SerializeField]
	TextMeshProUGUI blackPlayed;

	// Start is called before the first frame update
	void Start()
	{
		profilePanel.SetActive(false);
		nameText.text = "Name: " + UserControl.userData.name;
		whiteWins.text = UserControl.userData.whiteWins.ToString();
		whiteLose.text = UserControl.userData.whiteLose.ToString();
		whitePlayed.text = UserControl.userData.whitePlayed.ToString();
		blackWins.text = UserControl.userData.blackWins.ToString();
		blackLose.text = UserControl.userData.blackLose.ToString();
		blackPlayed.text = UserControl.userData.blackPlayed.ToString();
	}

	public void ShowProfilePanel()
	{
		if (!profilePanel.activeSelf)
			profilePanel.SetActive(true);
		else
			profilePanel.SetActive(false);
	}
}
