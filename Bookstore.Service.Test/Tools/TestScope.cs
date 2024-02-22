using Autofac;
using Rhetos;
using Rhetos.Logging;
using Rhetos.Security;
using Rhetos.Utilities;
using System;
using System.IO;

namespace Bookstore.Service.Test.Tools
{
    /// <summary>
    /// Helper class that manages Dependency Injection container for unit tests.
    /// The container can be customized for each unit test scope.
    /// </summary>
    public static class TestScope
    {
        /// <summary>
        /// Creates a thread-safe lifetime scope DI container (service provider)
        /// to isolate unit of work with a <b>separate database transaction</b>.
        /// To commit changes to database, call <see cref="IUnitOfWork.CommitAndClose"/> at the end of the 'using' block.
        /// </summary>
        /// <remarks>
        /// Use helper methods in <see cref="TestScopeContainerBuilderExtensions"/> to configuring components
        /// from the <paramref name="registerCustomComponents"/> delegate.
        /// </remarks>
        public static IUnitOfWorkScope Create(Action<ContainerBuilder> registerCustomComponents = null)
        {
            return _rhetosHost.Value.CreateScope(registerCustomComponents);
        }

        private const string RhetosAppPath = @"C:\Users\aprskalo\Desktop\Day1\Bookstore.Service\bin\Debug\net6.0\Bookstore.Service.dll";

        /// <summary>
        /// Reusing a single shared static DI container between tests, to reduce initialization time for each test.
        /// Each test should create a child scope with <see cref="TestScope.Create"/> method to start a 'using' block.
        /// </summary>
        private static readonly Lazy<RhetosHost> _rhetosHost = new(() => RhetosHost.CreateFrom(RhetosAppPath, ConfigureRhetosHostBuilder));

        private static void ConfigureRhetosHostBuilder(IRhetosHostBuilder rhetosHostBuilder)
        {
            rhetosHostBuilder.ConfigureContainer(builder =>
            {
                // Configuring standard Rhetos system services to work with unit tests:
                builder.RegisterType<ProcessUserInfo>().As<IUserInfo>();
                builder.RegisterType<ConsoleLogProvider>().As<ILogProvider>();
            });
        }
    }
}
