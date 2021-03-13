public enum Scenes { LOGIN = 0, MAINMENU, MATCHMAKING, INGAME, INVITATIONMATCH }

public enum ClockType { WHITE, BLACK }
public enum EndGameStates { WHITE_WIN, BLACK_WIN, DRAW }

/// <summary>
/// All possible board modes.
/// </summary>
public enum BoardMode { Undefined, PlayerVsPlayer, PlayerVsEngine, EngineVsPlayer, EngineVsEngine, Online, OnlineWhite, OnlineBlack }
