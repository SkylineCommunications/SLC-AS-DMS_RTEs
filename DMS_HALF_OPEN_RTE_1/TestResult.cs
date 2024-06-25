using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMS_HALF_OPEN_RTE_1
{
	public class TestResult
	{
		public string ParameterName { get; set; }

		public string DisplayName { get; set; }

		public string ElementName { get; set; }

		public string DmaName { get; set; }

		public string ReceivedValue { get; set; }

		public string ExpectedValue { get; set; }

		public bool Success { get; set; }
	}
}
