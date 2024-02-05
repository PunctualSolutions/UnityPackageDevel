using System.Collections.Generic;

namespace StarkNetwork
{
    public partial class NetworkClient
    {
        private readonly Dictionary<ulong, NetworkOperation> _records = new Dictionary<ulong, NetworkOperation>();
        private readonly Queue<ulong> _recordQueue = new Queue<ulong>();

        private int _maxRecordCount = 10;

        internal void SetMaxRecordCount(int maxCount) => _maxRecordCount = maxCount;

        /// <summary>
        /// 记录最后几次发送的消息
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="operation"></param>
        private void RecordOperationRequest(ulong requestId, NetworkOperation operation)
        {
            if (!_records.ContainsKey(requestId))
            {
                _records.Add(requestId, operation);
                _recordQueue.Enqueue(requestId);
            }
            
            if (_recordQueue.Count > _maxRecordCount)
            {
                var requestTobeDelete = _recordQueue.Dequeue();
                _records.Remove(requestTobeDelete);
            }
        }

        /// <summary>
        /// 通过 requestId 查找此前的操作，用于错误消息解析
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns></returns>
        internal NetworkOperation FindOperationRequest(ulong requestId)
        {
            if (_records.TryGetValue(requestId, out var operation))
            {
                return operation;
            }
            else
            {
                return null;
            }
        }
    }
}