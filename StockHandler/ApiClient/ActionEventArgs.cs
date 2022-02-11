using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiClient
{
	public class ActionEventArgs : EventArgs
	{
		public string Message { get; }

		public ActionEventArgs(string message)
		{
			Message = message;
		}
	}
}
