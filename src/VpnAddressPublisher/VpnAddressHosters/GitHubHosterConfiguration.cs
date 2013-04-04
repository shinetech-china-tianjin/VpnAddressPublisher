using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace VpnAddressPublisher
{
    internal class GitHubHosterConfiguration
    {
        static GitHubHosterConfiguration() {
            GitHubUserName          = ReadConfiguration("GitHubUserName");
            GitHubPassword          = ReadConfiguration("GitHubPassword");
            HosterRepository        = ReadConfiguration("HosterRepository");
            HosterBranch            = ReadConfiguration("HosterBranch");
            HosterVpnAddressFile    = ReadConfiguration("HosterVpnAddressFile");
            CommitterName           = ReadConfiguration("CommitterName");
            CommitterEmail          = ReadConfiguration("CommitterEmail");
            LocalRepositoryName     = ReadConfiguration("LocalRepositoryName");
        }

        private static string ReadConfiguration(string keyName) {
            return ConfigurationManager.AppSettings[keyName];
        }

        internal static string GitHubUserName       { get; private set; }
        internal static string GitHubPassword       { get; private set; }
        internal static string HosterRepository     { get; private set; }
        internal static string HosterBranch         { get; private set; }
        internal static string HosterVpnAddressFile { get; private set; }
        internal static string CommitterName        { get; private set; }
        internal static string CommitterEmail       { get; private set; }
        internal static string LocalRepositoryName  { get; private set; }
    }
}
