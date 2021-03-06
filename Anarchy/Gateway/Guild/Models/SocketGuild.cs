﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Discord.Gateway
{
    public class SocketGuild : DiscordGuild, IDisposable
    {
        public SocketGuild()
        {
            OnClientUpdated += (sender, e) => Channels.SetClientsInList(Client);
            JsonUpdated += (sender, json) =>
            {
                if (!Unavailable)
                    _channels = json.Value<JArray>("channels").PopulateListJson<GuildChannel>();
            };
        }


        [JsonProperty("large")]
        public bool Large { get; private set; }


        [JsonProperty("member_count")]
        public uint MemberCount { get; private set; }


        internal List<GuildChannel> _channels;
        [JsonIgnore]
        public IReadOnlyList<GuildChannel> Channels
        {
            get
            {
                if (!Unavailable)
                {
                    foreach (var channel in _channels)
                    {
                        channel.GuildId = Id;
                        channel.Json["guild_id"] = Id;
                    }
                }

                return _channels;
            }
        }


        [JsonProperty("joined_at")]
#pragma warning disable CS0649
        private string _joinedAt;
#pragma warning restore CS0659
        public DateTime JoinedAt
        {
            get { return DiscordTimestamp.FromString(_joinedAt); }
        }


        [JsonProperty("voice_states")]
        internal List<DiscordVoiceState> _voiceStates;

        public IReadOnlyList<DiscordVoiceState> VoiceStates
        {
            get
            {
                foreach (var state in _voiceStates)
                    state.Guild = this;

                return _voiceStates;
            }
        }


        public IReadOnlyList<GuildMember> GetMembers()
        {
            return ((DiscordSocketClient)Client).GetAllGuildMembers(Id);
        }


        /// <summary>
        /// Gets the guild's channels
        /// </summary>
        public override IReadOnlyList<GuildChannel> GetChannels()
        {
            var channels = base.GetChannels();
            _channels = channels.ToList();
            return channels;
        }


        public new void Dispose()
        {
            base.Dispose();
            _channels = null;
            _joinedAt = null;
            _voiceStates = null;
        }
    }
}
