/*
 This file is part of the Japes Programming Language.

 Japes is free software: you can redistribute it and/or modify
 it under the terms of the GNU Lesser General Public License as
 published by the Free Software Foundation, either version 3 of
 the License, or (at your option) any later version.

 Japes is distributed in the hope that it will be useful,
 but WITHOUT ANY WARRANTY; without even the implied warranty of
 MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 GNU General Public License for more details.

 You should have received a copy of the GNU Lesser General Public
 License along with Japes.  If not, see <http://www.gnu.org/licenses/>.
*/

/* WARNING: ALL MEMBERS IN THIS API ARE SUBJECT TO MAJOR REVISION UNTIL BOOTSTRAPPED. */

using japes.toolchain.api.io;

namespace japes.toolchain.api.pipeline {

    /// <summary>
    /// An interface representing a pipeline's environment context.
    /// </summary>
    public interface IEnvironment : IStackLogger {

        #region Logging

        /// <summary>
        /// Pop the last logger off of the logger stack.
        /// </summary>
        /// <returns></returns>
        public ILogger? PopLogger();

        ILogger? IStackLogger.Pop()
            => this.PopLogger();

        #endregion Logging
    }

    public interface IEnvironment<T, U> : IEnvironment {

    }

    public abstract class AbstractEnvironment<T, U> : IEnvironment {
        private readonly IStackLogger m_logger;

        #region Constructors

        public AbstractEnvironment(IStackLogger logger) {
            this.m_logger = logger;
        }

        public AbstractEnvironment(ILogger logger) {
            this.m_logger = new StackLogger(logger);
        }

        #endregion Constructors

        #region Logger

        public void Log(ILogger.Severity severity, string? tag, string format, params object?[]? args)
            => this.m_logger.Log(severity, tag, format, args);

        public void Log(ILogger.Severity severity, string format, params object?[]? args)
            => this.m_logger.Log(severity, null, format, args);

        public ILogger? PopLogger()
            => m_logger.Pop();

        public void Push(ILogger logger)
            => m_logger.Push(logger);


        #endregion Logger


    }
}
