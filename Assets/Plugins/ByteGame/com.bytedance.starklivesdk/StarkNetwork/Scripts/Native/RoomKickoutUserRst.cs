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

public class RoomKickoutUserRst : MessageBase {
  private global::System.Runtime.InteropServices.HandleRef swigCPtr;

  internal RoomKickoutUserRst(global::System.IntPtr cPtr, bool cMemoryOwn) : base(stark_matchmakingPINVOKE.RoomKickoutUserRst_SWIGUpcast(cPtr), cMemoryOwn) {
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
  }

  internal static global::System.Runtime.InteropServices.HandleRef getCPtr(RoomKickoutUserRst obj) {
    return (obj == null) ? new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero) : obj.swigCPtr;
  }

  protected override void Dispose(bool disposing) {
    lock(this) {
      if (swigCPtr.Handle != global::System.IntPtr.Zero) {
        if (swigCMemOwn) {
          swigCMemOwn = false;
          stark_matchmakingPINVOKE.delete_RoomKickoutUserRst(swigCPtr);
        }
        swigCPtr = new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero);
      }
      base.Dispose(disposing);
    }
  }

  public ulong room_id {
    set {
      stark_matchmakingPINVOKE.RoomKickoutUserRst_room_id_set(swigCPtr, value);
    } 
    get {
      ulong ret = stark_matchmakingPINVOKE.RoomKickoutUserRst_room_id_get(swigCPtr);
      return ret;
    } 
  }

  public ulong be_kicked_user_id {
    set {
      stark_matchmakingPINVOKE.RoomKickoutUserRst_be_kicked_user_id_set(swigCPtr, value);
    } 
    get {
      ulong ret = stark_matchmakingPINVOKE.RoomKickoutUserRst_be_kicked_user_id_get(swigCPtr);
      return ret;
    } 
  }

  public RoomKickoutUserRst() : this(stark_matchmakingPINVOKE.new_RoomKickoutUserRst(), true) {
  }

}

}
