using System;

namespace FirewallUtilities
{
    public class Enumerations
    {
        [Flags]
        public enum Action
        {
            NotConfigured = 0,
            Allow = 1,
            Block = 2
        }
        [Flags]
        public enum Enabled
        {
            Enabled = 1,
            Disabled = 2
        }
        [Flags]
        public enum Authentication
        {
            NotConfigured = 0,
            Required = 1,
            NoEncap = 2
        }
        [Flags]
        public enum Direction
        {
            Inbound = 1,
            Outbound = 2
        }

        [Flags]
        public enum EdgeTraversalPolicy
        {
            Block = 0,
            Allow = 1,
            DeferToUser = 2,
            DeferToApp = 4
        }
        [Flags]
        public enum Encryption
        {
            NotConfigured = 0,
            Required = 1,
            Dynamic = 2
        }
        [Flags]
        public enum PolicyStoreSourceType
        {
            Local = 0,
            GroupPolicy = 1,
            Dynamic = 2,
            Generated = 4,
            Hardcoded = 8
        }
        [Flags]
        public enum PrimaryStatus
        {
            Unknown = 0,
            OK = 1,
            Inactive = 2,
            Error = 4
        }
        [Flags]
        public enum Protocol
        {
            Any,
            TCP,
            UDP
        }

        [Flags]
        public enum InterfaceType
        {
            Any = 0,
            Wired = 1,
            Wireless = 2,
            RemoteAccess = 4
        }

        [Flags]
        public enum PolicyStore
        {
            Any = 0,
            Domain = 1,
            Private = 2,
            Public = 4,
            NotApplicable = 8
        }


        [Flags]
        public enum GpoBoolen
        {
            False = 0,
            True = 1,
            NotConfigured = 2,
        }
    }
}
