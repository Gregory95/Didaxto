using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Remoting.Messaging;
using System.Security.Authentication.ExtendedProtection.Configuration;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Didaxto
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

    class Didaxto
    {
        public string[] PositiveWords { get; set; }
        public string[] NegativeWords { get; set; }
        public string[] PosDomainWords { get; set; }
        public string[] NegDomainWords { get; set; }
        public string[] FutureWords { get; set; }
        public string[] Conjunctions { get; set; }
        public string[] Adverbs { get; set; }
        public string[] Increasers { get; set; }
        public string[] Decreasers { get; set; }
        public string[] Verbs { get; set; }
        public string[] Comparatives { get; set; }
        public string[] Pronouns { get; set; }
        public string[] Negations { get; set; }

        //Opinion word target to-->Target word
        public List<string> OpinionTargetList;


        //Initial list with opinion words
        public List<string> OpinionLexicon = new List<string>();

        //Extracted filtered seed opinion words with their orientation
        public List<OpinionWord> FilteredSeed;

        //Extracted Conjunction list with opinion words and their orientation
        public List<OpinionWord> ConjunctionList;

        //Extracted Double propagation list with opinion words and their orientation
        public List<OpinionWord> DoublePropagationList;
       

        public string[] SentencesInReview { get; set; } // a lot of sentences
        public string[] WordsInSentence { get; set; } //an array with words of one sentence

        public string Sentence { get; set; }
        public string Target { get; set; } // what is the goal
        public string OpinionWord { get; set; }
        public bool WordOrientation; // positive or negative
        public int PositionOfReview { get; set; } //index of a word in a sentence
        public int SentenceIndex { get; set; } //index of a sentence in opinion


        private readonly char[] _delimiter = new char[] {'.', '!'}; // sentence splitter 100%
        private readonly char[] _whitespace = new char[] {' '}; // whitespace 100%

        //15 reviews of a random products 100%
        private static readonly string[] Opinions =
        {
            "Bought as a gift for hubbie and he thoroughly enjoys using it for all media. Keeps him entertained for hours on end and with no running issues as other tablets we have.",
            ".well, by now one should know that apple is synonymous is unparalleled quality and the iPad 2 was no disappointment; fast, user-friendly, and designed with the end user in mind are just a few of the initial marvels of the machine. If you are in the market for a touch-screen tablet, look no further that the apple store, and choose no other provider that AT&T",
            "a few apps open slow??? the picture quality seems a little blury for a second then clears up nice. so far I love it",
            "A great device hamstrung my Sony\'s dumb use of a custom connector and NO usb charging. Also, come clever person at Sony has decided that a mobile device like this does NOT need a car charger. Doh, doh and Super Doh. I think even Homer Simpson would not be that dopey! Seriously guys, you dont have kids who need entertaining in the back of the car. Oh yes, I must remember to keep the Tablet fully charged before any car journey",
            "About half the time I get the message \"webpage not available\" even though it can be accessed from my laptop. Turning the machine off and then on sometimes corrects the problem",
            "Almost a week, and really enjoying it. Glad I waited for this to come out. Screen is brilliant, charges quickly (80% in 1 hr is for real). Responds quickly to input. The back has a nice feel and it allows you to have a good hold on the tablet. Toshiba quality. A must try if you\'re looking for a tablet",
            "Again Apple has done a great job with the aesthetics and functionality in this product. I am eager looking forward to the next chapter in the tablet world",
            "After I got the Ipad i was so happy. The quality of the display is outstanding. It's so much thinner and lighter than the first generation",
            "Doesn't feel fragile & wimpy. Performs beautifully. Good buy",
            "Good tablet almost as good as Ipad but lack of apps hurts",
            "great product for the price. far better than its competitors. samsung applications are good but doesn't have as many options as google play store. awesome remote to use with my samsung tv",
            "great for surfing the internet! good for the price",
            "Have had this tablet since Dec 2012 and works great. Works well in conjunction with my Galaxy S4. Smaller size makes it easy to throw in my travel bag as well",
            "GREAT TABLET. USE THIS DAILY AND IT MANAGES TO SERVE IT PURPOSE. IT IS SUPER EASY TO USE AND THE SIZE MAKES IT PERFECT!",
            "Hands down the best tablet I've ever used. I cannot fathom why HP did such a poor job with their investment in palm, but this device shows that at least they had some potential at one point. I would have truly loved to own a Pre3 or even a Veer so I could take full advantage of the WebOS ecosystem, but alas it won't happen now. Even on it's own the touchpad smokes the pants off every android tablet I've used and Apple's second go at the tablet market"
        };


        //Add positive words in my List from a text file 100%
        protected string[] FillPositiveWords()
        {
            PositiveWords = System.IO.File.ReadAllLines(
                @"C:\Users\grego\Desktop\Computer Science\Coding\FinalYearProject\Didaxto\Didaxto\Words\positive_words.txt");

            return PositiveWords;
        }

        //Add negative words in my List From a text file  100%
        protected string[] FillNegativeWords()
        {
            NegativeWords = System.IO.File.ReadAllLines(
                @"C:\Users\grego\Desktop\Computer Science\Coding\FinalYearProject\Didaxto\Didaxto\Words\negative_words.txt");

            return NegativeWords;
        }

        //will, would, going to 100%
        protected string[] FillFutureWords()
        {
            FutureWords = System.IO.File.ReadAllLines(
                @"C:\Users\grego\Desktop\Computer Science\Coding\FinalYearProject\Didaxto\Didaxto\Words\future_words.txt");

            return FutureWords;

        }

        //and,or, otherwise 100%
        protected string[] FillConjunctions()
        {
            Conjunctions = System.IO.File.ReadAllLines(
                @"C:\Users\grego\Desktop\Computer Science\Coding\FinalYearProject\Didaxto\Didaxto\Words\various_conj.txt");

            return Conjunctions;
        }

        //do, make, prepare 100%
        protected string[] FillVerbs()
        {
            Verbs = System.IO.File.ReadAllLines(
                @"C:\Users\grego\Desktop\Computer Science\Coding\FinalYearProject\Didaxto\Didaxto\Words\basic_verbs.txt");

            return Verbs;

        }

        //yet,later, because 100%
        protected string[] FillAdverbs()
        {
            Adverbs = System.IO.File.ReadAllLines(
                @"C:\Users\grego\Desktop\Computer Science\Coding\FinalYearProject\Didaxto\Didaxto\Words\adverbs.txt");

            return Adverbs;
        }

        //richer, faster, hotter 100%
        protected string[] FillIncreasers()
        {
            Increasers = System.IO.File.ReadAllLines(
                @"C:\Users\grego\Desktop\Computer Science\Coding\FinalYearProject\Didaxto\Didaxto\Words\increasers.txt");

            return Increasers;
        }

        //meaningless, speachless, useless 100%
        protected string[] FillDecreasers()
        {
            Decreasers = System.IO.File.ReadAllLines(
                @"C:\Users\grego\Desktop\Computer Science\Coding\FinalYearProject\Didaxto\Didaxto\Words\decreasers.txt");

            return Decreasers;
        }

        //better, bigger, calmer, cheaper 100%
        protected string[] FillComparatives()
        {
            Comparatives = System.IO.File.ReadAllLines(
                @"C:\Users\grego\Desktop\Computer Science\Coding\FinalYearProject\Didaxto\Didaxto\Words\comparatives.txt");

            return Comparatives;
        }
        // is , more , mine, little, some
        protected string[] FillPronouns()
        {
            Pronouns = System.IO.File.ReadAllLines(
                @"C:\Users\grego\Desktop\Computer Science\Coding\FinalYearProject\Didaxto\Didaxto\Words\pronouns.txt");
            return Pronouns;
        }
        //not, not
        protected string[] FillNegations()
        {
            Negations = System.IO.File.ReadAllLines(
                @"C:\Users\grego\Desktop\Computer Science\Coding\FinalYearProject\Didaxto\Didaxto\Words\negations.txt");
            return Negations;
        }
        
        //Separate the sentences 100%
        public string[] SentencesSeparator(string[] sentences, int index)
        {
            sentences = Opinions[index].Split(_delimiter, StringSplitOptions.None);
            var sentenceList = sentences.ToList();
            if (index < sentences.Length && sentences != null)
            {
                if(sentences[index] == "")
                    sentenceList.RemoveAt(index);
            }
            else
            {
                sentences = Opinions[index].Split(_delimiter, StringSplitOptions.None);

            }
            sentences = sentenceList.ToArray();
            return sentences;
        }

        //Split each word in a sentence 100%
        public string[] EachWordInSentence(string[] words, string[] sentences, int position)
        {
            words = sentences[position].Split(_whitespace, StringSplitOptions.None);
            var wordsList = words.ToList();
            for (var i = 0; i < words.Length; i++)
            {
                if (words[i] == "") // words[i] == "-" || words[i] == ">" || words[i] == "<" || words[i] =="@")
                    wordsList.RemoveAt(i);
                if (words[i] == "-")
                    wordsList.RemoveAt(i);
                if (words[i] == ">")
                    wordsList.RemoveAt(i);
                if (words[i] == "<")
                    wordsList.RemoveAt(i);
                if (words[i] == "@")
                    wordsList.RemoveAt(i);

            }

            words = wordsList.ToArray();

            return words;
        }

        //not sure yet
        public string OneSentence(string[] sentences, int positionOfSentence)
        {
            Sentence = sentences[positionOfSentence];
            return Sentence;
        }

        //Create the opinion words Lexicon 100%
        protected List<string> Lexicon()
        {
            PosDomainWords = System.IO.File.ReadAllLines(
                @"C:\Users\grego\Desktop\Computer Science\Coding\FinalYearProject\Didaxto\Didaxto\Words\pos_domain_words.txt");

            NegDomainWords = System.IO.File.ReadAllLines(
                @"C:\Users\grego\Desktop\Computer Science\Coding\FinalYearProject\Didaxto\Didaxto\Words\neg_domain_words.txt");

            foreach (var element in PosDomainWords)
                OpinionLexicon.Add(element);

            foreach (var element in NegDomainWords)
                OpinionLexicon.Add(element);

            return OpinionLexicon;
        }

        //Orientation=> if pos = true else false 80%
        protected bool GetOpinionWordOrientation(string word, int position, string sentence)
        {
            FillPositiveWords();
            FillNegativeWords();
            FillAdverbs();
            FillComparatives();
            FillConjunctions();
            FillDecreasers();
            FillFutureWords();
            FillIncreasers();
            FillVerbs();
            FillNegations();
            //{pos}
            if (PositiveWords.Contains(word))
            {
                WordOrientation = true;
            }

            if (NegativeWords.Contains(word))
            {
                WordOrientation = false;
            }

            if (position != 0)
            {
                //{pos} {pos}
                if (PositiveWords.Contains(WordsInSentence[position - 1]))
                {
                    WordOrientation = true;
                }

                //{neg} {pos}
                if (NegativeWords.Contains(WordsInSentence[position - 1]))
                {
                    WordOrientation = false;
                }

                if (position >= 2)
                {
                    //{fut} {verb} {pos}
                    if (FutureWords.Contains(WordsInSentence[position - 2]) &&
                        Verbs.Contains(WordsInSentence[position - 1]) &&
                        PositiveWords.Contains(WordsInSentence[position]))
                    {
                        WordOrientation = true;
                    }

                    //{pos} {neg}
                    if (PositiveWords.Contains(WordsInSentence[position - 1]) &&
                        NegativeWords.Contains(WordsInSentence[position]))
                    {
                        WordOrientation = false;
                    }

                    //{decr} {comp} {pos}
                    if (Decreasers.Contains(WordsInSentence[position - 2]) && Comparatives.Contains(
                                                                               WordsInSentence[position - 1])
                                                                           && PositiveWords.Contains(
                                                                               WordsInSentence[position]))
                    {
                        WordOrientation = true;
                    }

                    if (position >= 3)
                    {
                        //{pos} {conj} {nego} {cpos}
                        if (PositiveWords.Contains(WordsInSentence[position - 3]) && Conjunctions.Contains(
                                                                                      WordsInSentence[position - 2])
                                                                                  && Negations.Contains(
                                                                                      WordsInSentence[position - 1])
                                                                                  && PositiveWords.Contains(
                                                                                      WordsInSentence[position]))
                        {
                            WordOrientation = true;
                        }

                        //{neg} {conj} {incr} {cneg}
                        if (NegativeWords.Contains(WordsInSentence[position - 3]) && Conjunctions.Contains(
                                                                                      WordsInSentence[position - 2])
                                                                                  && Increasers.Contains(
                                                                                      WordsInSentence[position - 1])
                                                                                  && NegativeWords.Contains(
                                                                                      WordsInSentence[position]))
                        {
                            WordOrientation = false;
                        }
                    }
                }
            }
       
        return WordOrientation;
        }

        //Filter seed extraction process 100%
        protected List<OpinionWord> ExtractFilteredSeedOpinionWords()
        {
            FillPositiveWords();
            FillNegativeWords();
            Lexicon();
            PositionOfReview = 0;
            FilteredSeed = new List<OpinionWord>();
            foreach (var opinion in Opinions)
            {
                SentenceIndex = 0;
                SentencesInReview = SentencesSeparator(SentencesInReview, PositionOfReview);
                foreach (var sentence in SentencesInReview)
                {
                    WordsInSentence = EachWordInSentence(WordsInSentence, SentencesInReview, SentenceIndex);
                    var i = -1;
                    foreach (var word in WordsInSentence)
                    {
                        i++;
                        var tempWord = word.ToLower();
                        var opinionWord = new OpinionWord(tempWord, WordOrientation)
                        {
                            OpWord = tempWord,
                            Orientation = WordOrientation
                        };
                        if (PositiveWords.Contains(tempWord) || NegativeWords.Contains(tempWord))
                        {
                            var position = i;
                            WordOrientation = GetOpinionWordOrientation(tempWord, position, sentence);

                            if (!OpinionLexicon.Contains(tempWord))
                            {
                                FilteredSeed.Add(new OpinionWord(tempWord, WordOrientation)
                                    {OpWord = tempWord, Orientation = WordOrientation});
                            }
                            else
                            {
                                FilteredSeed.Add(opinionWord);

                            }//in documentation says that we have to add only oriantation..how to implement that?
                            
                        }
                    }

                    SentenceIndex++;
                    Array.Clear(WordsInSentence, 0, WordsInSentence.Length);
                }

                PositionOfReview++;
            }

            return FilteredSeed;

        }

        //Conjunction based opinion word extraction process 100%

        protected string GetConjunctionBaseOpinionWord(string word, int index, string sentence)
        {
            FillConjunctions();
            FillNegativeWords();
            FillPositiveWords();
            var tempWord = "";
            if (index + 2 <= WordsInSentence.Length || index + 1 <= WordsInSentence.Length)
            {
                var unknown = WordsInSentence[index];
                foreach (var conjunction in Conjunctions)
                {
                    if (Equals(conjunction, unknown))
                        if (PositiveWords.Contains(WordsInSentence[index + 1]) ||
                            NegativeWords.Contains(WordsInSentence[index + 1]))
                        {
                            tempWord = WordsInSentence[index + 1];
                            return tempWord;
                        }
                }
            }

            return null;

        }


        //Conjunction based extraction process 90%

        protected List<OpinionWord> ExtractConjunctionBasedOpinionWords()
        {
            FillPositiveWords();
            FillNegativeWords();
            PositionOfReview = 0;
            SentenceIndex = 0;
            ConjunctionList = new List<OpinionWord>();
            foreach (var Opinion in Opinions)
            {
                SentenceIndex = 0;
                SentencesInReview = SentencesSeparator(SentencesInReview, PositionOfReview);
                foreach (var sentence in SentencesInReview)
                {
                    WordsInSentence = EachWordInSentence(WordsInSentence, SentencesInReview, SentenceIndex);
                    var i = -1;
                    foreach (var word in WordsInSentence)
                    {
                        i++;
                        var tempWord = word.ToLower();
                        var position = i;
                        var opinionWord = new OpinionWord(tempWord, WordOrientation)
                        {
                            OpWord = tempWord,
                            Orientation = WordOrientation
                        };
                        //check if the opinion word is included in filteredSeed list
                        var matches = FilteredSeed.Find(Didaxto => opinionWord.OpWord == tempWord);
                        // var matches = FilteredSeed.Where(OpinionWord => OpinionWord.OpWord == tempWord);
                        if (FilteredSeed.Contains(matches)) //matches.OfType<OpinionWord>().Equals(opinionWord)
                            if (position + 1 < WordsInSentence.Length)
                            {
                                if (Conjunctions.Contains(WordsInSentence[position + 1]))
                                { 
                                    tempWord = GetConjunctionBaseOpinionWord(tempWord, position, sentence);
                                    {

                                        if (PositiveWords.Contains(tempWord) || NegativeWords.Contains(tempWord))
                                        {
                                            WordOrientation = GetOpinionWordOrientation(tempWord, position, sentence);
                                            if (!OpinionLexicon.Contains(tempWord))
                                            {
                                                ConjunctionList.Add(new OpinionWord(tempWord, WordOrientation)
                                                    {OpWord = tempWord, Orientation = WordOrientation});
                                            }
                                            else
                                            {
                                                ConjunctionList.Add(new OpinionWord(tempWord, opinionWord.Orientation));
                                            }
                                        }
                                    }
                                }
                            }
                    }

           
                    
                }

                SentenceIndex++;
                Array.Clear(WordsInSentence, 0, WordsInSentence.Length);
            }

            PositionOfReview++;
        
        return ConjunctionList;
        }

        // Find an opinion word target 90%
        protected string GetOpinionWordTarget(string OpinionWord, int index, string sentence)
        {
            FillConjunctions();
            FillAdverbs();
            FillComparatives();
            FillDecreasers();
            FillFutureWords();
            FillIncreasers();
            FillVerbs();
            FillPronouns();
            FillNegations();
            OpinionWord = WordsInSentence[index];
            if (OpinionWord != null)
            {
                if (sentence.Contains(OpinionWord))
                {
                    int myvalue = WordsInSentence.GetUpperBound(0);
                    if (myvalue >= index + 1)
                    {
                        Target = WordsInSentence[index + 1];
                        Target = Target.ToLower();
                        if ((!OpinionLexicon.Contains(Target)) && (!Conjunctions.Contains(Target)) &&
                            (!Comparatives.Contains(Target)) &&
                            (!FutureWords.Contains(Target)) &&
                            (!Adverbs.Contains(Target)) &&
                            (!Increasers.Contains(Target)) &&
                            (!Decreasers.Contains(Target)) &&
                            (!Verbs.Contains(Target)) &&
                            (!Pronouns.Contains(Target)) &&
                            (!Negations.Contains(Target)))
                            return Target;
                        {
                            if (Conjunctions.Contains(WordsInSentence[index + 1]))
                            {
                                Target = WordsInSentence[index + 2];
                                if ((!OpinionLexicon.Contains(Target)) && (!Conjunctions.Contains(Target)) &&
                                    (!Comparatives.Contains(Target)) &&
                                    (!FutureWords.Contains(Target)) &&
                                    (!Adverbs.Contains(Target)) &&
                                    (!Increasers.Contains(Target)) &&
                                    (!Decreasers.Contains(Target)) &&
                                    (!Verbs.Contains(Target)) &&
                                    (!Pronouns.Contains(Target)) &&
                                    (!Negations.Contains(Target)))
                                    return Target;
                            }
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
            }

            return null;

        }

        // fill up the opinion target list with new targets 80%
        protected List<string> ExtractOpinionWordTargets()
        {
            PositionOfReview = 0;
            SentenceIndex = 0;
            FillPositiveWords();
            FillNegativeWords();
            Lexicon();
            OpinionTargetList = new List<string>();
            foreach (var opinion in Opinions)
            {
                SentenceIndex = 0;
                SentencesInReview = SentencesSeparator(SentencesInReview, PositionOfReview);
                foreach (var sentence in SentencesInReview)
                {
                    WordsInSentence = EachWordInSentence(WordsInSentence, SentencesInReview, SentenceIndex);
                    var i = -1;
                    foreach (var word in WordsInSentence)
                    {
                        i++;
                        var tempWord = word.ToLower();
                        if (OpinionLexicon.Contains(tempWord))
                        {
                            Target = GetOpinionWordTarget(tempWord, i, sentence);
                            if (!OpinionTargetList.Contains(Target))
                                OpinionTargetList.Add(Target);
                        }
                    }

                    SentenceIndex++;
                    Array.Clear(WordsInSentence, 0, WordsInSentence.Length);
                }

                PositionOfReview++;
            }

            return OpinionTargetList;
        }

        //Double check the opinion words 80%
        protected string GetDoublePropagationOpinionWord(string OpinionWord, int index, string sentence)
        {
            FillConjunctions();
            Lexicon();
            if (!(index + 1 >= WordsInSentence.Length))
            {
                if (Conjunctions.Contains(WordsInSentence[index + 1]) && OpinionLexicon.Contains(OpinionWord))
                    return OpinionWord;
                else if(OpinionTargetList.Contains(WordsInSentence[index + 1]) && OpinionLexicon.Contains(OpinionWord))
                    return OpinionWord;

                else return null;
            }

            return null;
        }

        //double check the opinion word orientations 80%
        protected bool GetDoublePropagationOpWordOrientation(string OpionionTarget, int index, string sentence)
        {
            var d = new Didaxto();
            var tempOpinionWord = new OpinionWord(d.OpinionWord, WordOrientation);
            
            if (sentence.Contains(d.OpinionWord))
            {
                WordOrientation = GetOpinionWordOrientation(d.OpinionWord, index, sentence);
            }
            else
            {
                SentenceIndex++;
                SentencesSeparator(SentencesInReview, SentenceIndex);
                sentence = SentencesInReview[SentenceIndex];
                if (sentence.Contains(d.OpinionWord))
                    WordOrientation = GetOpinionWordOrientation(d.OpinionWord, index, sentence);
            }
            
            return WordOrientation;
        }

        //Double propagation method..under construction 100%
        protected void DoublePropagation()
        {
            foreach (var word in WordsInSentence)
            {
                var tempWord = word.ToLower();
                while (!OpinionLexicon.Contains(tempWord) || !OpinionTargetList.Contains(Target))
                {
                    ExtractOpinionWordTargets();
                    ExtractOpinionWords();
                }

            }
        }

        //Extract new opinion words 70%
        protected List<OpinionWord> ExtractOpinionWords()
        {
            FillPositiveWords();
            FillNegativeWords();
            Lexicon();
            PositionOfReview = 0;
            DoublePropagationList = new List<OpinionWord>();
            foreach (var opinion in Opinions)
            {
                SentenceIndex = 0;
                SentencesInReview = SentencesSeparator(SentencesInReview, PositionOfReview);
                foreach (var sentence in SentencesInReview)
                {
                    WordsInSentence = EachWordInSentence(WordsInSentence, SentencesInReview, SentenceIndex);
                    var i = -1;
                    foreach (var word in WordsInSentence)
                    {
                        i++;
                        var tempWord = word.ToLower();
                        var opinionWord = new OpinionWord(tempWord, WordOrientation)
                        {
                            OpWord = tempWord,
                            Orientation = WordOrientation
                        };
                        var position = i;
                        Target = GetOpinionWordTarget(tempWord, position, sentence);
                        if (OpinionTargetList.Contains(Target))
                        {
                            tempWord = GetDoublePropagationOpinionWord(tempWord, position, sentence);
                            if ((PositiveWords.Contains(tempWord) || NegativeWords.Contains(tempWord)) &&
                                tempWord != null)
                            {
                                WordOrientation = GetOpinionWordOrientation(tempWord, position, sentence);

                                if (!OpinionLexicon.Contains(tempWord))

                                    DoublePropagationList.Add(new OpinionWord(opinionWord.OpWord,
                                        opinionWord.Orientation));
                            }
                            else
                            {
                                if(tempWord != null)
                                DoublePropagationList.Add(new OpinionWord(tempWord, opinionWord.Orientation));
                            }
                        }
                    }
                    SentenceIndex++;
                }
                PositionOfReview++;

            }

            return DoublePropagationList;
        }

        //TEST
            static void Main(string[] args)
            {

                var d = new Didaxto();

                var orie = "";


                d.ExtractFilteredSeedOpinionWords();
                d.ExtractConjunctionBasedOpinionWords();
                d.ExtractOpinionWordTargets();
                d.ExtractOpinionWords();


                foreach (OpinionWord item in d.FilteredSeed)
                {

                    if (item.Orientation == true)
                    {
                        orie = "Positive";
                        Console.WriteLine("\t" + $"{item.OpWord}" + "\t" + orie);
                    }
                    else
                    {
                        orie = "Negative";
                        Console.WriteLine("\t" + $"{item.OpWord}" + "\t" + orie);

                    }
                }

                Console.Write("\n");

                foreach (OpinionWord item in d.ConjunctionList)
                {

                    if (item.Orientation == true)
                    {
                        orie = "Positive";
                        Console.WriteLine("\t" + $"{item.OpWord}" + "\t" + orie);
                    }
                    else
                    {
                        orie = "Negative";
                        Console.WriteLine("\t" + $"{item.OpWord}" + "\t" + orie);

                    }
                }
                Console.Write("\n");
                foreach (OpinionWord value in d.DoublePropagationList)
                {
                    if (value.Orientation == true)
                    {
                        orie = "Positive";
                        Console.WriteLine("\t" + $"{value.OpWord}" + "\t" + orie);
                    }
                    else
                    {
                        orie = "Negative";
                        Console.WriteLine("\t" + $"{value.OpWord}" + "\t" + orie);
                    }
                }

                Console.Write("\n");

            foreach (var element in d.OpinionTargetList)
                {
                    Console.WriteLine("\t" + element);
                }

                Console.Write("\n");

                Console.ReadKey();


            }
        }
    }

