using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DrakesGames.Factory
{
    public static class GenericFactory<T> where T : FactoryObjectBase
    {
        private static Dictionary<string, Type> typeByName;

        public static Dictionary<string, Type> TypesByName
        {
            get
            {
                InitializeFactory();
                return typeByName;
            }
        }

        private static bool IsInitialied => typeByName != null;

        private static void InitializeFactory()
        {
            if (IsInitialied) return;

            var types = Assembly.GetAssembly(typeof(T)).GetTypes()
                .Where(myType => myType.IsClass && myType.IsSubclassOf(typeof(T)));

            typeByName = new Dictionary<string, Type>();

            foreach (var type in types) typeByName.Add(type.Name, type);
        }

        public static bool CheckIfTypeExists(string typeName)
        {
            InitializeFactory();

            return typeByName.ContainsKey(typeName);
        }

        public static Type GetFactoryObjectType(string typeName)
        {
            InitializeFactory();
            if (typeByName.ContainsKey(typeName)) return typeByName[typeName];

            return null;
        }

        public static T GetFactoryObject(string typeName)
        {
            InitializeFactory();

            if (typeByName.ContainsKey(typeName))
            {
                var type = typeByName[typeName];
                var typeInstance = Activator.CreateInstance(type) as T;
                return typeInstance;
            }

            return null;
        }
    }

    public abstract class FactoryObjectBase
    {
        public string Name => GetType().Name;
    }
}