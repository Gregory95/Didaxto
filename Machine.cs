using System;
using Didaxto.DidaxtoParts;
// MAIN
namespace Didaxto.DidaxtoParts
{
    class Machine:Didaxto
    {
        //TEST
        protected static void Main(string[] args)
        {
        
        var didaxto = new Didaxto();
        var lists = new Lists();


        var orientation = "";

               lists.FilteredSeed = didaxto.ExtractFilteredSeedOpinionWords();
               lists.ConjunctionList = didaxto.ExtractConjunctionBasedOpinionWords();
               lists.OpinionTargetList = didaxto.ExtractOpinionWordTargets();
               lists.DoublePropagationList = didaxto.ExtractOpinionWords();


                foreach (OpinionWord item in lists.FilteredSeed)
                {

                    if (item.Orientation == true)
                    {
                        orientation = "Positive";
                        Console.WriteLine("\t" + $"{item.OpWord}" + "\t" + orientation);
                    }
                    else
                    {
                        orientation = "Negative";
                        Console.WriteLine("\t" + $"{item.OpWord}" + "\t" + orientation);

                    }
                }

                Console.Write("\n");

                foreach (OpinionWord item in lists.ConjunctionList)
                {

                    if (item.Orientation == true)
                    {
                        orientation = "Positive";
                        Console.WriteLine("\t" + $"{item.OpWord}" + "\t" + orientation);
                    }
                    else
                    {
                        orientation = "Negative";
                        Console.WriteLine("\t" + $"{item.OpWord}" + "\t" + orientation);

                    }
                }
                Console.Write("\n");
                foreach (OpinionWord value in lists.DoublePropagationList)
                {
                    if (value.Orientation == true)
                    {
                        orientation = "Positive";
                        Console.WriteLine("\t" + $"{value.OpWord}" + "\t" + orientation);
                    }
                    else
                    {
                        orientation = "Negative";
                        Console.WriteLine("\t" + $"{value.OpWord}" + "\t" + orientation);
                    }
                }

                Console.Write("\n");

            foreach (var element in lists.OpinionTargetList)
                {
                    Console.WriteLine("\t" + element);
                }

                Console.Write("\n");

                Console.ReadKey();


            }
    }

}

