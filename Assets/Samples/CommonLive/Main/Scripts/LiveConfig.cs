namespace ZhengDianWaiBao.CommonLive
{
    public struct LiveConfig
    {
        public string AccessKeySecret { get; private set; }
        public string AccessKeyId { get; private set; }
        public string Code { get; private set; }
        public long AppId { get; private set; }
    }
}