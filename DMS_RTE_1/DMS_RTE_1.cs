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

17/06/2024	1.0.0.1		SSU, Skyline	Initial version
****************************************************************************
*/

namespace DMS_RTE_1
{
	using System;
	using System.Collections.Generic;
	using Newtonsoft.Json;
	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Core.DataMinerSystem.Automation;
	using Skyline.DataMiner.Core.DataMinerSystem.Common;
	using Skyline.DataMiner.Net.Messages;

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
			IDms dms = engine.GetDms();
			var agents = dms.GetAgents();
			List<TestResult> openRteLi = new List<TestResult>();

			foreach (var agent in agents)
			{
				var rte = GetRteInfo(engine, agent.Id);
				TestResult openRtes = new TestResult()
				{
					ParameterName = "DMS 24 Hour RTEs",
					DmaName = agent.Name,
					ReceivedValue = rte["Rtes"],
				};

				openRteLi.Add(openRtes);
			}

			engine.AddScriptOutput("result", JsonConvert.SerializeObject(openRteLi));
		}

		private Dictionary<string, string> GetRteInfo(IEngine engine, int dma)
		{
			// Get RTEs and HF_RTEs
			Skyline.DataMiner.Net.Messages.ExecuteScriptMessage scriptRTEMessage = new ExecuteScriptMessage
			{
				DataMinerID = dma,// DMA ID
				ScriptName = "GetDmaRtes",
				Options = new SA(new[] { $"DEFER:{bool.FalseString}" }),
			};

			var response_RTE = engine.SendSLNetSingleResponseMessage(scriptRTEMessage) as ExecuteScriptResponseMessage;
			var scriptRTEResult = response_RTE?.ScriptOutput;
			return scriptRTEResult;
		}
	}
}
