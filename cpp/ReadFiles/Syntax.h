#pragma once
#include <string_view>
using namespace std::literals;

//--+----1----+----2----+----3----+----4----+----5----+----6----+----7----+----8
inline static constexpr auto c_syntaxText = LR"%%%(
Syntax:

    READFILES [switches] [filespecs]

Where:

    filespecs           Specifies a list of one or more files to be read.
                        Wildcards may be used to specify multiple files.
    switches:
        -?              Displays this syntax message
        -p:page_count   Specifies the number of pages to read at a time.
                        The default is 512. This value must be a whole number
                        between 1 and 4096, inclusive (4KB - 16MB).
        -t:thread_count Specifies the number number of threads to use.
                        The default is the number of logical threads on the
                        computer. This value must be between one and the
                        number of logical threads on the computer.

Purpose:
    This program simply profiles how long it takes to read the specified files.
    This is useful for profiling the speed of a hard drive, for example, but
    can also be useful for profiling network overhead.

)%%%"sv;
//--+----1----+----2----+----3----+----4----+----5----+----6----+----7----+----8
