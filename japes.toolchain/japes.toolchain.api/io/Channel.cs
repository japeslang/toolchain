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

using NetTextReader = System.IO.TextReader;
using NetTextWriter = System.IO.TextWriter; 

namespace japes.toolchain.api.io {

    #region Interfaces

    /// <summary>
    /// An interface specifying a stream of objects.
    /// </summary>
    public interface IChannel : IDisposable {

        #region Properties

        #endregion Properties

        internal void M_InternalContract();

        #region Logging

        /// <summary>
        /// Wrap a logger so that it does not 
        /// </summary>
        /// <param name="logger"></param>
        /// <returns></returns>
        public ILogger WrapLogger(ILogger logger)
            => logger;

        #endregion Logging
    }

    /// <summary>
    /// An interface representing an input channel of <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of value to read.</typeparam>
    public interface ISource<T> : IChannel {

        void IChannel.M_InternalContract() { }

        #region Properties

        /// <summary>
        /// Determine whether or not the stream can be read.
        /// </summary>
        public bool CanRead { get; }

        #endregion Properties

        #region I/O

        /// <summary>
        /// Attempt to extract the element from this source.
        /// </summary>
        /// <param name="value">The corresponding element.</param>
        /// <returns>Whether or not value is valid.</returns>
        public bool TryRead(out T value) {
            try {
                value = Read();
                return true;
            }
            catch (Exception) {
                value = default!;
                return false;
            }
        }

        /// <summary>
        /// Extract the next element from this source.
        /// </summary>
        /// <returns>The corresponding element.</returns>
        public T Read();

        #endregion I/O
    }

    /// <summary>
    /// A refinement of <c>ISource&lt;T&gt;</c> that represents a textual source.
    /// </summary>
    public interface ITextSource : ISource<char> {
        
    }

    /// <summary>
    /// An interface representing an output channel of <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of value to write.</typeparam>
    public interface ISink<T> : IChannel {

        void IChannel.M_InternalContract() { }

        #region Properties

        /// <summary>
        /// Determine whether or not the stream can be written.
        /// </summary>
        public bool CanWrite { get; }

        #endregion Properties

        #region I/O

        /// <summary>
        /// Attempt to flush any pending data to the underlying stream.
        /// </summary>
        public void Flush() { }

        /// <summary>
        /// Insert the next element into the sink.
        /// </summary>
        /// <param name="value"></param>
        public void Write(T value);

        #endregion I/O
    }

    /// <summary>
    /// A refinement of <c>ISink&lt;T&gt;</c> that represents a textual sink.
    /// </summary>
    public interface ITextSink : ISink<char> {

    }

    #endregion Interfaces

    #region Adapter Classes

    /// <summary>
    /// A concrete implementation of <c>ITextSource</c> which encapsulates a
    /// stream source.
    /// </summary>
    public class TextReader : ITextSource {
        private NetTextReader m_reader;

        #region Properties

        /// <inheritdoc cref="ISource{T}.CanRead"/>
        public bool CanRead => m_reader.Peek() != -1;

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Construct a new <c>TextReader</c> from a string's contents.
        /// </summary>
        /// <param name="value">The string to turn into a </param>
        public TextReader(string value)
            => m_reader = new StringReader(value);

        /// <summary>
        /// Construct a new <c>TextReader</c> from it's .NET 
        /// <paramref name="reader"/> counterpart.
        /// </summary>
        /// <param name="reader">The peer to wrap.</param>
        public TextReader(NetTextReader reader)
            => m_reader = reader;

        /// <summary>
        /// Construct a new <c>TextReader</c> from a file path.
        /// </summary>
        /// <param name="path">The path of the file to read.</param>
        /// <returns>The associated <c>TextReader</c>.</returns>
        public static TextReader FromFilePath(string path) {
            return new TextReader(File.OpenText(path));
        }

        /// <summary>
        /// Dispose of this object.
        /// </summary>
        public virtual void Dispose() 
            => m_reader?.Dispose();
        

        #endregion Constructors

        #region I/O

        /// <inheritdoc cref="ISource{T}.Read()"/>
        public char Read() {
            int ret = m_reader.Read();
            if (ret == -1) {
                throw new IOException("Stream exhausted.");
            }
            else {
                return (char) ret;
            }
        }

        #endregion I/O
    }

    /// <summary>
    /// A concrete implementation of <c>ITextSink</c> which encapsulates a
    /// stream sink.
    /// </summary>
    public class TextWriter : ITextSink {
        private NetTextWriter m_writer;

        #region Properties
        
        /// <inheritdoc cref="ISink{T}.CanWrite"/>
        public bool CanWrite => throw new NotImplementedException();

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Construct a new <c>TextWriter</c> from its 
        /// <<paramref name="writer"/> counterpart.
        /// </summary>
        /// <param name="writer">The peer to wrap.</param>
        public TextWriter(NetTextWriter writer) {
            m_writer = writer;
        }

        /// <summary>
        /// Construct a new <c>TextWriter</c> from a file path.
        /// </summary>
        /// <param name="path">The path of the file to write.</param>
        /// <param name="append">Whether or not to open in append mode.</param>
        /// <returns></returns>
        public TextWriter FromFilePath(string path, bool append = false) {
            NetTextWriter writer;

            if (append) {
                writer = File.AppendText(path);
            }
            else {
                writer = File.CreateText(path);
            }

            return new TextWriter(writer);
        }

        /// <summary>
        /// Dispose of this object.
        /// </summary>
        public void Dispose()
            => m_writer.Dispose();

        #endregion Constructors

        #region I/O

        /// <inheritdoc cref="ISink{T}.Write(T)"/>
        public void Write(char value) 
            => m_writer.Write(value);


        #endregion I/O
    }

    #endregion Adapter Classes
}
