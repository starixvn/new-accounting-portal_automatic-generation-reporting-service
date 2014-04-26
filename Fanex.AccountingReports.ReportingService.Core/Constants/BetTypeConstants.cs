// --------------------------------------------------------------------------
// <copyright file="BetTypeConstants.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.ReportingService.Core.Constants
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class BetTypeConstants
    {
        public const int Handicap = 1;
        public const int OverUnder2 = 3;
        public const int OverUnder = 85;
        public const int _1stHandicap = 7;
        public const int _1stOverUnder = 8;
        public const int CorrectScore = 4;
        public const int _1stCorrectScore = 30;
        public const int TotalGoal = 6;
        public const int FirstGoalLastGoal = 14;

        public const int MixParlay = 9;
        public const int SystemParlay = 29;

        public const int _5MinOU = 51;
        public const int _15MinOU = 52;
        public const int _30MinOU = 53;
        public const int _45MinOU = 54;
        public const int _60MinOU = 55;
        public const int EndOfDayOU = 56;

        public const int _5MinOE = 61;
        public const int _15MinOE = 62;
        public const int _30MinOE = 63;
        public const int _45MinOE = 64;
        public const int _60MinOE = 65;
        public const int EndOfDayOE = 66;

        public const int WinToteOdds = 31;
        public const int PlateToteOdds = 32;
        public const int WinPlaceToteOdds = 33;

        public const int WinFixedOdds = 41;
        public const int PlaceFixedOdds = 42;
        public const int WinPlaceFixedOdds = 43;

        public const int CasinoGames = 71;
        public const int P2P = 72;
        public const int Bingo = 73;
        public const int P2PGames = 1000;
        public const int Baccarat = 1101;
        public const int Roulette = 1102;
        public const int LiveBaccarat = 1103;
        public const int LiveRoulette = 1104;

        public const int OddEven = 86;

        public const int KickOff = 18999;
        public const int VirtualSportHandicap = 1201;
        public const int VirtualSportOverUnder = 1203;
        public const int VirtualSportCorrectScore = 1204;
        public const int _1X2VirtualSport = 1205;
        public const int VirtualSportTotalGoal = 1206;
        public const int VirtualSportDoubleChance = 1224;

        public const int MinVirtualSportBetType = 1201;
        public const int MaxVirtualSportBetType = 1299;

        public const int StartSetXBetTypeRange = 1311;
        public const int EndSetXBetTypeRange = 1334;

        public const int MinSportingSolutionBetType = 1301;
        public const int MaxSportingSolutionBetType = 1334;

        public const int Outright = 10;

        public const int MinKenoBetType = 1501;
        public const int MaxKenoBetType = 1599;
    }
}
