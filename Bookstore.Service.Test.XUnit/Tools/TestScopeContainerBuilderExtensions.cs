using Autofac;
using Rhetos.Logging;
using Rhetos.TestCommon;
using Rhetos.Utilities;
using System;
using System.Collections.Generic;

namespace Bookstore.Service.Test.Tools
{
    /// <summary>
    /// Helper methods for configuring <see cref="TestScope"/> components in scope of a unit tests.
    /// </summary>
    public static class TestScopeContainerBuilderExtensions
    {
        /// <summary>
        /// Reports all entries from Rhetos system log to the given list of strings.
        /// </summary>
        public static ContainerBuilder ConfigureLogMonitor(this ContainerBuilder builder, List<string> log, EventType minLevel = EventType.Trace)
        {
            builder.RegisterInstance(new ConsoleLogProvider((eventType, eventName, message) =>
                {
                    if (eventType >= minLevel)
                        log.Add("[" + eventType + "] " + (eventName != null ? (eventName + ": ") : "") + message());
                }))
                .As<ILogProvider>();
            return builder;
        }

        /// <summary>
        /// Override the default application user (current process account) for testing.
        /// </summary>
        public static ContainerBuilder ConfigureApplicationUser(this ContainerBuilder builder, string username)
        {
            builder.RegisterInstance(new TestUserInfo(username)).As<IUserInfo>();
            return builder;
        }

        /// <summary>
        /// This method uses a shallow copy of the original options instance for configuration.
        /// It supports <paramref name="configure"/> action that directly modifies properties of the options class.
        /// </summary>
        /// <remarks>
        /// Since options classes are usually singletons, the action must not modify an object that is referenced
        /// by the options class, without modifying the options class property,
        /// because it might affect configuration of other unit tests.
        /// </remarks>
        public static ContainerBuilder ConfigureOptions<TOptions>(this ContainerBuilder builder, Action<TOptions> configure) where TOptions : class
        {
            TOptions copy;
            using (var scope = TestScope.Create())
            {
                var options = scope.Resolve<TOptions>();
                // Options classes as usually singleton, so we are making a copy to avoid affecting configuration of other tests.
                copy = CsUtility.ShallowCopy(options);
            }

            configure.Invoke(copy);
            builder.RegisterInstance(copy);
            return builder;
        }
    }
}
