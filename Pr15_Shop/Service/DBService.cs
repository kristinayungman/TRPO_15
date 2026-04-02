using Pr15_Shop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pr15_Shop.Service
{
    public class DBService
    {
        private Pr15ShopContext context;
        public Pr15ShopContext Context => context;
        private static DBService? instance;
        public static DBService Instance
        {
            get
            {
                if (instance == null)
                    instance = new DBService();
                return instance;
            }
        }
        private DBService()
        {
            context = new Pr15ShopContext();
        }
    }
}


