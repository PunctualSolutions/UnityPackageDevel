using UnityEngine;

namespace StarkLive
{
    public enum RequestType
    {
        INIT,
        SET_APP_INFO,
        
        START_TASK_LIKE,
        START_TASK_COMMENT,
        START_TASK_GIFT,
        
        STOP_TASK_LIKE,
        STOP_TASK_COMMENT,
        STOP_TASK_GIFT,
        
        QUERY_TASK_STATE_LIKE,
        QUERY_TASK_STATE_COMMENT,
        QUERY_TASK_STATE_GIFT,
        
        TOP_GIFT,
        QUERY_FAILED_GIFT,
        
        WRITE_RANK,
        READ_RANK,
        READ_RANK_WITH_OPENID,
    }
}