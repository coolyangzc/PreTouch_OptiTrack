using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;

public class OptiTrack : MonoBehaviour {

	const string IP = "192.168.1.167";
	const int PORT = 7643;

    public DebugInfo debugInfo;
	public PreTouch pretouch;
	public OptiTrack2Coor opti2coor;

    private List<Vector3> posList = new List<Vector3>();
	private Vector3 dot;
    private TcpClient socket;
	private Thread thread;

	private void Connect() {
		thread = new Thread(RevThread);
		thread.Start();
	}

	private void RevThread() {
		socket = new TcpClient();
		socket.Connect(IP, PORT);
		if (socket.Connected == false)
			return;
        Debug.Log("revThread Succeeded");
		StreamReader streamReader = new StreamReader(socket.GetStream());
		while (true) {
			string line = streamReader.ReadLine();
			if (line == null)
				break;
			string[] args = line.Split(' ');
			switch(args[0]) {
				case "start":
					break;
				case "end":
					break;
				case "rbStart":
                    posList.Clear();
					break;
				case "rbEnd":
                    opti2coor.UpdateAxis(posList);
                    posList.Clear();
					break;
				case "rbPosition":
                    posList.Add(50f * new Vector3(-float.Parse(args[1]), float.Parse(args[2]), float.Parse(args[3])));
					break;
				case "dot":
					dot = 50f * new Vector3(-float.Parse(args[1]), float.Parse(args[2]), float.Parse(args[3]));
					dot = opti2coor.ToCoor(dot);
					pretouch.Move(dot);
					debugInfo.Log("dot", dot.x.ToString("f2") + ',' + dot.y.ToString("f2") + ',' + dot.z.ToString("f2"));
					break;
			}
		}
	
	}

	void Start() {
		Connect();
	}

	void OnApplicationQuit() {
		socket.Close();
		thread.Abort();
	}
}
