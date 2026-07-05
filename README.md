# Japes Commpiler Toolchain

This repository implements the Japes Compiler Toolchain. 

## Licensing

The source code associated with this project are licensed under the [GNU General Public License version 3](LICENSE.txt); in order to solve the question about linking against the standard library or the toolchain, [the Classpath exception](claspath.md) is appended to it. You should have recieved a copy of both with this file. 

## Subprojects

The following subprojects are sponsored in this repository:

- `src/japes.toolchain.stage0` - Bootstrap compiler implementation
- `src/japes.j4` - The J4 Macro Preprocessor implementation, stage 1 and beyond
- `src/japes.jinx` - The Jinx build system implementation, stage 1 and beyond
- `src/japesc` - The Japes compiler implementation, stage 1 and beyond 