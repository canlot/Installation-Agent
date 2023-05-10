using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Installation.Models.Helpers;

namespace Installation.Models.Notify
{
    public class Notify<T>
    {
		private Guid endpointId = new Guid().Empty();

		public Guid EndpointId
        {
			get { return endpointId; }
			set { endpointId = value; }
		}

		public T Object { get; set; }

    }
}
