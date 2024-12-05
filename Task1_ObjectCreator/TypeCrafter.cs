using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Task1_ObjectCreator;
public class ParseException : Exception
{
    string Message { get; set; }
    public ParseException(string message): base(message) { }
}
public static class TypeCrafter
{
    public static T TypeCraft<T>()
    {
        Type type = typeof(T);
        PropertyInfo[] properties = type.GetProperties();
        ConstructorInfo? constructor = type.GetConstructor(Type.EmptyTypes);
        if (constructor == null)
        {
            throw new InvalidOperationException($"Type {type.FullName} has no parameterless constructor.");
        }
        
        T result = (T)constructor.Invoke(null);
        foreach (PropertyInfo property in properties)
        {
            Type propertyType = property.PropertyType;
            Type parsableType = typeof(IParsable<>);
            var parsable = propertyType.GetInterfaces()
                .Any(t => t.IsGenericType && t.GetGenericTypeDefinition() == parsableType &&
                          t.GetGenericArguments()[0] == propertyType);
           
            if (propertyType == typeof(string))
            {
                string input = AskForInput(property.Name, propertyType.Name);
                property.SetValue(result, input);
            }
            else if (parsable)
            {
                string input = AskForInput(property.Name, propertyType.Name);
                MethodInfo? parseMethod = propertyType.GetMethod(
                    "TryParse",
                    BindingFlags.Public | BindingFlags.Static,
                    binder: null,
                    types: [typeof(string), typeof(IFormatProvider), propertyType.MakeByRefType()],
                    modifiers: null
                );
                if (parseMethod == null)
                {
                    if(propertyType == typeof(Author))

                    throw new ParseException($"{propertyType.FullName} does not have a static Parse method.");
                }
                object[] args = { input, null!, null };
                bool status = (bool)parseMethod.Invoke(null, args)!;

                if (status)
                    property.SetValue(result, args[2]);
                else
                    throw new ParseException($"Couldn't parse {input} to {propertyType}");

                /*
                object? parsed = parseMethod.Invoke(null, [input, null!]);
                property.SetValue(result, parsed);
                */
            }
            else
            {
                var crafter = typeof(TypeCrafter).GetMethod(nameof(TypeCraft), BindingFlags.Static | BindingFlags.Public);
                var generic = crafter?.MakeGenericMethod(propertyType);
                var complex = generic?.Invoke(null, null);        
                property.SetValue(result,complex);
            }

        }

        return result;
    }

    private static string AskForInput(string propertyName, string type)
    {
        Console.WriteLine($"Provide a variable {propertyName} of type {type}:");
        if (Console.ReadLine() is { } line)
        {
            return line;
        }

        throw new IOException("Line from the Console is not available");
    }
}
