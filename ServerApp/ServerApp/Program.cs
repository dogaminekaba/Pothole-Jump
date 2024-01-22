using GameNetwork;

string serverIp = "127.0.0.1";
int serverPort = 13000;

bool isRunning = true;

GameServer gameServer = new GameServer();
gameServer.Start(serverIp, serverPort);

Console.CancelKeyPress += delegate (object? sender, ConsoleCancelEventArgs e)
{
	Console.WriteLine("Program is ended abruptly.");
	e.Cancel = true;
	StopServer();
};

AppDomain.CurrentDomain.ProcessExit += delegate (object? sender, EventArgs e)
{
	StopServer();
};


while (isRunning)
{
	// run server
}

void StopServer()
{
	isRunning = false;
	Console.WriteLine("\nExiting...");
	gameServer.StopServer();

	// wait for server to close connections
	Thread.Sleep(500);
}
