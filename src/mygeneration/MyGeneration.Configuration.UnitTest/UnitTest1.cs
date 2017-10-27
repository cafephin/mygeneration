using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MyGeneration.Configuration.UnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            SetEntryAssembly();
            var defaultSettings = new DefaultSettings
                                  {
                                      DbConnectionSettings =
                                      {
                                          ConnectionInfoCollection = new List<ConnectionInfo>
                                                                     {
                                                                         new ConnectionInfo()
                                                                         {
                                                                             Name = "test",
                                                                             ConnectionString =
                                                                                 "test conn str",
                                                                             DbDriver = "test db driver"
                                                                         }
                                                                     },
                                          UserDatabaseAliases = { new DatabaseAlias(){DatabaseName = "appq.dev.app.local", Alias = "AppQ"}}
                                      },
                                      RecentFiles = { "file1.csgen", "file2.zeus" }
                                  };
            var xmlSerializer = new System.Xml.Serialization.XmlSerializer(defaultSettings.GetType());
            xmlSerializer.Serialize(Console.Out, defaultSettings);
            Console.WriteLine();
        }

        /// <summary>
        /// Use as first line in ad hoc tests (needed by XNA specifically)
        /// </summary>
        public static void SetEntryAssembly()
        {
            SetEntryAssembly(Assembly.GetCallingAssembly());
        }

        /// <summary>
        /// Allows setting the Entry Assembly when needed. 
        /// Use AssemblyUtilities.SetEntryAssembly() as first line in XNA ad hoc tests
        /// </summary>
        /// <param name="assembly">Assembly to set as entry assembly</param>
        public static void SetEntryAssembly(Assembly assembly)
        {
            var manager = new AppDomainManager();
            FieldInfo entryAssemblyfield = manager.GetType().GetField("m_entryAssembly", BindingFlags.Instance | BindingFlags.NonPublic);
            entryAssemblyfield.SetValue(manager, assembly);

            AppDomain domain = AppDomain.CurrentDomain;
            FieldInfo domainManagerField = domain.GetType().GetField("_domainManager", BindingFlags.Instance | BindingFlags.NonPublic);
            domainManagerField.SetValue(domain, manager);
        }
    }
}
