namespace MonsterHunt
{
    internal class UnitOfWork
    {
        public UnitOfWork()
        {
            Items = new ItemRepository();
            Merchants = new MerchantRepository();
            Monsters = new MonsterRepository();
            Routes = new RouteRepository();
            Towns = new TownRepository();
            Elements = new ElementRepository();
            Locations = new LocationRepository();
        }

        public ItemRepository Items { get; }

        public MerchantRepository Merchants { get; }

        public MonsterRepository Monsters { get; }

        public RouteRepository Routes { get; }

        public TownRepository Towns { get; }

        public ElementRepository Elements { get; }

        public LocationRepository Locations { get; }
    }
}