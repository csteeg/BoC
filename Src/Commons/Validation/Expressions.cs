using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BoC.Validation
{
    public static class Expressions
    {
        public const string Email = @"^([0-9a-zA-Z]+[-._+&])*[0-9a-zA-Z]+@([-0-9a-zA-Z]+[.])+[a-zA-Z]{2,6}$";
        public const string Guid = @"^(\{{0,1}([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}\}{0,1})$";
        public const string WebUrl = @"(http|https)://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?";
    }
}
