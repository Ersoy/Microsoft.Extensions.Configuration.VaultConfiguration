using System;
using System.Runtime.InteropServices;
using System.Security;

namespace HashiCorp.Vault.Utilities {

    internal static class Utils {

        public static string ToUnicodeString(this SecureString secureString) {
            var ptr = IntPtr.Zero;

            try {
                ptr = Marshal.SecureStringToGlobalAllocUnicode(secureString);
                return Marshal.PtrToStringUni(ptr);
            }
            finally {
                Marshal.Release(ptr);
            }
        }

        public static SecureString ToSecureString(this string str) {
            var secureString = new SecureString();
            foreach (var l in str) {
                secureString.AppendChar(l);
            }
            return secureString;
        }

    }

}
