# RoGetActivationFactory Project Overview #

RoGetActivationFactory.exe is a small utility to instantiate and inspect the
Activation Factory for a WinRT runtime class.

This project is a Windows Console application, implemented in C++ and using
C++/WinRT to consume WinRT.

## RoGetActivationFactory ##

### Description ###
A small utility to instantiate and inspect the Activation Factory for a
specified WinRT runtime class.

### Syntax: ###
    RoGetActivationFactory.exe activatable_class_name
        [dll_filename [--both | -b | --sta | -s | --mta | -m ]]

### Where: ###
    activatable_class_name  The name of the Windows Runtime Class that you
                            want to inspect.

    dll_filename            Optional. The name of a DLL that implements the
                            specified runtimeclass. If this is not specified,
                            the class name must be a system-registered class.

    --both | -b             Optional. If specified with the dll_filename, the
    --sta  | -s             threading model used for the object. The default
    --mta  | -m             is both.
