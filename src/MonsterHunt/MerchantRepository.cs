using System.Collections.Generic;
using System.Linq;

namespace MonsterHunt
{
    internal class MerchantRepository
    {
        private readonly List<Merchant> merchants = new List<Merchant>();

        public void Add(Merchant merchant)
        {
            merchants.Add(merchant);
        }

        public Merchant Find(string merchantName)
        {
            var merchant = merchants
                .Where(t => t.Name.AreEqualIgnoreCase(merchantName))
                .SingleOrDefault();

            return merchant;
        }
    }
}