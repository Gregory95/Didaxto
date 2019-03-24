using System;
using System.Collections.Generic;
using Didaxto.DidaxtoParts;

namespace Didaxto.DidaxtoParts
{
    partial class Lists:Dictionary
    {
        //Opinion word target to-->Target word
        public List<string> OpinionTargetList;

        //Initial list with opinion words
        public List<string> OpinionLexicon { get; set; }

        //Extracted filtered seed opinion words with their orientation
        public List<OpinionWord> FilteredSeed;

        //Extracted Conjunction list with opinion words and their orientation
        public List<OpinionWord> ConjunctionList;

        //Extracted Double propagation list with opinion words and their orientation
        public List<OpinionWord> DoublePropagationList;

    }
}
