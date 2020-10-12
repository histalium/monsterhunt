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
}