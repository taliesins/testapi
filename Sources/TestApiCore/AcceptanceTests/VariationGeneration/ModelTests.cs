﻿// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Test.VariationGeneration;
using Xunit;

namespace Microsoft.Test.AcceptanceTests.VariationGeneration
{
    public class ModelTests
    {
        [Fact]
        public void BasicModelTest()
        {
            var parameters = new List<ParameterBase>
            {
                new Parameter<string>("P1")
                {
                    "one",
                    "two",
                },
                new Parameter<int>("P2")
                {
                    1,
                    2
                },
                new Parameter<double>("P3")
                {
                    1.0,
                    2.0,
                    3.14
                }
            };

            Model model = new Model(parameters);

            Assert.True(model.GenerateVariations(3).Count() == 2 * 2 * 3, "Incorrect number of variations");

            var expectedVariations = new List<string>
            {
                "one 1 1",
                "one 2 2",
                "one 1 3.14",
                "two 2 1",
                "two 1 2",
                "two 2 3.14"
            };

            var actualVariations = WriteVariations(model.GenerateVariations(2));

            Assert.True(expectedVariations.Count == actualVariations.Count, "Expected: " + expectedVariations.Count + " Actual: " + actualVariations.Count);

            for (int i = 0; i < expectedVariations.Count; i++)
            {
                Assert.True(expectedVariations[i] == actualVariations[i], "Expected: " + expectedVariations[i] + " Actual: " + actualVariations[i]);
            }
        }



        [Fact]
        public void LargeModelTest()
        {
            var parameters = new List<ParameterBase>
            {
                new Parameter<string>("Type")
                {
                    "Single", "Spanned", "Striped", "Mirror", "RAID-5"
                },
                new Parameter<int>("Size")
                {
                    10, 100, 1000, 10000, 40000
                },
                new Parameter<FormatMethod>("Format Method")
                {
                    FormatMethod.Quick, FormatMethod.Slow
                },
                new Parameter<string>("File System")
                {
                    "FAT", "FAT32", "NTFS"
                },
                new Parameter<int>("Cluster Size")
                {
                    512, 1024, 2048, 4096, 8192, 16384
                },
                new Parameter<bool>("Compression")
                {
                    true, false
                }
            };

            Model model = new Model(parameters);

            Assert.True(model.GenerateVariations(6).Count() == 5 * 5 * 2 * 3 * 6 * 2);

            var expectedVariations = new List<string>
            {
                "Single 10 Quick FAT 512 True",
                "Single 100 Slow FAT32 1024 False",
                "Single 1000 Quick NTFS 2048 False",
                "Single 10000 Quick FAT32 4096 True",
                "Single 40000 Quick FAT 8192 False",
                "Single 10 Slow NTFS 16384 True",
                "Spanned 1000 Slow FAT 512 True",
                "Spanned 10000 Quick NTFS 1024 False",
                "Spanned 10 Slow FAT32 2048 False",
                "Spanned 40000 Slow NTFS 4096 True",
                "Spanned 100 Quick NTFS 8192 True",
                "Spanned 100 Quick FAT 16384 False",
                "Striped 40000 Quick FAT32 512 False",
                "Striped 10000 Slow FAT 1024 True",
                "Mirror 10 Quick FAT 1024 True",
                "RAID-5 10 Quick FAT 4096 False",
                "Mirror 10 Slow FAT32 8192 False",
                "Mirror 100 Slow NTFS 512 True",
                "RAID-5 100 Slow FAT32 2048 True",
                "Striped 100 Slow NTFS 4096 True",
                "RAID-5 1000 Slow NTFS 1024 False",
                "Mirror 1000 Slow FAT32 4096 False",
                "Striped 1000 Quick FAT 8192 True",
                "Striped 1000 Slow FAT32 16384 True",
                "RAID-5 10000 Quick FAT 512 False",
                "Mirror 10000 Quick FAT 2048 True",
                "RAID-5 10000 Quick NTFS 8192 True",
                "Mirror 10000 Slow FAT 16384 False",
                "Striped 10 Quick FAT32 2048 False",
                "Mirror 40000 Slow NTFS 1024 False",
                "RAID-5 40000 Quick FAT32 2048 True",
                "RAID-5 40000 Quick FAT 16384 False",
            };

            var actualVariations = WriteVariations(model.GenerateVariations(2));

            Assert.True(expectedVariations.Count == actualVariations.Count, "Expected: " + expectedVariations.Count + " Actual: " + actualVariations.Count);

            for (int i = 0; i < expectedVariations.Count; i++)
            {
                Assert.True(expectedVariations[i] == actualVariations[i], "Expected: " + expectedVariations[i] + " Actual: " + actualVariations[i]);
            }
        }

        [Fact]
        public void WeightTest()
        {
            var type = new Parameter<string>("Type")
            {
                "Single", 
                "Spanned",
                "Striped",
                "Mirror",
                "RAID-5"
            };

            var fileSystem = new Parameter<string>("File System")
            {
                "FAT", "FAT32", "NTFS"
            };

            var size = new Parameter<int>("Size")
            {
                10, 100, 1000, 10000, 40000
            };

            var clusterSize = new Parameter<int>("Cluster Size")
            {
                512, 1024, 2048, 4096, 8192, 16384
            };

            var value = new ParameterValue<FormatMethod>(FormatMethod.Quick) { Weight = 1.0 };
            var parameters = new List<ParameterBase>
            {
                type,
                size,
                new Parameter<FormatMethod>("Format Method")
                {
                    value, FormatMethod.Slow
                },
                fileSystem,
                clusterSize,
                new Parameter<bool>("Compression")
                {
                    true, false
                }
            };

            Model model = new Model(parameters);

            var defaultCount = model.GenerateVariations(2).Count((v) => (FormatMethod)v["Format Method"] == FormatMethod.Quick);

            value.Weight = 5.0;
            var emphasizedCount = model.GenerateVariations(2).Count((v) => (FormatMethod)v["Format Method"] == FormatMethod.Quick);

            value.Weight = 0.5;
            var deemphasizedCount = model.GenerateVariations(2).Count((v) => (FormatMethod)v["Format Method"] == FormatMethod.Quick);

            Assert.True(emphasizedCount > defaultCount);
            Assert.True(defaultCount > deemphasizedCount);
        }

        [Fact]
        public void LargeModel3WiseWithSimpleConstraintsTest()
        {
            var os = new Parameter<string>("OS") { "Windows XP SP3", "Windows Vista SP1", "Windows 7", "Windows Server 2003 SP2", "Windows Server 2008 R2" };
            var language = new Parameter<string>("language") { "ARA", "CHS", "CHT", "CSY", "DAN", "DEU", "ELL", "ENG", "ESN", "FIN", "FRA", "HEB", "HUN", "ITA", "JPN", "KOR", "NLD", "NOR", "PLK", "PSE", "PTB", "PTG", "RUS", "SVE", "TRK" };
            var sysLocale = new Parameter<string>("sysLocale") { "SameAsOsLanguage", "TRK" };
            var flavor = new Parameter<string>("flavor") { "fre", "chk" };
            var platform = new Parameter<string>("platform") { "x86", "x64", "x64wow" };
            var ieVersion = new Parameter<string>("ieVersion") { "osDefault", "ie7", "ie8" };
            var highDpi = new Parameter<string>("hiDpi") { "yes", "no" };
            var theme = new Parameter<string>("theme") { "Classic", "Luna", "Royale", "Classic High Contrast", "Aero Basic", "Aero Glass" };
            var sxs = new Parameter<string>("sxs") 
            {
                "3.5 SP1 + 4 (3.5 tests)",
                "3.5 SP1 + 4 (4 tests)",
                "3.5 SP1 + 4 - 4 (3.5 tests)",
                "4 + Mock 4.5 (4 tests)",
                "4 + Mock 5 (4 tests)",
                "4 + 3.5 SP1 (4 tests)",
                "4 + 3.5 SP1 (3.5 tests)",
                "4 + 3.5 SP1 - 4 (3.5 tests)",
                "4 (4 tests)",
            };
            List<ParameterBase> parameters = new List<ParameterBase> { os, language, sysLocale, flavor, platform, ieVersion, highDpi, theme, sxs };

            List<Constraint<Variation>> constraints = new List<Constraint<Variation>>
            {
                Constraint<Variation>
                    .If(v=> os.GetValue(v) == "Windows XP SP3" || os.GetValue(v) == "Windows Server 2003 SP2")
                    .Then(v=> theme.GetValue(v) != "Aero Basic"),
                Constraint<Variation>
                    .If(v=> os.GetValue(v) == "Windows XP SP3" || os.GetValue(v) == "Windows Server 2003 SP2")
                    .Then(v=> theme.GetValue(v) != "Aero Glass"),
                Constraint<Variation>
                    .If(v=> os.GetValue(v) == "Windows 7" || os.GetValue(v) == "Windows Server 2008 R2")
                    .Then(v=> theme.GetValue(v) != "Luna"),
            };

            Model m = new Model(parameters, constraints);
            Assert.Equal(m.GenerateVariations(3).Count(), 1423);
        }

        public static IList<string> WriteVariations(IEnumerable<Variation> variations)
        {
            List<string> strings = new List<string>();
            foreach (var v in variations)
            {
                StringBuilder builder = new StringBuilder();
                foreach (var value in v)
                {
                    builder.Append(value.Value.ToString() + " ");
                }

                strings.Add(builder.ToString().TrimEnd());
            }

            return strings;
        }
    }

    public enum FormatMethod
    {
        Quick,
        Slow
    }
}
