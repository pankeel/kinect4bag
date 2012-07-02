/* ----------------------------------------------------------------------------
 * This file was automatically generated by SWIG (http://www.swig.org).
 * Version 2.0.7
 *
 * Do not make changes to this file unless you know what you are doing--modify
 * the SWIG interface file instead.
 * ----------------------------------------------------------------------------- */


using System;
using System.Runtime.InteropServices;

class CGePhysicsPINVOKE {

  protected class SWIGExceptionHelper {

    public delegate void ExceptionDelegate(string message);
    public delegate void ExceptionArgumentDelegate(string message, string paramName);

    static ExceptionDelegate applicationDelegate = new ExceptionDelegate(SetPendingApplicationException);
    static ExceptionDelegate arithmeticDelegate = new ExceptionDelegate(SetPendingArithmeticException);
    static ExceptionDelegate divideByZeroDelegate = new ExceptionDelegate(SetPendingDivideByZeroException);
    static ExceptionDelegate indexOutOfRangeDelegate = new ExceptionDelegate(SetPendingIndexOutOfRangeException);
    static ExceptionDelegate invalidCastDelegate = new ExceptionDelegate(SetPendingInvalidCastException);
    static ExceptionDelegate invalidOperationDelegate = new ExceptionDelegate(SetPendingInvalidOperationException);
    static ExceptionDelegate ioDelegate = new ExceptionDelegate(SetPendingIOException);
    static ExceptionDelegate nullReferenceDelegate = new ExceptionDelegate(SetPendingNullReferenceException);
    static ExceptionDelegate outOfMemoryDelegate = new ExceptionDelegate(SetPendingOutOfMemoryException);
    static ExceptionDelegate overflowDelegate = new ExceptionDelegate(SetPendingOverflowException);
    static ExceptionDelegate systemDelegate = new ExceptionDelegate(SetPendingSystemException);

    static ExceptionArgumentDelegate argumentDelegate = new ExceptionArgumentDelegate(SetPendingArgumentException);
    static ExceptionArgumentDelegate argumentNullDelegate = new ExceptionArgumentDelegate(SetPendingArgumentNullException);
    static ExceptionArgumentDelegate argumentOutOfRangeDelegate = new ExceptionArgumentDelegate(SetPendingArgumentOutOfRangeException);

    [DllImport("CGePhysics", EntryPoint="SWIGRegisterExceptionCallbacks_CGePhysics")]
    public static extern void SWIGRegisterExceptionCallbacks_CGePhysics(
                                ExceptionDelegate applicationDelegate,
                                ExceptionDelegate arithmeticDelegate,
                                ExceptionDelegate divideByZeroDelegate, 
                                ExceptionDelegate indexOutOfRangeDelegate, 
                                ExceptionDelegate invalidCastDelegate,
                                ExceptionDelegate invalidOperationDelegate,
                                ExceptionDelegate ioDelegate,
                                ExceptionDelegate nullReferenceDelegate,
                                ExceptionDelegate outOfMemoryDelegate, 
                                ExceptionDelegate overflowDelegate, 
                                ExceptionDelegate systemExceptionDelegate);

    [DllImport("CGePhysics", EntryPoint="SWIGRegisterExceptionArgumentCallbacks_CGePhysics")]
    public static extern void SWIGRegisterExceptionCallbacksArgument_CGePhysics(
                                ExceptionArgumentDelegate argumentDelegate,
                                ExceptionArgumentDelegate argumentNullDelegate,
                                ExceptionArgumentDelegate argumentOutOfRangeDelegate);

    static void SetPendingApplicationException(string message) {
      SWIGPendingException.Set(new System.ApplicationException(message, SWIGPendingException.Retrieve()));
    }
    static void SetPendingArithmeticException(string message) {
      SWIGPendingException.Set(new System.ArithmeticException(message, SWIGPendingException.Retrieve()));
    }
    static void SetPendingDivideByZeroException(string message) {
      SWIGPendingException.Set(new System.DivideByZeroException(message, SWIGPendingException.Retrieve()));
    }
    static void SetPendingIndexOutOfRangeException(string message) {
      SWIGPendingException.Set(new System.IndexOutOfRangeException(message, SWIGPendingException.Retrieve()));
    }
    static void SetPendingInvalidCastException(string message) {
      SWIGPendingException.Set(new System.InvalidCastException(message, SWIGPendingException.Retrieve()));
    }
    static void SetPendingInvalidOperationException(string message) {
      SWIGPendingException.Set(new System.InvalidOperationException(message, SWIGPendingException.Retrieve()));
    }
    static void SetPendingIOException(string message) {
      SWIGPendingException.Set(new System.IO.IOException(message, SWIGPendingException.Retrieve()));
    }
    static void SetPendingNullReferenceException(string message) {
      SWIGPendingException.Set(new System.NullReferenceException(message, SWIGPendingException.Retrieve()));
    }
    static void SetPendingOutOfMemoryException(string message) {
      SWIGPendingException.Set(new System.OutOfMemoryException(message, SWIGPendingException.Retrieve()));
    }
    static void SetPendingOverflowException(string message) {
      SWIGPendingException.Set(new System.OverflowException(message, SWIGPendingException.Retrieve()));
    }
    static void SetPendingSystemException(string message) {
      SWIGPendingException.Set(new System.SystemException(message, SWIGPendingException.Retrieve()));
    }

    static void SetPendingArgumentException(string message, string paramName) {
      SWIGPendingException.Set(new System.ArgumentException(message, paramName, SWIGPendingException.Retrieve()));
    }
    static void SetPendingArgumentNullException(string message, string paramName) {
      Exception e = SWIGPendingException.Retrieve();
      if (e != null) message = message + " Inner Exception: " + e.Message;
      SWIGPendingException.Set(new System.ArgumentNullException(paramName, message));
    }
    static void SetPendingArgumentOutOfRangeException(string message, string paramName) {
      Exception e = SWIGPendingException.Retrieve();
      if (e != null) message = message + " Inner Exception: " + e.Message;
      SWIGPendingException.Set(new System.ArgumentOutOfRangeException(paramName, message));
    }

    static SWIGExceptionHelper() {
      SWIGRegisterExceptionCallbacks_CGePhysics(
                                applicationDelegate,
                                arithmeticDelegate,
                                divideByZeroDelegate,
                                indexOutOfRangeDelegate,
                                invalidCastDelegate,
                                invalidOperationDelegate,
                                ioDelegate,
                                nullReferenceDelegate,
                                outOfMemoryDelegate,
                                overflowDelegate,
                                systemDelegate);

      SWIGRegisterExceptionCallbacksArgument_CGePhysics(
                                argumentDelegate,
                                argumentNullDelegate,
                                argumentOutOfRangeDelegate);
    }
  }

  protected static SWIGExceptionHelper swigExceptionHelper = new SWIGExceptionHelper();

  public class SWIGPendingException {
    [ThreadStatic]
    private static Exception pendingException = null;
    private static int numExceptionsPending = 0;

    public static bool Pending {
      get {
        bool pending = false;
        if (numExceptionsPending > 0)
          if (pendingException != null)
            pending = true;
        return pending;
      } 
    }

    public static void Set(Exception e) {
      if (pendingException != null)
        throw new ApplicationException("FATAL: An earlier pending exception from unmanaged code was missed and thus not thrown (" + pendingException.ToString() + ")", e);
      pendingException = e;
      lock(typeof(CGePhysicsPINVOKE)) {
        numExceptionsPending++;
      }
    }

    public static Exception Retrieve() {
      Exception e = null;
      if (numExceptionsPending > 0) {
        if (pendingException != null) {
          e = pendingException;
          pendingException = null;
          lock(typeof(CGePhysicsPINVOKE)) {
            numExceptionsPending--;
          }
        }
      }
      return e;
    }
  }


  protected class SWIGStringHelper {

    public delegate string SWIGStringDelegate(string message);
    static SWIGStringDelegate stringDelegate = new SWIGStringDelegate(CreateString);

    [DllImport("CGePhysics", EntryPoint="SWIGRegisterStringCallback_CGePhysics")]
    public static extern void SWIGRegisterStringCallback_CGePhysics(SWIGStringDelegate stringDelegate);

    static string CreateString(string cString) {
      return cString;
    }

    static SWIGStringHelper() {
      SWIGRegisterStringCallback_CGePhysics(stringDelegate);
    }
  }

  static protected SWIGStringHelper swigStringHelper = new SWIGStringHelper();


  static CGePhysicsPINVOKE() {
  }


  [DllImport("CGePhysics", EntryPoint="CSharp_new_CGePhysX")]
  public static extern IntPtr new_CGePhysX();

  [DllImport("CGePhysics", EntryPoint="CSharp_delete_CGePhysX")]
  public static extern void delete_CGePhysX(HandleRef jarg1);

  [DllImport("CGePhysics", EntryPoint="CSharp_CGePhysX_OnInit")]
  public static extern void CGePhysX_OnInit(HandleRef jarg1);

  [DllImport("CGePhysics", EntryPoint="CSharp_CGePhysX_StepPhysX")]
  public static extern void CGePhysX_StepPhysX(HandleRef jarg1, float jarg2);

  [DllImport("CGePhysics", EntryPoint="CSharp_CGePhysX_createCloth__SWIG_0")]
  public static extern void CGePhysX_createCloth__SWIG_0(HandleRef jarg1, string jarg2, float jarg3, HandleRef jarg4, HandleRef jarg5);

  [DllImport("CGePhysics", EntryPoint="CSharp_CGePhysX_createCloth__SWIG_1")]
  public static extern void CGePhysX_createCloth__SWIG_1(HandleRef jarg1, string jarg2, float jarg3, HandleRef jarg4);

  [DllImport("CGePhysics", EntryPoint="CSharp_CGePhysX_createCloth__SWIG_2")]
  public static extern void CGePhysX_createCloth__SWIG_2(HandleRef jarg1, string jarg2, float jarg3);

  [DllImport("CGePhysics", EntryPoint="CSharp_CGePhysX_createCloth__SWIG_3")]
  public static extern void CGePhysX_createCloth__SWIG_3(HandleRef jarg1, string jarg2);

  [DllImport("CGePhysics", EntryPoint="CSharp_CGePhysX_addCollisionSpheres")]
  public static extern bool CGePhysX_addCollisionSpheres(HandleRef jarg1, HandleRef jarg2, HandleRef jarg3, HandleRef jarg4);

  [DllImport("CGePhysics", EntryPoint="CSharp_CGePhysX_updateCollisionSpheres")]
  public static extern void CGePhysX_updateCollisionSpheres(HandleRef jarg1, HandleRef jarg2);

  [DllImport("CGePhysics", EntryPoint="CSharp_CGePhysX_getClothIndices")]
  public static extern bool CGePhysX_getClothIndices(HandleRef jarg1, HandleRef jarg2, HandleRef jarg3);

  [DllImport("CGePhysics", EntryPoint="CSharp_CGePhysX_getClothIndicesCount")]
  public static extern int CGePhysX_getClothIndicesCount(HandleRef jarg1);

  [DllImport("CGePhysics", EntryPoint="CSharp_CGePhysX_getClothIndicesContent")]
  public static extern bool CGePhysX_getClothIndicesContent(HandleRef jarg1, [Out, MarshalAs(UnmanagedType.LPArray)]int[] jarg2);

  [DllImport("CGePhysics", EntryPoint="CSharp_CGePhysX_getClothParticles__SWIG_0")]
  public static extern bool CGePhysX_getClothParticles__SWIG_0(HandleRef jarg1, HandleRef jarg2);

  [DllImport("CGePhysics", EntryPoint="CSharp_CGePhysX_getClothParticesCount")]
  public static extern int CGePhysX_getClothParticesCount(HandleRef jarg1);

  [DllImport("CGePhysics", EntryPoint="CSharp_CGePhysX_getClothParticlesContent")]
  public static extern bool CGePhysX_getClothParticlesContent(HandleRef jarg1, [Out, MarshalAs(UnmanagedType.LPArray)]float[] jarg2);

  [DllImport("CGePhysics", EntryPoint="CSharp_CGePhysX_getClothParticles__SWIG_1")]
  public static extern bool CGePhysX_getClothParticles__SWIG_1(HandleRef jarg1, HandleRef jarg2, HandleRef jarg3);

  [DllImport("CGePhysics", EntryPoint="CSharp_CGePhysX_Destroy")]
  public static extern void CGePhysX_Destroy(HandleRef jarg1);

  [DllImport("CGePhysics", EntryPoint="CSharp_CGePhysX_mCloth_set")]
  public static extern void CGePhysX_mCloth_set(HandleRef jarg1, HandleRef jarg2);

  [DllImport("CGePhysics", EntryPoint="CSharp_CGePhysX_mCloth_get")]
  public static extern IntPtr CGePhysX_mCloth_get(HandleRef jarg1);

  [DllImport("CGePhysics", EntryPoint="CSharp_CGePhysX_mClothIndices_set")]
  public static extern void CGePhysX_mClothIndices_set(HandleRef jarg1, HandleRef jarg2);

  [DllImport("CGePhysics", EntryPoint="CSharp_CGePhysX_mClothIndices_get")]
  public static extern IntPtr CGePhysX_mClothIndices_get(HandleRef jarg1);

  [DllImport("CGePhysics", EntryPoint="CSharp_CGePhysX_mClothVertices_set")]
  public static extern void CGePhysX_mClothVertices_set(HandleRef jarg1, HandleRef jarg2);

  [DllImport("CGePhysics", EntryPoint="CSharp_CGePhysX_mClothVertices_get")]
  public static extern IntPtr CGePhysX_mClothVertices_get(HandleRef jarg1);

  [DllImport("CGePhysics", EntryPoint="CSharp_CGePhysX_mClothCollisionData_set")]
  public static extern void CGePhysX_mClothCollisionData_set(HandleRef jarg1, HandleRef jarg2);

  [DllImport("CGePhysics", EntryPoint="CSharp_CGePhysX_mClothCollisionData_get")]
  public static extern IntPtr CGePhysX_mClothCollisionData_get(HandleRef jarg1);

  [DllImport("CGePhysics", EntryPoint="CSharp_CGePhysX_mClothCollisionSpheres_set")]
  public static extern void CGePhysX_mClothCollisionSpheres_set(HandleRef jarg1, HandleRef jarg2);

  [DllImport("CGePhysics", EntryPoint="CSharp_CGePhysX_mClothCollisionSpheres_get")]
  public static extern IntPtr CGePhysX_mClothCollisionSpheres_get(HandleRef jarg1);

  [DllImport("CGePhysics", EntryPoint="CSharp_CGePhysX_mClothCollisionSpheresIndexPair_set")]
  public static extern void CGePhysX_mClothCollisionSpheresIndexPair_set(HandleRef jarg1, HandleRef jarg2);

  [DllImport("CGePhysics", EntryPoint="CSharp_CGePhysX_mClothCollisionSpheresIndexPair_get")]
  public static extern IntPtr CGePhysX_mClothCollisionSpheresIndexPair_get(HandleRef jarg1);

  [DllImport("CGePhysics", EntryPoint="CSharp_CGePhysX_mClothCollisionIndexPair_set")]
  public static extern void CGePhysX_mClothCollisionIndexPair_set(HandleRef jarg1, HandleRef jarg2);

  [DllImport("CGePhysics", EntryPoint="CSharp_CGePhysX_mClothCollisionIndexPair_get")]
  public static extern IntPtr CGePhysX_mClothCollisionIndexPair_get(HandleRef jarg1);

  [DllImport("CGePhysics", EntryPoint="CSharp_CGePhysX_mCharacterScale_set")]
  public static extern void CGePhysX_mCharacterScale_set(HandleRef jarg1, HandleRef jarg2);

  [DllImport("CGePhysics", EntryPoint="CSharp_CGePhysX_mCharacterScale_get")]
  public static extern IntPtr CGePhysX_mCharacterScale_get(HandleRef jarg1);

  [DllImport("CGePhysics", EntryPoint="CSharp_CGePhysX_getPhysics")]
  public static extern IntPtr CGePhysX_getPhysics(HandleRef jarg1);

  [DllImport("CGePhysics", EntryPoint="CSharp_CGePhysX_getScene")]
  public static extern IntPtr CGePhysX_getScene(HandleRef jarg1);

  [DllImport("CGePhysics", EntryPoint="CSharp_CGePhysX_getCooking")]
  public static extern IntPtr CGePhysX_getCooking(HandleRef jarg1);

  [DllImport("CGePhysics", EntryPoint="CSharp_PxVec3Wrapper_x_set")]
  public static extern void PxVec3Wrapper_x_set(HandleRef jarg1, double jarg2);

  [DllImport("CGePhysics", EntryPoint="CSharp_PxVec3Wrapper_x_get")]
  public static extern double PxVec3Wrapper_x_get(HandleRef jarg1);

  [DllImport("CGePhysics", EntryPoint="CSharp_PxVec3Wrapper_y_set")]
  public static extern void PxVec3Wrapper_y_set(HandleRef jarg1, double jarg2);

  [DllImport("CGePhysics", EntryPoint="CSharp_PxVec3Wrapper_y_get")]
  public static extern double PxVec3Wrapper_y_get(HandleRef jarg1);

  [DllImport("CGePhysics", EntryPoint="CSharp_PxVec3Wrapper_z_set")]
  public static extern void PxVec3Wrapper_z_set(HandleRef jarg1, double jarg2);

  [DllImport("CGePhysics", EntryPoint="CSharp_PxVec3Wrapper_z_get")]
  public static extern double PxVec3Wrapper_z_get(HandleRef jarg1);

  [DllImport("CGePhysics", EntryPoint="CSharp_new_PxVec3Wrapper__SWIG_0")]
  public static extern IntPtr new_PxVec3Wrapper__SWIG_0();

  [DllImport("CGePhysics", EntryPoint="CSharp_new_PxVec3Wrapper__SWIG_1")]
  public static extern IntPtr new_PxVec3Wrapper__SWIG_1(double jarg1, double jarg2, double jarg3);

  [DllImport("CGePhysics", EntryPoint="CSharp_delete_PxVec3Wrapper")]
  public static extern void delete_PxVec3Wrapper(HandleRef jarg1);

  [DllImport("CGePhysics", EntryPoint="CSharp_Int_Vector_Clear")]
  public static extern void Int_Vector_Clear(HandleRef jarg1);

  [DllImport("CGePhysics", EntryPoint="CSharp_Int_Vector_Add")]
  public static extern void Int_Vector_Add(HandleRef jarg1, int jarg2);

  [DllImport("CGePhysics", EntryPoint="CSharp_Int_Vector_size")]
  public static extern uint Int_Vector_size(HandleRef jarg1);

  [DllImport("CGePhysics", EntryPoint="CSharp_Int_Vector_capacity")]
  public static extern uint Int_Vector_capacity(HandleRef jarg1);

  [DllImport("CGePhysics", EntryPoint="CSharp_Int_Vector_reserve")]
  public static extern void Int_Vector_reserve(HandleRef jarg1, uint jarg2);

  [DllImport("CGePhysics", EntryPoint="CSharp_new_Int_Vector__SWIG_0")]
  public static extern IntPtr new_Int_Vector__SWIG_0();

  [DllImport("CGePhysics", EntryPoint="CSharp_new_Int_Vector__SWIG_1")]
  public static extern IntPtr new_Int_Vector__SWIG_1(HandleRef jarg1);

  [DllImport("CGePhysics", EntryPoint="CSharp_new_Int_Vector__SWIG_2")]
  public static extern IntPtr new_Int_Vector__SWIG_2(int jarg1);

  [DllImport("CGePhysics", EntryPoint="CSharp_Int_Vector_getitemcopy")]
  public static extern int Int_Vector_getitemcopy(HandleRef jarg1, int jarg2);

  [DllImport("CGePhysics", EntryPoint="CSharp_Int_Vector_getitem")]
  public static extern int Int_Vector_getitem(HandleRef jarg1, int jarg2);

  [DllImport("CGePhysics", EntryPoint="CSharp_Int_Vector_setitem")]
  public static extern void Int_Vector_setitem(HandleRef jarg1, int jarg2, int jarg3);

  [DllImport("CGePhysics", EntryPoint="CSharp_Int_Vector_AddRange")]
  public static extern void Int_Vector_AddRange(HandleRef jarg1, HandleRef jarg2);

  [DllImport("CGePhysics", EntryPoint="CSharp_Int_Vector_GetRange")]
  public static extern IntPtr Int_Vector_GetRange(HandleRef jarg1, int jarg2, int jarg3);

  [DllImport("CGePhysics", EntryPoint="CSharp_Int_Vector_Insert")]
  public static extern void Int_Vector_Insert(HandleRef jarg1, int jarg2, int jarg3);

  [DllImport("CGePhysics", EntryPoint="CSharp_Int_Vector_InsertRange")]
  public static extern void Int_Vector_InsertRange(HandleRef jarg1, int jarg2, HandleRef jarg3);

  [DllImport("CGePhysics", EntryPoint="CSharp_Int_Vector_RemoveAt")]
  public static extern void Int_Vector_RemoveAt(HandleRef jarg1, int jarg2);

  [DllImport("CGePhysics", EntryPoint="CSharp_Int_Vector_RemoveRange")]
  public static extern void Int_Vector_RemoveRange(HandleRef jarg1, int jarg2, int jarg3);

  [DllImport("CGePhysics", EntryPoint="CSharp_Int_Vector_Repeat")]
  public static extern IntPtr Int_Vector_Repeat(int jarg1, int jarg2);

  [DllImport("CGePhysics", EntryPoint="CSharp_Int_Vector_Reverse__SWIG_0")]
  public static extern void Int_Vector_Reverse__SWIG_0(HandleRef jarg1);

  [DllImport("CGePhysics", EntryPoint="CSharp_Int_Vector_Reverse__SWIG_1")]
  public static extern void Int_Vector_Reverse__SWIG_1(HandleRef jarg1, int jarg2, int jarg3);

  [DllImport("CGePhysics", EntryPoint="CSharp_Int_Vector_SetRange")]
  public static extern void Int_Vector_SetRange(HandleRef jarg1, int jarg2, HandleRef jarg3);

  [DllImport("CGePhysics", EntryPoint="CSharp_Int_Vector_Contains")]
  public static extern bool Int_Vector_Contains(HandleRef jarg1, int jarg2);

  [DllImport("CGePhysics", EntryPoint="CSharp_Int_Vector_IndexOf")]
  public static extern int Int_Vector_IndexOf(HandleRef jarg1, int jarg2);

  [DllImport("CGePhysics", EntryPoint="CSharp_Int_Vector_LastIndexOf")]
  public static extern int Int_Vector_LastIndexOf(HandleRef jarg1, int jarg2);

  [DllImport("CGePhysics", EntryPoint="CSharp_Int_Vector_Remove")]
  public static extern bool Int_Vector_Remove(HandleRef jarg1, int jarg2);

  [DllImport("CGePhysics", EntryPoint="CSharp_delete_Int_Vector")]
  public static extern void delete_Int_Vector(HandleRef jarg1);

  [DllImport("CGePhysics", EntryPoint="CSharp_PxVec3_Vector_Clear")]
  public static extern void PxVec3_Vector_Clear(HandleRef jarg1);

  [DllImport("CGePhysics", EntryPoint="CSharp_PxVec3_Vector_Add")]
  public static extern void PxVec3_Vector_Add(HandleRef jarg1, HandleRef jarg2);

  [DllImport("CGePhysics", EntryPoint="CSharp_PxVec3_Vector_size")]
  public static extern uint PxVec3_Vector_size(HandleRef jarg1);

  [DllImport("CGePhysics", EntryPoint="CSharp_PxVec3_Vector_capacity")]
  public static extern uint PxVec3_Vector_capacity(HandleRef jarg1);

  [DllImport("CGePhysics", EntryPoint="CSharp_PxVec3_Vector_reserve")]
  public static extern void PxVec3_Vector_reserve(HandleRef jarg1, uint jarg2);

  [DllImport("CGePhysics", EntryPoint="CSharp_new_PxVec3_Vector__SWIG_0")]
  public static extern IntPtr new_PxVec3_Vector__SWIG_0();

  [DllImport("CGePhysics", EntryPoint="CSharp_new_PxVec3_Vector__SWIG_1")]
  public static extern IntPtr new_PxVec3_Vector__SWIG_1(HandleRef jarg1);

  [DllImport("CGePhysics", EntryPoint="CSharp_new_PxVec3_Vector__SWIG_2")]
  public static extern IntPtr new_PxVec3_Vector__SWIG_2(int jarg1);

  [DllImport("CGePhysics", EntryPoint="CSharp_PxVec3_Vector_getitemcopy")]
  public static extern IntPtr PxVec3_Vector_getitemcopy(HandleRef jarg1, int jarg2);

  [DllImport("CGePhysics", EntryPoint="CSharp_PxVec3_Vector_getitem")]
  public static extern IntPtr PxVec3_Vector_getitem(HandleRef jarg1, int jarg2);

  [DllImport("CGePhysics", EntryPoint="CSharp_PxVec3_Vector_setitem")]
  public static extern void PxVec3_Vector_setitem(HandleRef jarg1, int jarg2, HandleRef jarg3);

  [DllImport("CGePhysics", EntryPoint="CSharp_PxVec3_Vector_AddRange")]
  public static extern void PxVec3_Vector_AddRange(HandleRef jarg1, HandleRef jarg2);

  [DllImport("CGePhysics", EntryPoint="CSharp_PxVec3_Vector_GetRange")]
  public static extern IntPtr PxVec3_Vector_GetRange(HandleRef jarg1, int jarg2, int jarg3);

  [DllImport("CGePhysics", EntryPoint="CSharp_PxVec3_Vector_Insert")]
  public static extern void PxVec3_Vector_Insert(HandleRef jarg1, int jarg2, HandleRef jarg3);

  [DllImport("CGePhysics", EntryPoint="CSharp_PxVec3_Vector_InsertRange")]
  public static extern void PxVec3_Vector_InsertRange(HandleRef jarg1, int jarg2, HandleRef jarg3);

  [DllImport("CGePhysics", EntryPoint="CSharp_PxVec3_Vector_RemoveAt")]
  public static extern void PxVec3_Vector_RemoveAt(HandleRef jarg1, int jarg2);

  [DllImport("CGePhysics", EntryPoint="CSharp_PxVec3_Vector_RemoveRange")]
  public static extern void PxVec3_Vector_RemoveRange(HandleRef jarg1, int jarg2, int jarg3);

  [DllImport("CGePhysics", EntryPoint="CSharp_PxVec3_Vector_Repeat")]
  public static extern IntPtr PxVec3_Vector_Repeat(HandleRef jarg1, int jarg2);

  [DllImport("CGePhysics", EntryPoint="CSharp_PxVec3_Vector_Reverse__SWIG_0")]
  public static extern void PxVec3_Vector_Reverse__SWIG_0(HandleRef jarg1);

  [DllImport("CGePhysics", EntryPoint="CSharp_PxVec3_Vector_Reverse__SWIG_1")]
  public static extern void PxVec3_Vector_Reverse__SWIG_1(HandleRef jarg1, int jarg2, int jarg3);

  [DllImport("CGePhysics", EntryPoint="CSharp_PxVec3_Vector_SetRange")]
  public static extern void PxVec3_Vector_SetRange(HandleRef jarg1, int jarg2, HandleRef jarg3);

  [DllImport("CGePhysics", EntryPoint="CSharp_delete_PxVec3_Vector")]
  public static extern void delete_PxVec3_Vector(HandleRef jarg1);
}
