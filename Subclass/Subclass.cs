using Exiled.API.Features;
using System;
using System.IO;

namespace Subclass
{
	class Subclass : Plugin<Config>
	{
		internal static string SubclassFilePath = Path.Combine(Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "EXILED"), "Plugins"), "Subclass");

		private EventHandlers ev;

		public override void OnEnabled()
		{
			base.OnEnabled();

			VerifyFilesystem();

			ev = new EventHandlers();

			Exiled.Events.Handlers.Server.WaitingForPlayers += ev.OnWaitingForPlayers;
			Exiled.Events.Handlers.Server.RoundStarted += ev.OnRoundStart;
			Exiled.Events.Handlers.Player.Died += ev.OnPlayerDeath;
			Exiled.Events.Handlers.Player.Left += ev.OnDisconnect;
			Exiled.Events.Handlers.Player.ChangingRole += ev.OnSetRole;
		}

		public override void OnDisabled()
		{
			base.OnDisabled();

			Exiled.Events.Handlers.Server.WaitingForPlayers -= ev.OnWaitingForPlayers;
			Exiled.Events.Handlers.Server.RoundStarted -= ev.OnRoundStart;
			Exiled.Events.Handlers.Player.Died -= ev.OnPlayerDeath;
			Exiled.Events.Handlers.Player.Left -= ev.OnDisconnect;
			Exiled.Events.Handlers.Player.ChangingRole -= ev.OnSetRole;

			ev = null;
		}

		private void VerifyFilesystem()
		{
			if (!Directory.Exists(SubclassFilePath))
			{
				Directory.CreateDirectory(SubclassFilePath);
			}
		}

		public override string Author => "Cyanox";
	}
}
