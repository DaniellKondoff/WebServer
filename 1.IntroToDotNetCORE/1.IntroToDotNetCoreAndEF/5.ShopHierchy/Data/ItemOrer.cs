﻿using System;
using System.Collections.Generic;
using System.Text;

namespace _5.ShopHierchy.Data
{
    public class ItemOrder
    {
        public int ItemId { get; set; }

        public Item Item { get; set; }

        public int OrderId { get; set; }

        public Order Order { get; set; }

    }
}
