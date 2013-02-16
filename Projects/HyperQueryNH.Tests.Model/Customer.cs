using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyperQueryNH.Tests.Model
{
    public class Customer : EntityBase
    {
        public virtual string FirstName { get; set; }
        public virtual char MiddleInitial { get; set; }
        public virtual string LastName { get; set; }
        public virtual string FullName 
		{
			get { return string.Format("{0} {1}. {2}", FirstName, MiddleInitial, LastName); }
        }
        public virtual Address Address { get; set; }
        public virtual string WebAddress { get; set; }
        public virtual string Email { get; set; }
        public virtual string Establishment { get; set; }
        public virtual double CustomerDiscount { get; set; }
        public virtual DateTime DateSubscribed { get; set; }
        public virtual IList<Order> Orders { get; set; }
    }
}
