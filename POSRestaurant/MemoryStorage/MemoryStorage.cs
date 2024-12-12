using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSRestaurant.MemoryStorage
{
    public static class MemoryStorage
    {
        public static long LastOrderNumber {  get; set; }

        public static Dictionary<int, long> LastKOTForTable { get; set; } = new Dictionary<int, long>();
    }
}
