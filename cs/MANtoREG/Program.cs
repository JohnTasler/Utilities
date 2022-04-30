using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace MANtoREG
{
    class Program
    {
        string[] _args;
        string _inputFile;
        string _outputFile;


        static int Main(string[] args)
        {
            var program = new Program(args);
            return program.Run();
        }

        Program(string[] args)
        {
            _args = args;
        }

        int Run()
        {
            var result = this.ParseCommandLine();
            if (result != 0)
                return result;

            this.ProcessFile(_inputFile, _outputFile);

            return 0;
        }

        int ParseCommandLine()
        {
            if (_args.Length == 0 || string.IsNullOrWhiteSpace(_args[0]))
                return ShowSyntax();

            switch (_args[0].ToLowerInvariant())
            {
                case "/?":
                case "-?":
                case "/help":
                case "-help":
                case "help":
                    return ShowSyntax();
            }

            _inputFile = Path.GetFullPath(_args[0]);
            _outputFile = _args.Length > 1 && !string.IsNullOrWhiteSpace(_args[1])
                ? _args[1]
                : Path.ChangeExtension(_inputFile, "reg");

            if (!File.Exists(_inputFile))
                return ShowSyntax($"Specified file does not exist: {_inputFile}");

            try
            {
                this.ProcessFile(_inputFile, _outputFile);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 2;
            }


            return 0;
        }

        static XName MakeXName(string localName)
        {
            return XName.Get(localName, "urn:schemas-microsoft-com:asm.v3");
        }

        int ProcessFile(string input, string output)
        {
            var root = XElement.Load(input);

            var registryKeys =
                from keyElement in root.Elements(MakeXName("registryKeys")).Elements(MakeXName("registryKey"))
                select new
                {
                    KeyName = (string)keyElement.Attribute("keyName"),
                    KeyValues =
                        from keyValue in keyElement.Elements(MakeXName("registryValue"))
                        select new
                        {
                            Name = (string)keyValue.Attribute("name"),
                            Value = (string)keyValue.Attribute("value"),
                            ValueType = (string)keyValue.Attribute("valueType")
                        }
                };

            using (var writer = File.CreateText(output))
            {
                writer.WriteLine("Windows Registry Editor Version 5.00");

                foreach (var entry in registryKeys)
                {
                    writer.WriteLine();
                    writer.WriteLine($"[{entry.KeyName}]");

                    foreach (var value in entry.KeyValues)
                    {
                        writer.WriteLine($"{FormatValueName(value.Name)}={TranslateValue(value.ValueType, value.Value)}");
                    }
                }
            }

            return 0;
        }

        static string FormatValueName(string valueName)
        {
            return string.IsNullOrWhiteSpace(valueName) ? "@" : $"\"{valueName}\"";
        }

        static string TranslateValue(string regType, string value)
        {
            switch (regType)
            {
                case "REG_SZ" :
                {
                    var newValue = value.Replace("$(runtime.system32)", @"%SystemRoot%\System32");
                    if (newValue != value)
                        return TranslateValue("REG_EXPAND_SZ", newValue);

                    value = $"\"{newValue.Replace("\"", "\\\"")}\"";
                    break;
                }

                case "REG_EXPAND_SZ":
                case "REG_MULTI_SZ" :
                {
                    value = string.Join(",", value
                        .SelectMany(c => BitConverter.GetBytes(c))
                        .Select(b => string.Format("{0:X2}", b)));
                    break;
                }

                case "REG_BINARY":
                {
                    value = string.Join(",", value.ToBinary()
                        .Select(b => string.Format("{0:X2}", b)));
                    break;
                }

                case "REG_QWORD":
                {
                    if (value.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                    {
                        value = value.Substring(2);
                    }

                    var result = ulong.Parse(value, NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo);
                    value = string.Join(",", BitConverter.GetBytes(result)
                        .Select(b => string.Format("{0:X2}", b)));
                    break;
                }

                case "REG_DWORD":
                {
                    if (value.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                    {
                        value = value.Substring(2);
                    }

                    var result = uint.Parse(value, NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo);
                    value = result.ToString("X8");
                    break;
                }
            }

            return ConvertRegTypeToTag(regType) + value;
        }

        static string ConvertRegTypeToTag(string regType)
        {
            switch (regType)
            {
                case "REG_BINARY"   : return "hex:";
                case "REG_DWORD"    : return "dword:";
                case "REG_QWORD"    : return "hex(b):";
                case "REG_EXPAND_SZ": return "hex(2):";
                case "REG_MULTI_SZ" : return "hex(7):";
            }

            return "";
        }

        int ShowSyntax(string errorMessage = null)
        {
            if (!string.IsNullOrWhiteSpace(errorMessage))
                Console.WriteLine($"ERROR: {errorMessage}{System.Environment.NewLine}");

            Console.WriteLine("SYNTAX:    MANtoREG inputManifestFile [outputRegistryFile]");

            return 1;
        }
    }

    public static class StringExtensions
    {
        public static IEnumerable<byte> ToBinary(this string @this)
        {
            var length = @this.Length;
            for (int index = 0; index < length; index += 2)
            {
                var byteString = @this.Substring(index, 2);
                var value = byte.Parse(byteString, NumberStyles.AllowHexSpecifier, NumberFormatInfo.InvariantInfo);
                yield return value;
            }
        }
    }
}
