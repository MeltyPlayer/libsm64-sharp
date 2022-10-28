# libsm64 - C# Bindings (WIP)

[![Nuget](https://img.shields.io/nuget/v/libsm64-sharp)](https://www.nuget.org/packages/libsm64-sharp)

## Overview

C# bindings for [libsm64](https://github.com/libsm64/libsm64).

This library provides both:
- A high-level C# object-oriented layer for safely calling the libsm64 methods.
- A low-level interop layer for calling [libsm64-ext](https://github.com/MeltyPlayer/libsm64-ext)'s methods directly.

## Credits

- [libsm64](https://github.com/libsm64/libsm64), the original C library that provides this functionality.
- [libsm64-unity](https://github.com/libsm64/libsm64-unity), which I shamelessly copied the core interop code from.

## Installation via NuGet

To install `libsm64-sharp`, run the following command in the **Package Manager Console**:

```
Install-Package libsm64-sharp
```

## Usage

### High-level

For a simpler experience working with libsm64, you can use the high-level API. Its entrypoint is the `Sm64Context` class, which provides a more standard C# API for working with libsm64. 

A context must be created from a Super Mario 64 ROM in order to load texture and audio data, like so:

```c#
var romBytes = File.ReadAllBytes("Super_Mario_64_(US).z64");
var sm64Context = Sm64Context.InitFromRom(romBytes);
```

***Note**: The ROM used must be the US version in the Z64 format (i.e. big-endian). To verify if your ROM is correct, make sure its MD5 hash is `20b854b239203baf6c961b850a4a51a2`.*

For a full example, refer to this repository's [Demo Project](https://github.com/MeltyPlayer/libsm64-sharp/tree/main/Demo%20Project).

### Low-level

For direct access to libsm64, you can use the low-level API. This can be accessed via the [`LibSm64Interop`](https://github.com/MeltyPlayer/libsm64-sharp/blob/main/LibSm64Sharp/src/lowlevel/LibSm64Interop.cs) class, which exposes static methods that call the C library.

For a full example, refer to how `Sm64Context` calls these interop methods within [LibSm64Sharp](https://github.com/MeltyPlayer/libsm64-sharp/tree/main/LibSm64Sharp).
