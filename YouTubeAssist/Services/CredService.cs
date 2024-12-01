using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Windows.Security.Credentials.UI;

namespace YouTubeAssist.Services
{
    internal class CredService
    {
        private async Task<bool> IsWindowsHelloAvailableAsync()
        {
            try
            {
                // Check if Windows Hello is available
                var availabilityInfo = await UserConsentVerifier.CheckAvailabilityAsync();
                return availabilityInfo == UserConsentVerifierAvailability.Available;
            }
            catch
            {
                return false;
            }
        }

        private async Task<bool> PerformWindowsHelloAuthenticationAsync()
        {
            try
            {
                // Prompt for Windows Hello authentication
                var verificationResult = await UserConsentVerifier.RequestVerificationAsync("WebExt needs to verify your identity using Windows Hello");

                // Map the verification result to a boolean
                switch (verificationResult)
                {
                    case UserConsentVerificationResult.Verified:
                        return true;
                    case UserConsentVerificationResult.DeviceNotPresent:
                    case UserConsentVerificationResult.Canceled:
                    case UserConsentVerificationResult.DisabledByPolicy:
                    default:
                        return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> Authenticate()
        {
            try
            {
                // Check if Windows Hello is available
                if (!await IsWindowsHelloAvailableAsync())
                {
                    Debug.WriteLine("Windows Hello is not avaliable");
                    return false;
                }

                // Perform Windows Hello authentication
                var authResult = await PerformWindowsHelloAuthenticationAsync();

                return authResult;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Windows Hello error {ex.Message}");
                return false;
            }
        }
    }
}
