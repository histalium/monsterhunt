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

    internal class CanNotUseItemException : Exception
    {
        public CanNotUseItemException() : base("Can not use item") { }
    }

    internal class NotAtAMerchantException : Exception
    {
        public NotAtAMerchantException() : base("Not at a merchant") { }
    }

    internal class MerchantDoesNotOfferException : Exception
    {
        public MerchantDoesNotOfferException() : base("Merchant does not offer item") { }
    }

    internal class NotEnoughCoinsException : Exception
    {
        public NotEnoughCoinsException() : base("Not enaugh coins") { }
    }

    internal class MerchantDoesNotRequestException : Exception
    {
        public MerchantDoesNotRequestException() : base("Merchant does not request item") { }
    }

    internal class ItemNotRecipeException : Exception
    {
        public ItemNotRecipeException() : base("Item is not a recipe") { }
    }

    internal class DoNotKnowRecipeException : Exception
    {
        public DoNotKnowRecipeException() : base("Do not know recipe") { }
    }

    internal class MissingIngredientsException : Exception
    {
        public MissingIngredientsException() : base("Some ingredients are missing") { }
    }
}