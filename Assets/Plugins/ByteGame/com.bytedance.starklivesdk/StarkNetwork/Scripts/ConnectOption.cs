// Copyright @ www.bytedance.com
// Time: 2022-08-16
// Author: wuwenbin@bytedance.com
// Description: 将登录用的信息封装成单一结构体

namespace StarkNetwork
{
    public interface IConnectOption
    {
        string Token { get; }
        string Address { get; }
        int Port { get; }
        bool IsFake { get; }
    }

    public class ConnectOption : IConnectOption
    {
        private string _token;
        public string Token => _token;
        private string _address;
        public string Address => _address;
        private int _port;
        public int Port => _port;
        private bool _isFake;
        public bool IsFake => _isFake;

        public ConnectOption SetToken(string uid)
        {
            _token = uid;
            return this;
        }

        public ConnectOption SetServer(string address, int port)
        {
            _address = address;
            _port = port;
            return this;
        }

        public ConnectOption SetIsFake(bool isFake)
        {
            _isFake = isFake;
            return this;
        }

    }

}