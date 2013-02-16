using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyperQueryNH.Tests.Model
{
    public class Product : EntityBase
    {
        public virtual string Name { get; set; }
        public virtual string Manufacturer { get; set; }
        public virtual string Model { get; set; }
        public virtual int Year { get; set; }
        public virtual double Price { get; set; }
        public virtual Category Catagory { get; set; }
    }
}
