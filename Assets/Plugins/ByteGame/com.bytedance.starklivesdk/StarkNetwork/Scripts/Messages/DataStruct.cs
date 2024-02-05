using System;
using System.Collections.Generic;
using System.Linq;
using StarkMatchmaking;

namespace StarkNetwork
{
    public interface ISerializedMessage
    {
        void Parse(object data);
    }
    
    [Serializable]
    public class SerializedMessageBaseInfo: ISerializedMessage
    {
        public int msgType;
        public ulong requestId;
        public SerializedErrorInfo errorInfo;

        public virtual void Parse(object data)
        {
            InnerParse((MessageBase)data);
        }

        private void InnerParse(MessageBase data)
        {
            msgType = data.msg_type;
            requestId = data.request_id;
            errorInfo = new SerializedErrorInfo(data.err, data.request_id);
        }
    }
    
    [Serializable]
    public class SerializedErrorInfo
    {
        public ulong requestId;
        public int errCode;
        public string errMsg;

        public SerializedErrorInfo(ErrorInfo data, ulong reqId)
        {
            requestId = reqId;
            errCode = data.err_code;
            errMsg = data.err_msg;
        }
    }
    
    [Serializable]
    public class SerializedConnectResult : SerializedMessageBaseInfo
    {
        public ConnectResumeMod resumeMod;
        
        public override void Parse(object data)
        {
            base.Parse(data);
            InnerParse((ConnectResult)data);
        }

        private void InnerParse(ConnectResult data)
        {
            resumeMod = (ConnectResumeMod)data.resume_mod;
        }
    }
    
    [Serializable]
    public class SerializedConnectFailedResult : SerializedMessageBaseInfo
    {
        public ConnectResumeMod resumeMod;
        
        public override void Parse(object data)
        {
            base.Parse(data);
            InnerParse((ConnectFailedResult)data);
        }

        private void InnerParse(ConnectFailedResult data)
        {
            resumeMod = (ConnectResumeMod)data.resume_mod;
        }
    }
    
    [Serializable]
    public class SerializedConnectCloseMessage : SerializedMessageBaseInfo
    {
        public ConnectCloseReason closeReason;
        
        public override void Parse(object data)
        {
            base.Parse(data);
            InnerParse((ConnectCloseMsg)data);
        }

        private void InnerParse(ConnectCloseMsg data)
        {
            closeReason = (ConnectCloseReason)data.close_reason;
        }
    }
    
    [Serializable]
    public class SerializedPlayerCurrentInfo : SerializedMessageBaseInfo
    {
        public SerializedRoomInfo[] roomList;
        
        public override void Parse(object data)
        {
            base.Parse(data);
            InnerParse((PlayerCurrentInfoMsg)data);
        }

        private void InnerParse(PlayerCurrentInfoMsg data)
        {
            roomList = data.room_list.Select(x => new SerializedRoomInfo(x)).ToArray();
        }
    }
    
    [Serializable]
    public class SerializedRoomInfo
    {
        public ulong roomId;
        public ulong ownerId;
        public int maxPlayerNum;
        public HashSet<ulong> users;
        public Dictionary<string, string> metaData;

        public SerializedRoomInfo(Room data)
        {
            roomId = data.room_id;
            ownerId = data.owner_id;
            maxPlayerNum = (int)data.max_user_num;
            users = new HashSet<ulong>(data.users);
            metaData = data.meta_data.ToDictionary();
            // dsInfo = new SerializedDsInfo(data.ds_info);
        }
    }

    [Serializable]
    public class SerializedDsInfo
    {
        public string ip;
        public int port;
        public string token;
        
        public SerializedDsInfo(DedicatedServer data)
        {
            ip = data.ip;
            port = data.port;
            token = data.token;
        }
    }
    
    [Serializable]
    public class SerializedRoomInfoMsg : SerializedMessageBaseInfo
    {
        public ulong roomId;
        public ulong ownerId;
        public int maxPlayerNum;
        public HashSet<ulong> users;
        public Dictionary<string, string> metaData;
        
        public override void Parse(object data)
        {
            base.Parse(data);
            InnerParse((RoomInfoMsg)data);
        }

        private void InnerParse(RoomInfoMsg data)
        {
            roomId = data.room_id;
            ownerId = data.owner_id;
            // curPlayerNum = data.cur_user_num;
            maxPlayerNum = data.max_user_num;
            users = new HashSet<ulong>(data.users);
            metaData = data.meta_data.ToDictionary();
            // dsInfo = new SerializedDsInfo(data.ds_info);
        }
    }
    
    [Serializable]
    public class SerializedUserEnterOrLeaveInfo : SerializedMessageBaseInfo
    {
        public ulong roomId;
        public ulong userId;
        
        public override void Parse(object data)
        {
            base.Parse(data);
            InnerParse((UserEnterOrLeaveInfo)data);
        }

        private void InnerParse(UserEnterOrLeaveInfo data)
        {
            roomId = data.room_id;
            userId = data.user_id;
        }
    }
    
    [Serializable]
    public class SerializedRoomOwnerUpdateInfo : SerializedMessageBaseInfo
    {
        public ulong roomId;
        public ulong newOwnerId;
        
        public override void Parse(object data)
        {
            base.Parse(data);
            InnerParse((RoomOwnerUpdateInfo)data);
        }

        private void InnerParse(RoomOwnerUpdateInfo data)
        {
            roomId = data.room_id;
            newOwnerId = data.new_owner_id;
        }
    }
    
    [Serializable]
    public class SerializedRoomLeaveResult : SerializedMessageBaseInfo
    {
        public ulong roomId;
        public LeaveRoomReason reason;
        
        public override void Parse(object data)
        {
            base.Parse(data);
            InnerParse((RoomLeaveResult)data);
        }

        private void InnerParse(RoomLeaveResult data)
        {
            roomId = data.room_id;
            reason = data.reason;
        }
    }
    
    [Serializable]
    public class SerializedRoomMetaDataUpdateResult : SerializedMessageBaseInfo
    {
        public ulong roomId;
        /// <summary>
        /// 发起改变的人
        /// </summary>
        public ulong changerId;
        /// <summary>
        /// 更新的元信息
        /// </summary>
        public Dictionary<string, string> metaData;
        
        public override void Parse(object data)
        {
            base.Parse(data);
            InnerParse((RoomMeteDataUpdateResult)data);
        }

        private void InnerParse(RoomMeteDataUpdateResult data)
        {
            roomId = data.room_id;
            changerId = data.changer_id;
            metaData = data.changed_meta_data.meta_data.ToDictionary();
        }
    }
    
    [Serializable]
    public class SerializedRoomKickOutUserResult : SerializedMessageBaseInfo
    {
        public ulong roomId;
        public ulong kickedUserId;
        
        public override void Parse(object data)
        {
            base.Parse(data);
            InnerParse((RoomKickoutUserRst)data);
        }

        private void InnerParse(RoomKickoutUserRst data)
        {
            roomId = data.room_id;
            kickedUserId = data.be_kicked_user_id;
        }
    }

    [Serializable]
    public class SerializedSwitchRoomRst : SerializedMessageBaseInfo
    {
        public ulong roomId;
        public string token;
        public override void Parse(object data)
        {
            base.Parse(data);
            InnerParse((SwitchRoomRst)data);
        }

        private void InnerParse(SwitchRoomRst data)
        {
            roomId = data.room_id;
            token = data.token;
        }
    }

    [Serializable]
    public class SerializedSyncMsgInfo : SerializedMessageBaseInfo
    {
        public ulong roomId;
        public ulong fromUserId;
        public string msg;
        
        public override void Parse(object data)
        {
            base.Parse(data);
            InnerParse((SyncMsgInfo)data);
        }

        private void InnerParse(SyncMsgInfo data)
        {
            roomId = data.room_id;
            fromUserId = data.from_player_id;
            msg = data.msg;
        }
    }
    
    [Serializable]
    public class SerializedRpcCallInfo : SerializedMessageBaseInfo
    {
        public string methodName;
        public Vec_string paramTypes;
        public Vec_Int intArr;
        public Vec_string strArr;
        public Vec_Bool boolArr;
        public Vec_Double doubleArr;
        public Vec_UInt uint32Arr;
        public Vec_ulong uint64_arr;
        public RpcCallContext context;
        public override void Parse(object data)
        {
            base.Parse(data);
            InnerParse((RpcServerCallInfo)data);
        }
        private void InnerParse(RpcServerCallInfo data)
        {
            methodName = data.method_name;
            paramTypes = data.param_type_arr;
            intArr = data.int_arr;
            strArr = data.str_arr;
            boolArr = data.bool_arr;
            doubleArr = data.double_arr;
            uint32Arr = data.uint32_arr;
            uint64_arr = data.uint64_arr;
            context = data.context;
        }
    }

    [Serializable]
    public class SerializedLiveGiftInfoMsg : SerializedMessageBaseInfo
    {
        // public string GiftId;
        // public int GiftNum;
        // public int GiftValue;
        // public string AvatarUrl;
        // public string NickName;
        // public DateTime Time;
        
    }

    [Serializable]
    public class SerializedLiveCommentInfoMsg : SerializedMessageBaseInfo
    {
        
    }
    
    [Serializable]
    public class SerializedLiveLikeInfoMsg : SerializedMessageBaseInfo
    {
        
    }
}