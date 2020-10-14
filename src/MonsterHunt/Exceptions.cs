using System;

namespace MonsterHunt
{
    internal class InvalidTownException : Exception
    {
        public InvalidTownException() : base("Invalid location") { }
    }

    internal class SameTownException : Exception
    {
        public SameTownException() : base("Town same as current town") { }
    }

    internal class NoRouteToTownException : Exception
    {
        public NoRouteToTownException() : base("No route to town") { }
    }

    internal class InBattleModeException : Exception
    {
        public InBattleModeException() : base("In battle mode") { }
    }

    internal class NotInBattleModeException : Exception
    {
        public NotInBattleModeException() : base("Not in battle mode") { }
    }

    internal class InvalidMerchantException : Exception
    {
        public InvalidMerchantException() : base("Invalid merchant") { }
    }

    internal class SameMerchantException : Exception
    {
        public SameMerchantException() : base("Merchant same as current merchant") { }
    }

    internal class MerchantNotInTownException : Exception
    {
        public MerchantNotInTownException() : base("Merchant not in town") { }
    }
}