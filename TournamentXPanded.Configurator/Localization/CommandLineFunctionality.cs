using System;
using System.Collections.Generic;
using System.Reflection;

namespace TournamentXPanded.Configurator.Localization
{
    public static class CommandLineFunctionality
    {
        private static Dictionary<string, CommandLineFunctionality.CommandLineFunction> AllFunctions;

        static CommandLineFunctionality()
        {
            CommandLineFunctionality.AllFunctions = new Dictionary<string, CommandLineFunctionality.CommandLineFunction>();
        }

        public static string CallFunction(string concatName, string concatArguments)
        {
            List<string> strs;
            strs = (concatArguments == String.Empty ? new List<string>() : new List<string>(concatArguments.Split(new Char[] { ' ' })));
            return CommandLineFunctionality.AllFunctions[concatName].Call(strs);
        }

        private static bool CheckAssemblyReferencesThis(Assembly assembly)
        {
            Assembly assembly1 = typeof(CommandLineFunctionality).Assembly;
            if (assembly1.GetName().Name == assembly.GetName().Name)
            {
                return true;
            }
            AssemblyName[] referencedAssemblies = assembly.GetReferencedAssemblies();
            for (int i = 0; i < (int)referencedAssemblies.Length; i++)
            {
                if (referencedAssemblies[i].Name == assembly1.GetName().Name)
                {
                    return true;
                }
            }
            return false;
        }

        public static List<string> CollectCommandLineFunctions()
        {
            List<string> strs = new List<string>();
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < (int)assemblies.Length; i++)
            {
                Assembly assembly = assemblies[i];
                if (CommandLineFunctionality.CheckAssemblyReferencesThis(assembly))
                {
                    Type[] types = assembly.GetTypes();
                    for (int j = 0; j < (int)types.Length; j++)
                    {
                        MethodInfo[] methods = types[j].GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                        for (int k = 0; k < (int)methods.Length; k++)
                        {
                            MethodInfo methodInfo = methods[k];
                            object[] customAttributes = methodInfo.GetCustomAttributes(typeof(CommandLineFunctionality.CommandLineArgumentFunction), false);
                            if (customAttributes != null && customAttributes.Length != 0)
                            {
                                CommandLineFunctionality.CommandLineArgumentFunction commandLineArgumentFunction = customAttributes[0] as CommandLineFunctionality.CommandLineArgumentFunction;
                                if (commandLineArgumentFunction != null && !(methodInfo.ReturnType != typeof(String)))
                                {
                                    string name = commandLineArgumentFunction.Name;
                                    string str = String.Concat(commandLineArgumentFunction.GroupName, ".", name);
                                    strs.Add(str);
                                    CommandLineFunctionality.CommandLineFunction commandLineFunction = new CommandLineFunctionality.CommandLineFunction((Func<List<string>, string>)Delegate.CreateDelegate(typeof(Func<List<string>, string>), methodInfo));
                                    CommandLineFunctionality.AllFunctions.Add(str, commandLineFunction);
                                }
                            }
                        }
                    }
                }
            }
            return strs;
        }

        public class CommandLineArgumentFunction : Attribute
        {
            public string Name;

            public string GroupName;

            public CommandLineArgumentFunction(string name, string groupname)
            {
                this.Name = name;
                this.GroupName = groupname;
            }
        }

        private class CommandLineFunction
        {
            public Func<List<string>, string> CommandLineFunc;

            public List<CommandLineFunctionality.CommandLineFunction> Children;

            public CommandLineFunction(Func<List<string>, string> commandlinefunc)
            {
                this.CommandLineFunc = commandlinefunc;
                this.Children = new List<CommandLineFunctionality.CommandLineFunction>();
            }

            public string Call(List<string> objects)
            {
                return this.CommandLineFunc(objects);
            }
        }
    }
}