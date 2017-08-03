using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Runtime;
using System.Security;
using System.Globalization;

namespace WebBrowserUtils.ExtendWebBrowser.NativeMethods
{

    [StructLayout(LayoutKind.Explicit)]
    internal struct HRESULT
    {
        [FieldOffset(0)]
        private readonly uint _value;
        public static readonly HRESULT S_OK = new HRESULT(0u);
        public static readonly HRESULT S_FALSE = new HRESULT(1u);
        public static readonly HRESULT E_NOTIMPL = new HRESULT(2147500033u);
        public static readonly HRESULT E_NOINTERFACE = new HRESULT(2147500034u);
        public static readonly HRESULT E_POINTER = new HRESULT(2147500035u);
        public static readonly HRESULT E_ABORT = new HRESULT(2147500036u);
        public static readonly HRESULT E_FAIL = new HRESULT(2147500037u);
        public static readonly HRESULT E_UNEXPECTED = new HRESULT(2147549183u);
        public static readonly HRESULT DISP_E_MEMBERNOTFOUND = new HRESULT(2147614723u);
        public static readonly HRESULT DISP_E_TYPEMISMATCH = new HRESULT(2147614725u);
        public static readonly HRESULT DISP_E_UNKNOWNNAME = new HRESULT(2147614726u);
        public static readonly HRESULT DISP_E_EXCEPTION = new HRESULT(2147614729u);
        public static readonly HRESULT DISP_E_OVERFLOW = new HRESULT(2147614730u);
        public static readonly HRESULT DISP_E_BADINDEX = new HRESULT(2147614731u);
        public static readonly HRESULT DISP_E_BADPARAMCOUNT = new HRESULT(2147614734u);
        public static readonly HRESULT DISP_E_PARAMNOTOPTIONAL = new HRESULT(2147614735u);
        public static readonly HRESULT SCRIPT_E_REPORTED = new HRESULT(2147614977u);
        public static readonly HRESULT STG_E_INVALIDFUNCTION = new HRESULT(2147680257u);
        public static readonly HRESULT DESTS_E_NO_MATCHING_ASSOC_HANDLER = new HRESULT(2147749635u);
        public static readonly HRESULT E_ACCESSDENIED = new HRESULT(2147942405u);
        public static readonly HRESULT E_OUTOFMEMORY = new HRESULT(2147942414u);
        public static readonly HRESULT E_INVALIDARG = new HRESULT(2147942487u);
        public static readonly HRESULT COR_E_OBJECTDISPOSED = new HRESULT(2148734498u);
        public static readonly HRESULT WC_E_GREATERTHAN = new HRESULT(3222072867u);
        public static readonly HRESULT WC_E_SYNTAX = new HRESULT(3222072877u);
        public Facility Facility
        {
            get
            {
                return HRESULT.GetFacility((int)this._value);
            }
        }
        public int Code
        {
            get
            {
                return HRESULT.GetCode((int)this._value);
            }
        }
        public bool Succeeded
        {
            get
            {
                return this._value >= 0u;
            }
        }
        public bool Failed
        {
            get
            {
                return this._value < 0u;
            }
        }
        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        public HRESULT(uint i)
        {
            this._value = i;
        }
        public static HRESULT Make(bool severe, Facility facility, int code)
        {
            return new HRESULT((uint)((severe ? ((Facility)(-2147483648)) : Facility.Null) | (Facility)((int)facility << 16) | (Facility)code));
        }
        public static Facility GetFacility(int errorCode)
        {
            return (Facility)(errorCode >> 16 & 8191);
        }
        public static int GetCode(int error)
        {
            return error & 65535;
        }
        public override string ToString()
        {
            FieldInfo[] fields = typeof(HRESULT).GetFields(BindingFlags.Static | BindingFlags.Public);
            for (int i = 0; i < fields.Length; i++)
            {
                FieldInfo fieldInfo = fields[i];
                if (fieldInfo.FieldType == typeof(HRESULT))
                {
                    HRESULT hrLeft = (HRESULT)fieldInfo.GetValue(null);
                    if (hrLeft == this)
                    {
                        string result = fieldInfo.Name;
                        return result;
                    }
                }
            }
            if (this.Facility == Facility.Win32)
            {
                FieldInfo[] fields2 = typeof(Win32Error).GetFields(BindingFlags.Static | BindingFlags.Public);
                for (int j = 0; j < fields2.Length; j++)
                {
                    FieldInfo fieldInfo2 = fields2[j];
                    if (fieldInfo2.FieldType == typeof(Win32Error))
                    {
                        Win32Error error = (Win32Error)fieldInfo2.GetValue(null);
                        if ((HRESULT)error == this)
                        {
                            string result = "HRESULT_FROM_WIN32(" + fieldInfo2.Name + ")";
                            return result;
                        }
                    }
                }
            }
            return string.Format(CultureInfo.InvariantCulture, "0x{0:X8}", new object[]
			{
				this._value
			});
        }
        public override bool Equals(object obj)
        {
            bool result;
            try
            {
                result = (((HRESULT)obj)._value == this._value);
            }
            catch (InvalidCastException)
            {
                result = false;
            }
            return result;
        }
        public override int GetHashCode()
        {
            return this._value.GetHashCode();
        }
        public static bool operator ==(HRESULT hrLeft, HRESULT hrRight)
        {
            return hrLeft._value == hrRight._value;
        }
        public static bool operator !=(HRESULT hrLeft, HRESULT hrRight)
        {
            return !(hrLeft == hrRight);
        }
        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        public void ThrowIfFailed()
        {
            this.ThrowIfFailed(null);
        }
        [SecurityCritical, SecurityTreatAsSafe]
        public void ThrowIfFailed(string message)
        {
            Exception exception = this.GetException(message);
            if (exception != null)
            {
                throw exception;
            }
        }
        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        public Exception GetException()
        {
            return this.GetException(null);
        }
        [SecurityCritical, SecurityTreatAsSafe]
        public Exception GetException(string message)
        {
            if (!this.Failed)
            {
                return null;
            }
            Exception ex = Marshal.GetExceptionForHR((int)this._value, new IntPtr(-1));
            if (ex.GetType() == typeof(COMException))
            {
                Facility facility = this.Facility;
                if (facility == Facility.Win32)
                {
                    if (string.IsNullOrEmpty(message))
                    {
                        ex = new Win32Exception(this.Code);
                    }
                    else
                    {
                        ex = new Win32Exception(this.Code, message);
                    }
                }
                else
                {
                    ex = new COMException(message ?? ex.Message, (int)this._value);
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(message))
                {
                    ConstructorInfo constructor = ex.GetType().GetConstructor(new Type[]
					{
						typeof(string)
					});
                    if (null != constructor)
                    {
                        ex = (constructor.Invoke(new object[]
						{
							message
						}) as Exception);
                    }
                }
            }
            return ex;
        }
    }
}
