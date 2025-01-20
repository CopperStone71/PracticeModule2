using Practice2.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practice2.Services
{
    internal class Helper
    {
        private static prog_comEntities _context;
        public static prog_comEntities GetContext()
        {
            return new prog_comEntities();
        }
    }
}
