using System.Collections.Generic;
using UnityEngine;

public class TileContainer : Singleton<TileContainer>
{
    public Dictionary<string, TileVariant> _tileDict;

    [System.Serializable]
    public class TileVariant
    {
        public TileVariant(string m, int r, Socket s)
        {
            mesh = m;
            rotation = r;
            sockets = s;
        }

        public string mesh;
        public int rotation;
        public Socket sockets;

    }

    [System.Serializable]
    public class Socket
    {
        public string posZ;
        public string negZ;
        public string posX;
        public string negX;

        public Socket(string pZ, string nZ, string pX, string nX)
        {
            posZ = pZ;
            negZ = nZ;
            posX = pX;
            negX = nX;
        }
    }

    private TileVariant pathBlank = new TileVariant("pathBlank", 0, new Socket("-1", "-1", "-1", "-1"));
    private TileVariant pathCross = new TileVariant("pathCross", 0, new Socket("1", "1", "1", "1"));

    private void Start()
    {
        
    }
}
