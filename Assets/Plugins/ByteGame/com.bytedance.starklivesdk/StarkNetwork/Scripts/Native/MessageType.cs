//------------------------------------------------------------------------------
// <auto-generated />
//
// This file was automatically generated by SWIG (http://www.swig.org).
// Version 4.0.2
//
// Do not make changes to this file unless you know what you are doing--modify
// the SWIG interface file instead.
//------------------------------------------------------------------------------

namespace StarkMatchmaking {

public enum MessageType {
  MsgUnknown = -1,
  Connected = 10000,
  ConnectFailed,
  Disconnected,
  ConnectClosed,
  PlayerCurrentState,
  RoomMatched = 10100,
  RoomMatchFailed,
  MatchMakingProcessUpdate,
  MatchCanceled,
  RoomCreated = 10200,
  RoomCreateFailed,
  RoomEntered,
  RoomEnteredFailed,
  PeopleEnteredRoom,
  PeopleLeavedRoom,
  RoomDateUpdate,
  RoomLeaved,
  RoomOwnerUpdate,
  RoomKickMember,
  RoomHandOverOwnerRst,
  SwitchRoomResult = 10260,
  SyncMsgReceived = 10800,
  OnError = 11000,
  RpcServerCall = 50000
}

}
