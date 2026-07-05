<!-- @import https://github.com/japeslang/japes/blob/main/CLAUDE.md -->
# Context Initialization (Mandatory)

BEFORE starting any task, check your current session context. If the global project guidelines have not been read from `https://github.com/japeslang/japes/blob/main/CLAUDE.md`:

1. Use your system file tools to read `https://github.com/japeslang/japes/blob/main/CLAUDE.md`
2. Merge those global rules within these rules.
3. If they are already in your context window, do not read them to save tokens.Do not continue without it.
4. If you cannot read `https://github.com/japeslang/japes/blob/main/CLAUDE.md` because you are incapable of reading it, do not continue until the user has pasted its contents.

## Fundamental Facts

- This repository is the toolchain repository and contains build tools for compiling the Japes Programming Language.
- `japesc` is the compiler frontend.
- `j4` is the macro preprocessor.
- `jinx` is the build manager.
- The path `src/japes.toolchain.stage0` points to a stage0 compiler that can be used to bootstrap the Japes Programming Language.