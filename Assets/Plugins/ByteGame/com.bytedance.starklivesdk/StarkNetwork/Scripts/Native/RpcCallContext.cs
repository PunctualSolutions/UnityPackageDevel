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

public class RpcCallContext : global::System.IDisposable {
  private global::System.Runtime.InteropServices.HandleRef swigCPtr;
  protected bool swigCMemOwn;

  internal RpcCallContext(global::System.IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
  }

  internal static global::System.Runtime.InteropServices.HandleRef getCPtr(RpcCallContext obj) {
    return (obj == null) ? new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero) : obj.swigCPtr;
  }

  ~RpcCallContext() {
    Dispose(false);
  }

  public void Dispose() {
    Dispose(true);
    global::System.GC.SuppressFinalize(this);
  }

  protected virtual void Dispose(bool disposing) {
    lock(this) {
      if (swigCPtr.Handle != global::System.IntPtr.Zero) {
        if (swigCMemOwn) {
          swigCMemOwn = false;
          stark_matchmakingPINVOKE.delete_RpcCallContext(swigCPtr);
        }
        swigCPtr = new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero);
      }
    }
  }

  public uint src_hash {
    set {
      stark_matchmakingPINVOKE.RpcCallContext_src_hash_set(swigCPtr, value);
    } 
    get {
      uint ret = stark_matchmakingPINVOKE.RpcCallContext_src_hash_get(swigCPtr);
      return ret;
    } 
  }

  public ulong src_id {
    set {
      stark_matchmakingPINVOKE.RpcCallContext_src_id_set(swigCPtr, value);
    } 
    get {
      ulong ret = stark_matchmakingPINVOKE.RpcCallContext_src_id_get(swigCPtr);
      return ret;
    } 
  }

  public string tgt_method {
    set {
      stark_matchmakingPINVOKE.RpcCallContext_tgt_method_set(swigCPtr, value);
      if (stark_matchmakingPINVOKE.SWIGPendingException.Pending) throw stark_matchmakingPINVOKE.SWIGPendingException.Retrieve();
    } 
    get {
      string ret = stark_matchmakingPINVOKE.RpcCallContext_tgt_method_get(swigCPtr);
      if (stark_matchmakingPINVOKE.SWIGPendingException.Pending) throw stark_matchmakingPINVOKE.SWIGPendingException.Retrieve();
      return ret;
    } 
  }

  public RpcCallContext() : this(stark_matchmakingPINVOKE.new_RpcCallContext(), true) {
  }

}

}
