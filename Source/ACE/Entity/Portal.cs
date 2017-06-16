﻿using System;
using ACE.Network.GameEvent.Events;
using ACE.Network.GameMessages.Messages;
using ACE.Entity.Enum;
using ACE.Network.Enum;
using ACE.Entity.Enum.Properties;

namespace ACE.Entity
{
    public sealed class Portal : CollidableObject
    {
        // private readonly Position portalDestination;

        public Position Destination { get; private set; }

        // private byte portalSocietyId;

        private enum SpecialPortalWCID : ushort
        {
            /// <summary>
            /// Training Academy's Central Courtyard's portal weenieClassID
            /// </summary>
            CentralCourtyard = 31061,

            /// <summary>
            /// Training Academy's Outer Courtyard's portal weenieClassID
            /// </summary>
            OuterCourtyard = 29334
        }

        private enum SpecialPortalLandblockID : uint
        {
            /// <summary>
            /// Shoushi :: Training Academy's Central Courtyard's portal raw LandblockID
            /// </summary>
            ShoushiCCLaunch = 0x7f030273,

            /// <summary>
            /// Shoushi :: Training Academy's Central Courtyard's portal destination raw LandblockID
            /// </summary>
            ShoushiCCLanding = 0x7f03021e,

            /// <summary>
            /// Yaraq :: Training Academy's Central Courtyard's portal raw LandblockID
            /// </summary>
            YaraqCCLaunch = 0x8c040273,

            /// <summary>
            /// Yaraq :: Training Academy's Central Courtyard's portal destination raw LandblockID
            /// </summary>
            YaraqCCLanding = 0x8c04021e,

            /// <summary>
            /// Sanamar :: Training Academy's Central Courtyard's portal raw LandblockID
            /// </summary>
            SanamarCCLaunch = 0x72030273,

            /// <summary>
            /// Sanamar :: Training Academy's Central Courtyard's portal destination raw LandblockID
            /// </summary>
            SanamarCCLanding = 0x7203021e,

            /// <summary>
            /// Holtburg :: Training Academy's Central Courtyard's portal destination raw LandblockID
            /// </summary>
            HoltburgCCLanding = 0x8603021e,

            /// <summary>
            /// Shoushi :: Training Academy's Outer Courtyard's portal raw LandblockID
            /// </summary>
            ShoushiOCLaunch = 0x7f030331,

            /// <summary>
            /// Shoushi :: Training Academy's Outer Courtyard's portal destination raw LandblockID
            /// </summary>
            ShoushiOCLanding = 0x7f0302c3,

            /// <summary>
            /// Yaraq :: Training Academy's Outer Courtyard's portal raw LandblockID
            /// </summary>
            YaraqOCLaunch = 0x8c040331,

            /// <summary>
            /// Yaraq :: Training Academy's Outer Courtyard's portal destination raw LandblockID
            /// </summary>
            YaraqOCLanding = 0x8c0402c3,

            /// <summary>
            /// Sanamar :: Training Academy's Outer Courtyard's portal raw LandblockID
            /// </summary>
            SanamarOCLaunch = 0x72030331,

            /// <summary>
            /// Sanamar :: Training Academy's Outer Courtyard's portal destination raw LandblockID
            /// </summary>
            SanamarOCLanding = 0x720302c3,

            /// <summary>
            /// Holtburg :: Training Academy's Outer Courtyard's portal destination raw LandblockID
            /// </summary>
            HoltburgOCLanding = 0x860302c3
        }

        public Portal(ObjectType type, ObjectGuid guid, string name, ushort weenieClassId, ObjectDescriptionFlag descriptionFlag, WeenieHeaderFlag weenieFlag, Position position)
            : base(type, guid, name, weenieClassId, descriptionFlag, weenieFlag, position)
        {
        }

        public Portal(AceObject aceO)
            : base(aceO)
        {
            // FIXME(ddevec): Should be inhereted from aceO, not extend it...
            if (aceO.Destination != null)
                Destination = aceO.Destination;
        }

        public uint MinimumLevel
        {
            get { return AceObject.GetIntProperty(PropertyInt.MinLevel) ?? 0; }
        }

        public uint MaximumLevel
        {
            get { return AceObject.GetIntProperty(PropertyInt.MaxLevel) ?? 0; }
        }

        public uint SocietyId
        {
            get { return 0; }
        }

        public bool IsTieable
        {
            get { return ((AceObject.GetIntProperty(PropertyInt.PortalBitmask) ?? 0) & (uint)PortalBitmask.NoRecall) == 0; }
            set
            {
                uint current = AceObject.GetIntProperty(PropertyInt.PortalBitmask) ?? 0;
                current = (value ? (current & ~(uint)PortalBitmask.NoRecall) : current | (uint)PortalBitmask.NoRecall);
                AceObject.SetIntProperty(PropertyInt.PortalBitmask, current);
            }
        }

        public bool IsSummonable
        {
            get { return ((AceObject.GetIntProperty(PropertyInt.PortalBitmask) ?? 0) & (uint)PortalBitmask.NoSummon) == 0; }
            set
            {
                uint current = AceObject.GetIntProperty(PropertyInt.PortalBitmask) ?? 0;
                current = (value ? (current & ~(uint)PortalBitmask.NoSummon) : current | (uint)PortalBitmask.NoSummon);
                AceObject.SetIntProperty(PropertyInt.PortalBitmask, current);
            }
        }

        public bool IsRecallable
        {
            get { return IsTieable; }
            set { IsTieable = value; }
        }

        public override void OnCollide(Player player)
        {
            // validate within use range :: set to a fixed value as static Portals are normally OnCollide usage
            var rangeCheck = 5.0f;

            if (player.Location.SquaredDistanceTo(Location) < rangeCheck)
            {
                if (Destination != null)
                {
                    if ((player.Level >= MinimumLevel) && ((player.Level <= MaximumLevel) || (MaximumLevel == 0)))
                    {
                        var portalDest = Destination;
                        switch (WeenieClassid)
                        {
                            // Setup correct racial portal destination for the Central Courtyard in the Training Academy
                            case (ushort)SpecialPortalWCID.CentralCourtyard:
                                {
                                    var playerLandblockId = player.Location.LandblockId.Raw;
                                    switch (playerLandblockId)
                                    {
                                        case (uint)SpecialPortalLandblockID.ShoushiCCLaunch: // Shoushi
                                            {
                                                portalDest.LandblockId =
                                                    new LandblockId((uint)SpecialPortalLandblockID.ShoushiCCLanding);
                                                break;
                                            }
                                        case (uint)SpecialPortalLandblockID.YaraqCCLaunch: // Yaraq
                                            {
                                                portalDest.LandblockId =
                                                    new LandblockId((uint)SpecialPortalLandblockID.YaraqCCLanding);
                                                break;
                                            }
                                        case (uint)SpecialPortalLandblockID.SanamarCCLaunch: // Sanamar
                                            {
                                                portalDest.LandblockId =
                                                    new LandblockId((uint)SpecialPortalLandblockID.SanamarCCLanding);
                                                break;
                                            }
                                        default: // Holtburg
                                            {
                                                portalDest.LandblockId =
                                                    new LandblockId((uint)SpecialPortalLandblockID.HoltburgCCLanding);
                                                break;
                                            }
                                    }

                                    portalDest.PositionX = Destination.PositionX;
                                    portalDest.PositionY = Destination.PositionY;
                                    portalDest.PositionZ = Destination.PositionZ;
                                    portalDest.RotationX = Destination.RotationX;
                                    portalDest.RotationY = Destination.RotationY;
                                    portalDest.RotationZ = Destination.RotationZ;
                                    portalDest.RotationW = Destination.RotationW;
                                    break;
                                }
                            // Setup correct racial portal destination for the Outer Courtyard in the Training Academy
                            case (ushort)SpecialPortalWCID.OuterCourtyard:
                                {
                                    var playerLandblockId = player.Location.LandblockId.Raw;
                                    switch (playerLandblockId)
                                    {
                                        case (uint)SpecialPortalLandblockID.ShoushiOCLaunch: // Shoushi
                                            {
                                                portalDest.LandblockId =
                                                    new LandblockId((uint)SpecialPortalLandblockID.ShoushiOCLanding);
                                                break;
                                            }
                                        case (uint)SpecialPortalLandblockID.YaraqOCLaunch: // Yaraq
                                            {
                                                portalDest.LandblockId =
                                                    new LandblockId((uint)SpecialPortalLandblockID.YaraqOCLanding);
                                                break;
                                            }
                                        case (uint)SpecialPortalLandblockID.SanamarOCLaunch: // Sanamar
                                            {
                                                portalDest.LandblockId =
                                                    new LandblockId((uint)SpecialPortalLandblockID.SanamarOCLanding);
                                                break;
                                            }
                                        default: // Holtburg
                                            {
                                                portalDest.LandblockId =
                                                    new LandblockId((uint)SpecialPortalLandblockID.HoltburgOCLanding);
                                                break;
                                            }
                                    }

                                    portalDest.PositionX = Destination.PositionX;
                                    portalDest.PositionY = Destination.PositionY;
                                    portalDest.PositionZ = Destination.PositionZ;
                                    portalDest.RotationX = Destination.RotationX;
                                    portalDest.RotationY = Destination.RotationY;
                                    portalDest.RotationZ = Destination.RotationZ;
                                    portalDest.RotationW = Destination.RotationW;
                                    break;
                                }
                            // All other portals don't need adjustments.
                            default:
                                {
                                    break;
                                }
                        }

                        player.Session.Player.Teleport(portalDest);

                        // If the portal just used is able to be recalled to,
                        // save the destination coordinates to the LastPortal character position save table
                        if (Convert.ToBoolean(IsRecallable) == true) player.SetCharacterPosition(PositionType.LastPortal, portalDest);

                        // always send useDone event
                        var sendUseDoneEvent = new GameEventUseDone(player.Session);
                        player.Session.Network.EnqueueSend(sendUseDoneEvent);
                    }
                    else if ((player.Level > MaximumLevel) && (MaximumLevel != 0))
                    {
                        // You are too powerful to interact with that portal!
                        var usePortalMessage = new GameEventDisplayStatusMessage(
                            player.Session,
                            StatusMessageType1.Enum_04AC);

                        // always send useDone event
                        var sendUseDoneEvent = new GameEventUseDone(player.Session);
                        player.Session.Network.EnqueueSend(usePortalMessage, sendUseDoneEvent);
                    }
                    else
                    {
                        // You are not powerful enough to interact with that portal!
                        var usePortalMessage = new GameEventDisplayStatusMessage(
                            player.Session,
                            StatusMessageType1.Enum_04AB);

                        // always send useDone event
                        var sendUseDoneEvent = new GameEventUseDone(player.Session);
                        player.Session.Network.EnqueueSend(usePortalMessage, sendUseDoneEvent);
                    }
                }
                else
                {
                    var serverMessage = "Portal destination for portal ID " + this.WeenieClassid
                                        + " not yet implemented!";
                    var usePortalMessage = new GameMessageSystemChat(serverMessage, ChatMessageType.System);

                    // always send useDone event
                    var sendUseDoneEvent = new GameEventUseDone(player.Session);
                    player.Session.Network.EnqueueSend(usePortalMessage, sendUseDoneEvent);
                }
            }
            else
            {
                // always send useDone event
                var sendUseDoneEvent = new GameEventUseDone(player.Session);
                player.Session.Network.EnqueueSend(sendUseDoneEvent);
            }
        }
    }
}