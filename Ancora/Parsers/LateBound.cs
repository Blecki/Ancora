﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ancora.Parsers
{
    public class LateBound : Parser
    {
        private Parser SubParser;
        private LateBound Parent;
        private List<LateBound> Clones;

        public LateBound()
        {
        }

        public LateBound(Parser SubParser)
        {
            this.SubParser = SubParser;
        }

        public void SetSubParser(Parser SubParser)
        {
            this.SubParser = SubParser;
            if (Clones != null) foreach (var clone in Clones) clone.SetSubParser(SubParser);
        }

        protected override Parser ImplementClone()
        {
            if (SubParser != null) //Okay, this has already been bound.
                return new LateBound(SubParser);

            var r = new LateBound(); //Track this late bound so we can bind it... later!

            if (Parent == null) //We are the original late bound
            {
                if (Clones == null) Clones = new List<LateBound>();
                Clones.Add(r);
            }
            else //We are cloning a clone
                Parent.Clones.Add(r);

            return r;
        }

        protected override ParseResult ImplementParse(StringIterator InputStream)
        {
            if (SubParser == null) return ParseResult.Failure;
            var result = SubParser.Parse(InputStream);
            result.Flags |= Flags;
            return result;
        }
    }
}
