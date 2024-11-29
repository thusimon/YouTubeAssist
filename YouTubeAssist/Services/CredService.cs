using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace YouTubeAssist.Services
{
    internal class CredService
    {
        [DllImport("credui.dll", CharSet = CharSet.Unicode)]
        private static extern int CredUIPromptForWindowsCredentials(
            ref CREDUI_INFO credInfo,
            int authError,
            ref uint authPackage,
            IntPtr inAuthBuffer,
            uint inAuthBufferSize,
            out IntPtr outAuthBuffer,
            out uint outAuthBufferSize,
            ref bool save,
            uint flags);

        [DllImport("credui.dll", CharSet = CharSet.Unicode)]
        private static extern bool CredUnPackAuthenticationBuffer(
            uint flags,
            IntPtr authBuffer,
            uint authBufferSize,
            StringBuilder username,
            ref uint usernameSize,
            StringBuilder domainName,
            ref uint domainNameSize,
            StringBuilder password,
            ref uint passwordSize);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern void CredFree(IntPtr buffer);

        private const uint CREDUIWIN_GENERIC = 0x1;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct CREDUI_INFO
        {
            public int cbSize;
            public IntPtr hwndParent;
            public string pszMessageText;
            public string pszCaptionText;
            public IntPtr hbmBanner;
        }

        public static bool Authenticate()
        {
            var credInfo = new CREDUI_INFO
            {
                cbSize = Marshal.SizeOf(typeof(CREDUI_INFO)),
                hwndParent = IntPtr.Zero,
                pszMessageText = "Please authenticate with your Windows Hello credentials.",
                pszCaptionText = "Windows Hello Authentication",
                hbmBanner = IntPtr.Zero
            };

            uint authPackage = 0;
            IntPtr outAuthBuffer = IntPtr.Zero;
            uint outAuthBufferSize = 0;
            bool save = false;

            int result = CredUIPromptForWindowsCredentials(
                ref credInfo,
                0, // No specific error
                ref authPackage,
                IntPtr.Zero,
                0,
                out outAuthBuffer,
                out outAuthBufferSize,
                ref save,
                CREDUIWIN_GENERIC);

            if (result == 0) // ERROR_SUCCESS
            {
                var username = new StringBuilder(256);
                var password = new StringBuilder(256);
                uint usernameSize = 256, passwordSize = 256;
                var domainName = new StringBuilder(256);
                uint domainNameSize = 256;

                if (CredUnPackAuthenticationBuffer(
                    0,
                    outAuthBuffer,
                    outAuthBufferSize,
                    username,
                    ref usernameSize,
                    domainName,
                    ref domainNameSize,
                    password,
                    ref passwordSize))
                {
                    Debug.WriteLine($"Username: {username}");
                    Debug.WriteLine($"Domain: {domainName}");
                    Debug.WriteLine($"Password: {password}");
                }

                // Free the allocated authentication buffer
                CredFree(outAuthBuffer);
                return true;
            }
            else
            {
                Debug.WriteLine("Authentication was canceled or failed.");
                return false;
            }
        }
    }
}
