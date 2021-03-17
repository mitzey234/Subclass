using Exiled.API.Features;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace Subclass
{
	partial class EventHandlers
	{
		private List<Subclass_c> LoadSubclasses()
		{
			List<Subclass_c> list = new List<Subclass_c>();

			foreach (string file in Directory.GetFiles(Subclass.SubclassFilePath))
			{
				if (file.Contains(".json"))
				{
					Subclass_c subclass = null;
					try
					{
						subclass = JsonConvert.DeserializeObject<Subclass_c>(File.ReadAllText(file));
					}
					catch (Exception x)
					{
						Log.Error($"Error parsing subclass '{Path.GetFileName(file)}', make sure your data types are correct!");
					}
					if (subclass != null)
					{
						if (!Enum.IsDefined(typeof(RoleType), subclass.Class))
						{
							Log.Error($"Error parsing class '{subclass.Class}' in subclass '{Path.GetFileName(file)}', subclass will not be loaded.");
							continue;
						}

						bool isValid = true;
						foreach (string e in subclass.Equipment)
						{
							if (!Enum.IsDefined(typeof(ItemType), e))
							{
								Log.Error($"Error parsing equipment '{e}' in subclass '{Path.GetFileName(file)}', subclass will not be loaded.");
								isValid = false;
							}
						}
						if (!isValid) continue;

						list.Add(subclass);
						Log.Info($"Successfully loaded subclass '{Path.GetFileName(file)}'!");
					}
				}
			}
			return list;
		}

		private void KillSubclassPlayer(Player player)
		{
			player.CustomInfo = string.Empty;
			player.ReferenceHub.nicknameSync.ShownPlayerInfo |= PlayerInfoArea.Role;
		}
	}
}
