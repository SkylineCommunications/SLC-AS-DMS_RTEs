/*
****************************************************************************
*  Copyright (c) 2024,  Skyline Communications NV  All Rights Reserved.    *
****************************************************************************

By using this script, you expressly agree with the usage terms and
conditions set out below.
This script and all related materials are protected by copyrights and
other intellectual property rights that exclusively belong
to Skyline Communications.

A user license granted for this script is strictly for personal use only.
This script may not be used in any way by anyone without the prior
written consent of Skyline Communications. Any sublicensing of this
script is forbidden.

Any modifications to this script by the user are only allowed for
personal use and within the intended purpose of the script,
and will remain the sole responsibility of the user.
Skyline Communications will not be responsible for any damages or
malfunctions whatsoever of the script resulting from a modification
or adaptation by the user.

The content of this script is confidential information.
The user hereby agrees to keep this confidential information strictly
secret and confidential and not to disclose or reveal it, in whole
or in part, directly or indirectly to any person, entity, organization
or administration without the prior written consent of
Skyline Communications.

Any inquiries can be addressed to:

	Skyline Communications NV
	Ambachtenstraat 33
	B-8870 Izegem
	Belgium
	Tel.	: +32 51 31 35 69
	Fax.	: +32 51 31 01 29
	E-mail	: info@skyline.be
	Web		: www.skyline.be
	Contact	: Ben Vandenberghe

****************************************************************************
Revision History:

DATE		VERSION		AUTHOR			COMMENTS

dd/mm/2024	1.0.0.1		XXX, Skyline	Initial version
****************************************************************************
*/

namespace GetDmaRtes_1
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using Skyline.DataMiner.Automation;

	/// <summary>
	/// Represents a DataMiner Automation script.
	/// </summary>
	public class Script
	{
		/// <summary>
		/// The script entry point.
		/// </summary>
		/// <param name="engine">Link with SLAutomation process.</param>
		public void Run(IEngine engine)
		{
			try
			{
				RunSafe(engine);
			}
			catch (ScriptAbortException)
			{
				// Catch normal abort exceptions (engine.ExitFail or engine.ExitSuccess)
				throw; // Comment if it should be treated as a normal exit of the script.
			}
			catch (ScriptForceAbortException)
			{
				// Catch forced abort exceptions, caused via external maintenance messages.
				throw;
			}
			catch (ScriptTimeoutException)
			{
				// Catch timeout exceptions for when a script has been running for too long.
				throw;
			}
			catch (InteractiveUserDetachedException)
			{
				// Catch a user detaching from the interactive script by closing the window.
				// Only applicable for interactive scripts, can be removed for non-interactive scripts.
				throw;
			}
			catch (Exception e)
			{
				engine.ExitFail("Run|Something went wrong: " + e);
			}
		}

		private void RunSafe(IEngine engine)
		{
			List<string> rtelineList = GetRTEs("OPEN RTE: Thread problem", engine);
			string string_numOfRTEs = rtelineList.Last();
			int numOfRTEs = int.Parse(string_numOfRTEs);
			engine.AddScriptOutput("Rtes", numOfRTEs.ToString());
			engine.AddScriptOutput(
				$"LineOfRTEs",
				string.Join("\n", rtelineList.Where(x => !string.IsNullOrEmpty(x))));

			List<string> hf_rtelineList = GetRTEs("HALFOPEN RTE: -", engine);
			string string_numOfHFRTEs = hf_rtelineList.Last();
			int numOfHFRTEs = int.Parse(string_numOfHFRTEs);

			engine.AddScriptOutput("HalfOpenRtes", numOfHFRTEs.ToString());

			engine.AddScriptOutput(
				$"LineOfHalfOpenRtes",
				string.Join("\n", hf_rtelineList.Where(xhf => !string.IsNullOrEmpty(xhf))));
		}

		public static List<string> GetRTEs(string isRTEorHF, IEngine engine)
		{
			string logFile = @"C:\Skyline DataMiner\logging\SLWatchdog2.txt";
			List<string> saveRTEline;
			saveRTEline = new List<string>();
			int rteCount = 0;
			using (Stream stream = File.Open(logFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			using (StreamReader sr = new StreamReader(stream))
			{
				DateTime endDate = DateTime.Now;
				DateTime startDate = endDate.AddDays(-1);
				while (!sr.EndOfStream)
				{
					string line = sr.ReadLine();
					if (line.Contains(isRTEorHF) && DateTime.TryParse(line.Substring(0, 19), out DateTime dateTime) && dateTime >= startDate && dateTime <= endDate)
					{
						rteCount++;
						saveRTEline.Add(line);
					}
				}
			}

			saveRTEline.Add(rteCount.ToString());

			return saveRTEline;
		}
	}
}