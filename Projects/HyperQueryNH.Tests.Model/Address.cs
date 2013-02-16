using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyperQueryNH.Tests.Model
{
    public class Address
    {
        public virtual string Address1 { get; set; }
        public virtual string Address2 { get; set; }
        public virtual string City { get; set; }
        public virtual string State { get; set; }
        public virtual string Zip { get; set; }
        public virtual string County { get; set; }
        public virtual string Country { get; set; }
    }
}
