using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhetos.Dom.DefaultConcepts;
using Rhetos.Utilities;
using System;
using System.Diagnostics;
using System.Linq;

namespace Bookstore.Service.Test.Tools
{
    [TestClass]
    public static class Setup
    {
        [AssemblyInitialize]
        public static void InsertCurrentUser(TestContext testContext)
        {
            var sw = Stopwatch.StartNew();
            using (var scope = TestScope.Create())
            {
                var repository = scope.Resolve<Common.DomRepository>();

                var userInfo = scope.Resolve<IUserInfo>();
                if (userInfo?.IsUserRecognized == true)
                {
                    var principal = repository.Common.Principal.Query(p => p.Name == userInfo.UserName).SingleOrDefault();
                    if (principal == null)
                    {
                        repository.Common.Principal.Insert(new Common.Principal { Name = userInfo.UserName });
                    }
                }

                scope.CommitAndClose();
            }
            Console.WriteLine($"Application startup time: {sw.Elapsed}");
        }
    }
}
