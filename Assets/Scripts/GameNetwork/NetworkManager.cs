using GameNetwork;

public class NetworkManager
{
	private GameClient myClient;

	private string serverIp = "127.0.0.1";
	private int serverPort = 13000;

	public ConnectionResponse ConnectToGameServer(string username, int modelId)
	{
		if(myClient == null || myClient.isDisconnected())
			myClient = new GameClient();

		ConnectionResponse result = myClient.Connect(serverIp, serverPort, username, modelId);
		return result;
	}

	public void DisconnectClient()
	{
		if(myClient != null)
			myClient.Disconnect();
	}

	public bool isDisconnected()
	{
		if(myClient != null)
			return myClient.isDisconnected();
		else
			return false;
	}

	public int getClientId()
	{
		if (myClient != null)
			return myClient.GetClientId();
		else
			return -1;
	}

	public GameState GetLastGameState()
	{
		if (myClient != null)
			return myClient.GetLastGameState();
		else return null;
	}

	public void SendMove(int boxId)
	{
		// TODO - Send next move and receive next state
	}

}
