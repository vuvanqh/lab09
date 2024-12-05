using System;

namespace Task1_ObjectCreator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Part 1
            try
            {
                Author author = TypeCrafter.TypeCraft<Author>();
                Console.WriteLine(author);
            }
            catch (ParseException)
            {
                try
                {
                    Author author = TypeCrafter.TypeCraft<Author>();
                    Console.WriteLine(author);
                }
                catch(ParseException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            // Part 2
            try
            {
                Book book = TypeCrafter.TypeCraft<Book>();
                Console.WriteLine(book);
            }
            catch (ParseException)
            {
                try
                {
                    Book book = TypeCrafter.TypeCraft<Book>();
                    Console.WriteLine(book);
                }
                catch (ParseException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            
        }
    }
}
