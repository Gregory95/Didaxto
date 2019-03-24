
namespace Didaxto.DidaxtoParts
{
    class OpinionWord
    {
        public string OpWord { get; set; }
        public bool Orientation { get; set; }

        public OpinionWord(string word, bool orientation)
        {
            this.OpWord = word;
            this.Orientation = orientation;
        }
    }
}
