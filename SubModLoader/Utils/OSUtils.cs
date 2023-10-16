using System;

namespace SubModLoader.Utils {
    /// <summary>
    /// Static utility class for getting OS info
    /// </summary>
    public static class OSUtils {
        /// <summary>
        /// Gets the name of the OS
        /// </summary>
        public static string GetOSName() {
            OperatingSystem os = Environment.OSVersion;

            return os.Platform switch {
                PlatformID.Win32S or PlatformID.Win32Windows or PlatformID.WinCE => "Old Windows",
                PlatformID.Win32NT => os.Version.Major switch {
                    < 5 => "Windows NT",
                    5 => os.Version.Minor switch {
                        0 => "Windows 2000",
                        1 => "Windows XP",
                        2 => "Windows 2003",
                        _ => os.VersionString
                    },
                    6 => os.Version.Major switch {
                        0 => "Windows Vista",
                        1 => "Windows 7",
                        2 => "Windows 8",
                        3 => "Windows 8.1",
                        _ => os.VersionString
                    },
                    10 => os.Version.Build switch {
                        < 22000 => "Windows 10",
                        _ => "Windows 11"
                    },
                    _ => os.VersionString
                },
                _ => os.VersionString
            };
        }
    }
}
