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

    internal class InvalidItemException : Exception
    {
        public InvalidItemException() : base("Invalid item") { }
    }

    internal class ItemNotAWeaponException : Exception
    {
        public ItemNotAWeaponException() : base("Item not a weapon") { }
    }

    internal class DoNotOwnItemException : Exception
    {
        public DoNotOwnItemException() : base("Do not own item") { }
    }

    internal class ItemNotBodyArmorException : Exception
    {
        public ItemNotBodyArmorException() : base("Item not body armor") { }
    }

    internal class ItemNotLegArmorException : Exception
    {
        public ItemNotLegArmorException() : base("Item not leg armor") { }
    }

    internal class DefeatedException : Exception
    {
        public DefeatedException() : base("You are defeated") { }
    }
}