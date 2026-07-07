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

using japes.io;
using japes.toolchain.pipeline;

namespace __japesc {



    public sealed class Driver {

        #region Classes


        #endregion Classes

        #region Properties

        public static Driver Instance { 
            get;
            private set;
        }

        /// <summary>
        /// The raw arguments supplied to this constructor.
        /// </summary>
        public static IReadOnlyList<string> RawArgs { get; private set; }

        public bool IsOpen => throw new NotImplementedException();

        public static ISource<string> Arguments { get; private set; }

        #endregion Properties

        #region Options Parsing



        #endregion Options Parsing

        #region Constructor

        
        #endregion Constructor

        #region Pipeline Construction


        #endregion Pipeline Construction

        #region Options Parsing

        public void ParseArguments(string[] args) {
            throw new NotImplementedException();
        }

        #endregion Options Parsing

        #region Entry

        private Driver(string[] args) {
            Driver.Instance = this;
 
        }

        public static void Main(string[] args) {
            Driver driver = new Driver(args);
            Environment.Exit(driver.M_Run().Result);  
        }
        
        private async ValueTask<int> M_Run() {
            return 0;
        }
         
        #endregion Entry
    }
}