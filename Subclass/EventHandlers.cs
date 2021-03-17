using Exiled.API.Features;
using Exiled.Events.EventArgs;
using System;
using MEC;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Exiled.API.Enums;

namespace Subclass
{
	partial class EventHandlers
	{
		private List<Subclass_c> subclasses;
		private List<Player> subclassPlayers = new List<Player>();

		internal void OnWaitingForPlayers()
		{
			subclasses = LoadSubclasses();
			subclassPlayers.Clear();
		}

		internal void OnRoundStart()
		{
			foreach (Subclass_c subclass in subclasses)
			{
				for (int i = 0; i < subclass.Count; i++)
				{
					if (UnityEngine.Random.Range(0, 100) <= subclass.SpawnChance)
					{
						// Gather candidates
						List<Player> candidates = Player.List.Where(x => x.Role == (RoleType)Enum.Parse(typeof(RoleType), subclass.Class)).ToList();
						if (candidates.Count > 0)
						{
							Player selected = candidates[UnityEngine.Random.Range(0, candidates.Count)];
							subclassPlayers.Add(selected);

							// Nickname
							selected.ReferenceHub.nicknameSync.ShownPlayerInfo &= ~PlayerInfoArea.Role;
							selected.CustomInfo = subclass.Nickname;

							// Health
							selected.MaxHealth = subclass.Health;
							selected.Health = subclass.Health;

							// Spawn
							if (subclass.Spawn != null)
							{
								Room room = Map.Rooms.FirstOrDefault(x => x.Type == (RoomType)Enum.Parse(typeof(RoomType), subclass.Spawn));
								if (room != null)
								{
									Vector3 pos = room.transform.position;
									pos.y += 2;
									Timing.CallDelayed(0.5f, () => selected.Position = pos);
								}
								else
								{
									Log.Error($"Invalid spawnpoint '{subclass.Spawn}'!");
								}
							}

							// Equipment
							selected.Inventory.items.Clear();
							foreach (string e in subclass.Equipment)
							{
								selected.AddItem((ItemType)Enum.Parse(typeof(ItemType), e));
							}

							// Broadcast
							selected.Broadcast((ushort)subclass.BroadcastTime, subclass.Broadcast);
						}
					}
				}
			}
		}

		internal void OnPlayerDeath(DiedEventArgs ev)
		{
			if (subclassPlayers.Contains(ev.Target))
			{
				KillSubclassPlayer(ev.Target);
			}
		}

		internal void OnDisconnect(LeftEventArgs ev)
		{
			if (subclassPlayers.Contains(ev.Player))
			{
				KillSubclassPlayer(ev.Player);
			}
		}

		internal void OnSetRole(ChangingRoleEventArgs ev)
		{
			if (subclassPlayers.Contains(ev.Player))
			{
				KillSubclassPlayer(ev.Player);
			}
		}
	}
}
