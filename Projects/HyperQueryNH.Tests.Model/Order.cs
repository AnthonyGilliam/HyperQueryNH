using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyperQueryNH.Tests.Model
{
    public class Order : EntityBase
    {
		private IList<Product> _products;
		
		public Order()
		{
			_products = new List<Product>();
		}

		public virtual string OrderNumber { get; set; }
        public virtual DateTime OrderDate { get; set; }
        public virtual double TotalPrice 
        {
            get 
            { 
                double total = 0.00;

				total = _products.Aggregate(total, (ttl, prod) => ttl += prod.Price);
				total *= Customer != null
							? ((100 - Customer.CustomerDiscount) / 100)
							: 100;

            	return total;
            }
        }
        public virtual Customer Customer { get; set; }
    	public virtual IList<Product> Products
    	{
    		get { return _products; }
    		set { _products = value; }
    	}
    }
}
