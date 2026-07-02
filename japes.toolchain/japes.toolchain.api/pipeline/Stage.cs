/*
 This file is part of the Xi Programming Language.

 Xi is free software: you can redistribute it and/or modify
 it under the terms of the GNU Lesser General Public License as
 published by the Free Software Foundation, either version 3 of
 the License, or (at your option) any later version.

 Xi is distributed in the hope that it will be useful,
 but WITHOUT ANY WARRANTY; without even the implied warranty of
 MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 GNU General Public License for more details.

 You should have received a copy of the GNU Lesser General Public
 License along with Xi.  If not, see <http://www.gnu.org/licenses/>.
*/

/* WARNING: ALL MEMBERS IN THIS API ARE SUBJECT TO MAJOR REVISION UNTIL BOOTSTRAPPED. */

using japes.toolchain.api.io;

namespace japes.toolchain.api.pipeline {

    /// <summary>
    /// An interface representing a compiler pipeline.
    /// </summary>
    public interface IPipeline {

        #region Classes

        /// <summary>
        /// An interface specifying an individual stage in the pipeline.
        /// </summary>
        /// <typeparam name="I">The input type of the pipeline.</typeparam>
        /// <typeparam name="O">The output type of the pipeline.</typeparam>
        public interface IStage<I, O> {

            internal void M_InternalContract();

            public O Consume(I input, IEnvironment<I, O> ctx);
        }

        /// <summary>
        /// An interface representinging a source preprocessing stage.
        /// </summary>
        public interface IPreprocessorStage : IStage<ITextSource, ITextSink> {
            internal void M_IternalContract() { }
        }

        #endregion Classes
    }
}
