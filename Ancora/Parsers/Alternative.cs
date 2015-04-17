using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ancora.Parsers
{
    public class Alternative : Parser
    {
        public List<Parser> SubParsers;

        public Alternative(params Parser[] SubParsers)
        {
            this.SubParsers = new List<Parser>(SubParsers);
        }

        public static Alternative operator |(Alternative LHS, Parser RHS)
        {
            var r = LHS.Clone() as Alternative;
            r.SubParsers.Add(RHS);
            return r;
        }

        protected override Parser ImplementClone()
        {
            return new Alternative(SubParsers.ToArray());
        }

        public override ParseResult Parse(StringIterator InputStream)
        {
           foreach (var sub in SubParsers)
           {
               var subResult = sub.Parse(InputStream);
               if (subResult.ParseSucceeded)
                   return subResult.ApplyFlags(this.Flags);
           }

           return ParseResult.Failure;
        }
    }
}
