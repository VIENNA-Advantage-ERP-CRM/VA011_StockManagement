using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VAdvantage.Model;

namespace VA011.Models
{
    public class ProductModel
    {

    }

    public class Products
    {
        public string Name { get; set; }
        public int M_Product_ID { get; set; }
        public Decimal? QtyOnHand { get; set; }
        public int C_UOM_ID { get; set; }
        public string UOM { get; set; }
        public Decimal? Reserved { get; set; }
        public Decimal? QtyAvailable { get; set; }
        public Decimal? UnConfirmed { get; set; }
        public Decimal? Ordered { get; set; }
        public Decimal? Demanded { get; set; }
        public Decimal? TillReorder { get; set; }
        public Decimal? QtyToReplenish { get; set; }
        public Decimal? MinLevel { get; set; }
        public string Value { get; set; }
    }
}