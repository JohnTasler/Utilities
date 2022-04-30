// Guids.cs
// MUST match guids.h
using System;

namespace TaslerComputing.EditorOptions
{
    static class GuidList
    {
        public const string guidEditorOptionsPkgString = "5ddf3b50-c9e4-48f8-821a-beceeee45b81";
        public const string guidEditorOptionsCmdSetString = "14429b72-ad5e-420f-a875-6e445907a3ba";

        public static readonly Guid guidEditorOptionsCmdSet = new Guid(guidEditorOptionsCmdSetString);
    };
}