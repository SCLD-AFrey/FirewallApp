using System;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Data.Filtering;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
namespace FirewallApp.Data
{

    public partial class FirewallRule
    {
        public FirewallRule(Session session) : base(session) { }
        public override void AfterConstruction() { base.AfterConstruction(); }
    }

}
