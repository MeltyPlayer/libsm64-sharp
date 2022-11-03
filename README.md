# libsm64 - C# Bindings (WIP)

[![Nuget](https://img.shields.io/nuget/v/libsm64-sharp)](https://www.nuget.org/packages/libsm64-sharp)

## Overview

Bindings for calling [libsm64](https://github.com/libsm64/libsm64) from C#.

This library provides both:
- [libsm64-ext](https://github.com/MeltyPlayer/libsm64-ext)'s interop layer for calling libsm64 methods directly.
- A C# object-oriented layer for safely calling the libsm64 methods.

## Credits

- [libsm64](https://github.com/libsm64/libsm64), the original C library that provides this functionality.
- [libsm64-unity](https://github.com/libsm64/libsm64-unity), which I shamelessly copied the core interop code from.
- [Quad64](https://github.com/DavidSM64/Quad64), which I shamelessly copied into this project to load levels.