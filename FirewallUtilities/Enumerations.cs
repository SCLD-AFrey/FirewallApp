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
        public enum Profile
        {
            Any = 0, Domain = 1, Private = 2, Public = 4, NotApplicable = 8
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
        public enum GpoBoolean
        {
            False = 0,
            True = 1,
            NotConfigured = 2,
        }
        [Flags]
        public enum IPsecThroughNAT
        {
            None = 0, Server = 1, Both = 2, NotConfigured = 4
        }

        [Flags]
        public enum CRLCheck
        {
            None = 0, AttemptCrlCheck = 1, RequireCrlCheck = 2, NotConfigured = 4
        }
        public enum PacketQueuing
        {
            None = 0, Inbound = 1, Forward = 2, NotConfigured = 4
        }
        public enum TrafficExemption
        {
            None = 0, NeighborDiscovery = 1, Icmp = 2, RouterDiscovery = 4, Dhcp = 8, NotConfigured = 16
        }
        public enum KeyEncoding
        {
            UTF16 = 0, UTF8 = 1, NotConfigured = 2
        }
    }
}
