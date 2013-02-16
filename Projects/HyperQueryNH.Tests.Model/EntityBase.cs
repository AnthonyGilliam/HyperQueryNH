using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyperQueryNH.Tests.Model
{
	public abstract class EntityBase
	{
		public virtual Guid ID { get; set; }
		public virtual DateTime DateOfCreation { get; set; }
	}
}
