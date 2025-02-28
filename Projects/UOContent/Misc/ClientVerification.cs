using System;
using Server.Buffers;
using Server.Gumps;
using Server.Logging;
using Server.Mobiles;
using Server.Network;

namespace Server.Misc
{
    public static class ClientVerification
    {
        private static readonly ILogger logger = LogFactory.GetLogger(typeof(ClientVerification));

        private static bool _enable;
        private static InvalidClientResponse _invalidClientResponse;
        private static string _versionExpression;

        private static TimeSpan _ageLeniency;
        private static TimeSpan _gameTimeLeniency;

        public static ClientVersion MinRequired { get; private set; }
        public static ClientVersion MaxRequired { get; private set; }

        public static bool AllowRegular => true;
        public static bool AllowUOTD => false;
        public static TimeSpan KickDelay { get; private set; }

        public static void Configure()
        {
            MinRequired = ServerConfiguration.GetSetting("clientVerification.minRequired", (ClientVersion)null);
            MaxRequired = ServerConfiguration.GetSetting("clientVerification.maxRequired", (ClientVersion)null);

            _enable = ServerConfiguration.GetOrUpdateSetting("clientVerification.enable", true);
            _invalidClientResponse =
                ServerConfiguration.GetOrUpdateSetting("clientVerification.invalidClientResponse", InvalidClientResponse.Kick);
            _ageLeniency = ServerConfiguration.GetOrUpdateSetting("clientVerification.ageLeniency", TimeSpan.FromDays(10));
            _gameTimeLeniency = ServerConfiguration.GetOrUpdateSetting(
                "clientVerification.gameTimeLeniency",
                TimeSpan.FromHours(25)
            );
            KickDelay = ServerConfiguration.GetOrUpdateSetting("clientVerification.kickDelay", TimeSpan.FromSeconds(20.0));
        }

        public static void Initialize()
        {
            EventSink.ClientVersionReceived += EventSink_ClientVersionReceived;

            if (MinRequired == null && MaxRequired == null)
            {
                MinRequired = UOClient.ServerClientVersion;
            }

            if (MinRequired != null || MaxRequired != null)
            {
                logger.Information(
                    "Restricting client version to {ClientVersion}. Action to be taken: {Action}",
                    GetVersionExpression(),
                    _invalidClientResponse
                );
            }
        }

        private static string GetVersionExpression()
        {
            if (_versionExpression == null)
            {
                if (MinRequired != null && MaxRequired != null)
                {
                    _versionExpression = $"{MinRequired}-{MaxRequired}";
                }
                else if (MinRequired != null)
                {
                    _versionExpression = $"{MinRequired} or newer";
                }
                else
                {
                    _versionExpression = $"{MaxRequired} or older";
                }
            }

            return _versionExpression;
        }

        private static void EventSink_ClientVersionReceived(NetState state, ClientVersion version)
        {
            using var message = ValueStringBuilder.Create();

            if (!_enable || state.Mobile?.AccessLevel != AccessLevel.Player)
            {
                return;
            }

            var strictRequirement = _invalidClientResponse == InvalidClientResponse.Kick ||
                                    _invalidClientResponse == InvalidClientResponse.LenientKick &&
                                    Core.Now - state.Mobile.Created > _ageLeniency &&
                                    state.Mobile is PlayerMobile mobile &&
                                    mobile.GameTime > _gameTimeLeniency;

            bool shouldKick = false;

            if (MinRequired != null && version < MinRequired)
            {
                message.Append($"This server doesn't support clients older than {MinRequired}.");
                shouldKick = strictRequirement;
            }
            else if (MaxRequired != null && version > MaxRequired)
            {
                message.Append($"This server doesn't support clients newer than {MaxRequired}.");
                shouldKick = strictRequirement;
            }
            else if (!AllowRegular || !AllowUOTD)
            {
                if (!AllowRegular && version.Type == ClientType.Regular)
                {
                    message.Append("This server does not allow regular clients to connect.");
                    shouldKick = true;
                }
                else if (!AllowUOTD && state.IsUOTDClient)
                {
                    message.Append("This server does not allow UO:TD clients to connect.");
                    shouldKick = true;
                }

                if (message.Length > 0)
                {
                    if (AllowRegular && AllowUOTD)
                    {
                        message.Append(" You can use regular or UO:TD clients.");
                    }
                    else if (AllowRegular)
                    {
                        message.Append(" You can use regular clients.");
                    }
                    else if (AllowUOTD)
                    {
                        message.Append(" You can use UO:TD clients.");
                    }
                }
            }

            if (message.Length > 0)
            {
                state.Mobile.SendMessage(0x22, message.ToString());
            }

            if (shouldKick)
            {
                state.Mobile.SendMessage(0x22, "You will be disconnected in {0} seconds.", KickDelay.TotalSeconds);
                Timer.StartTimer(KickDelay, () => OnKick(state));
                return;
            }

            if (message.Length > 0)
            {
                switch (_invalidClientResponse)
                {
                    case InvalidClientResponse.Warn:
                        {
                            state.Mobile.SendMessage(
                                0x22,
                                $"This server recommends that your client version is {GetVersionExpression()}."
                            );
                            break;
                        }
                    case InvalidClientResponse.LenientKick:
                    case InvalidClientResponse.Annoy:
                        {
                            SendAnnoyGump(state.Mobile);
                            break;
                        }
                }
            }
        }

        private static void OnKick(NetState ns)
        {
            if (ns.Running)
            {
                var version = ns.Version;
                ns.LogInfo($"Disconnecting, bad version ({version})");
                ns.Disconnect($"Invalid client version {version}.");
            }
        }

        private static void KickMessage(Mobile from, bool okay)
        {
            from.SendMessage("You will be reminded of this again.");

            if (_invalidClientResponse == InvalidClientResponse.LenientKick)
            {
                from.SendMessage(
                    "Invalid clients will be kicked after {0} days of character age and {1} hours of play time",
                    _ageLeniency,
                    _gameTimeLeniency
                );
            }

            Timer.StartTimer(TimeSpan.FromMinutes(Utility.Random(5, 15)), () => SendAnnoyGump(from));
        }

        private static void SendAnnoyGump(Mobile m)
        {
            if (m.NetState != null)
            {
                Gump g = new WarningGump(
                    1060637,
                    30720,
                    $"Your client is invalid.<br>This server recommends that your client version is {GetVersionExpression()}.<br> <br>You are currently using version {m.NetState.Version}.",
                    0xFFC000,
                    480,
                    360,
                    okay => KickMessage(m, okay),
                    false
                )
                {
                    Draggable = false,
                    Closable = false,
                    Resizable = false,
                };

                m.SendGump(g);
            }
        }

        private enum InvalidClientResponse
        {
            Ignore,
            Warn,
            Annoy,
            LenientKick,
            Kick
        }
    }
}
