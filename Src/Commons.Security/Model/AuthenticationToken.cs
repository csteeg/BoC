using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BoC.Persistence;

namespace BoC.Security.Model
{
	public class AuthenticationToken : BaseEntity<long>
	{
		public virtual User User { get; set; }
		public virtual string ClaimedIdentifier { get; set; }
		public virtual string FriendlyIdentifier { get; set; }
		public virtual DateTime LastUsed { get; set; }
	}
}
