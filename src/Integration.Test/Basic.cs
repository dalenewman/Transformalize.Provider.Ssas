using System;
using System.Linq;
using Autofac;
using Dapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Transformalize.Configuration;
using Transformalize.Containers.Autofac;
using Transformalize.Contracts;
using Transformalize.Providers.Ado;
using Transformalize.Providers.Ado.Autofac;
using Transformalize.Providers.Console;
using Transformalize.Providers.SqlServer.Autofac;
using Transformalize.Providers.Ssas.Autofac;
using Transformalize.Transforms.Jint.Autofac;

namespace Integration.Test {

   [TestClass]
   public class Basic {

      [TestInitialize]
      public void TransformalizeNorthWind() {
         var logger = new ConsoleLogger(LogLevel.Debug);
         using (var outer = new ConfigurationContainer(new JintModule()).CreateScope("NorthwindToSqlServer.xml?Mode=init", logger)) {
            var process = outer.Resolve<Process>();
            using (var inner = new Container(new JintModule(), new AdoProviderModule(), new SqlServerModule(process)).CreateScope(process, logger)) {
               var controller = inner.Resolve<IProcessController>();
               controller.Execute();
            }
         }
      }

      [TestMethod]
      public void WriteToSsas() {
         var logger = new ConsoleLogger(LogLevel.Debug);
         using (var outer = new ConfigurationContainer().CreateScope("NorthwindToSsas.xml?Mode=init", logger)) {

            var process = outer.Resolve<Process>();
            using (var inner = new Container(new SqlServerModule(process), new SsasModule()).CreateScope(process, logger)) {
               inner.Resolve<IProcessController>().Execute();
            }
         }
      }

      [TestMethod]
      public void ReadFromSsas() {
         var logger = new ConsoleLogger(LogLevel.Debug);
         using (var outer = new ConfigurationContainer().CreateScope("NorthwindFromSsas.xml", logger)) {
            var process = outer.Resolve<Process>();
            using (var inner = new Container(new SsasModule()).CreateScope(process, logger)) {
               inner.Resolve<IProcessController>().Execute();
               Assert.AreEqual(1281, process.Entities.First().Rows.Count());
            }
         }
      }
   }
}
