/*
 This file is part of the Japes Programming Language.

 Japes is free software: you can redistribute it and/or modify
 it under the terms of the GNU General Public License as
 published by the Free Software Foundation, either version 3 of
 the License, or (at your option) any later version.

 Japes is distributed in the hope that it will be useful,
 but WITHOUT ANY WARRANTY; without even the implied warranty of
 MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 GNU General Public License for more details.

 You should have received a copy of the GNU General Public
 License along with Japes.  If not, see <http://www.gnu.org/licenses/>.
*/

/* WARNING: ALL MEMBERS IN THIS API ARE SUBJECT TO MAJOR REVISION UNTIL BOOTSTRAPPED. */

using static japes.toolchain.api.io.ILogger;

namespace japes.toolchain.api.io {

    #region Interfaces

    /// <summary>
    /// An interface specifying a Japes Toolchain API logger.
    /// </summary>
    public interface ILogger {
        #region Classes

        /// <summary>
        /// An enumeration specifying a log message's severity.
        /// </summary>
        public enum Severity {
            /// <summary>
            /// The message represents minutae.
            /// </summary>
            TRIVIAL,
            /// <summary>
            /// The message represents useful (but non-error) information. 
            /// </summary>
            INFO,
            /// <summary>
            /// The message represents information about something that may
            /// lead to unintended behavior, but is not strictly an error.
            /// </summary>
            WARNING,
            /// <summary>
            /// The message represents an error that permits processing to 
            /// continue, but which will at some point trigger a fatal 
            /// condition that invalidates its result.
            /// </summary>
            ERROR,
            /// <summary>
            /// The message represents an error that is irrecoverable. The
            /// result is inherently invalid.
            /// </summary>
            FATAL
        };


        #endregion Classes

        /// <summary>
        /// Record a new log message.
        /// </summary>
        /// <param name="severity">The severity associated with this 
        /// message.</param>
        /// <param name="tag">An optional channel name to contextualize the
        /// nature of the message.</param>
        /// <param name="format">The message to emit.</param>
        /// <param name="args">The arguments to emit.</param>
        public void Log(Severity severity, string? tag, string format, params object?[]? args);

        /// <summary>
        /// Record a new log message.
        /// </summary>
        /// <param name="severity">The severity associated with this 
        /// message.</param>
        /// <param name="format">The message to emit.</param>
        /// <param name="args">The arguments to emit.</param>
        public void Log(Severity severity, string format, params object?[]? args)
            => Log(severity, null, format, args);
    }

    /// <summary>
    /// A refinement of <c>Ilogger</c> which can change its context by pushing
    /// another logger to a stack.
    /// </summary>
    public interface IStackLogger : ILogger {
        #region Stack Manipulation

        /// <summary>
        /// Push <paramref name="logger"/> to the stack.
        /// </summary>
        /// <param name="logger">The logger to push.</param>
        public void Push(ILogger logger);

        /// <summary>
        /// Pop the logger from the top of the stac.
        /// </summary>
        /// <returns>If there was a logger to pop, a valid logger. Otherwise, null.</returns>
        public ILogger? Pop();

        #endregion Stack Manipulation
    }

    #endregion Interfaces

    /// <summary>
    /// A logger that acts in reference to another.
    /// </summary>
    /// <typeparam name="P">The type of the parent logger.</typeparam>
    public abstract class ProxyLogger<P> : ILogger
        where P : ILogger {
        internal P m_parent;

        #region Properties

        /// <summary>
        /// Construct a new <c>ProxyLogger</c> from its <paramref name="parent"/>.
        /// </summary>
        /// <param name="parent">The parent logger to encapsulate.</param>
        protected ProxyLogger(P parent)
            => m_parent = parent;

        #endregion Properties

        #region ILogger

        /// <summary>
        /// Log a potentially transformed message to the parent logger. 
        /// </summary>
        /// <param name="severity">The severity associated with this 
        /// message.</param>
        /// <param name="tag">An optional channel name to contextualize the
        /// nature of the message.</param>
        /// <param name="format">The message to emit.</param>
        /// <param name="args">The arguments to emit.</param>
        public virtual void Log(Severity severity, string? tag, string format, params object?[]? args)
            => m_parent.Log(severity, tag, format, args);

        /// <summary>
        /// Log a potentially transformed message to the parent logger. 
        /// </summary>
        /// <param name="severity">The severity associated with this 
        /// message.</param>
        /// <param name="format">The message to emit.</param>
        /// <param name="args">The arguments to emit.</param>
        public void Log(Severity severity, string format, params object?[]? args)
            => Log(severity, null, format, args);

        #endregion ILogger
    }

    /// <summary>
    /// A proxy logger that implements <c>IStackLogger</c>.
    /// </summary>
    public class StackLogger : ProxyLogger<ILogger>, IStackLogger {
        private Stack<ILogger> m_frames;

        #region Constructors

        /// <summary>
        /// Construct a new <paramref name="fallback"/>
        /// </summary>
        /// <param name="fallback">The fallback logger to use.</param>
        public StackLogger(ILogger fallback) : base(fallback) {
            this.m_frames = new Stack<ILogger>();
        }

        #endregion Constructors

        #region Logging

        /// <inheritdoc cref="ProxyLogger{P}.Log(Severity, string?, string, object?[]?)"/>
        public sealed override void Log(Severity severity, string? tag, string format, params object?[]? args) {
            m_parent.Log(severity, tag, format, args);
        }

        #endregion Logging

        #region Stack Management

        /// <inheritdoc cref="IStackLogger.Push(ILogger)"/>
        public void Push(ILogger logger) {
            this.m_frames.Push(logger);
        }

        /// <inheritdoc cref="IStackLogger.Pop()"/>
        public ILogger? Pop() {
            if (m_frames.TryPop(out ILogger? result))
                return result;
            else
                return null;
        }

        #endregion Stack Management
    }

    /// <summary>
    /// Utilities for working with I/O.
    /// </summary>
    public static partial class IOUtils {
        #region Logging Facades

        public static void LogTrivial(this ILogger? logger, string? tag,
            string format, params object?[]? args)
                => logger.Log(Severity.TRIVIAL, tag, format, args);

        public static void LogTrivial(this ILogger? logger, string format,
            params object?[]? args)
                => logger.Log(Severity.TRIVIAL, format, args);

        public static void LogInfo(this ILogger? logger, string? tag,
            string format, params object?[]? args)
                => logger.Log(Severity.INFO, tag, format, args);

        public static void LogInfo(this ILogger? logger, string format,
            params object?[]? args)
                => logger.Log(Severity.INFO, format, args);

        public static void LogWarning(this ILogger? logger, string? tag,
            string format, params object?[]? args)
                => logger.Log(Severity.WARNING, tag, format, args);

        public static void LogWarning(this ILogger? logger, string format,
            params object?[]? args)
                => logger.Log(Severity.WARNING, format, args);

        public static void LogError(this ILogger? logger, string? tag,
            string format, params object?[]? args)
                => logger.Log(Severity.ERROR, tag, format, args);

        public static void LogError(this ILogger? logger, string format,
            params object?[]? args)
                => logger.Log(Severity.ERROR, format, args);

        public static void LogFatal(this ILogger? logger, string? tag,
            string format, params object?[]? args)
                => logger.Log(Severity.INFO, tag, format, args);

        public static void LogFatal(this ILogger? logger, string format,
            params object?[]? args)
                => logger.Log(Severity.INFO, format, args);

        #endregion Logging Facads
    }
}
