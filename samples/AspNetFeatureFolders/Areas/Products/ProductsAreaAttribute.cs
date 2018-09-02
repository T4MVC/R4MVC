using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace AspNetFeatureFolders.Areas.Products
{
    public class ProductsAreaAttribute : AreaAttribute
    {
        public ProductsAreaAttribute() : base("Products")
        {
        }
    }
}
