// See https://aka.ms/new-console-template for more information

using BenchmarkDotNet.Running;
using LightJS.Benchmarks;

var summary = BenchmarkRunner.Run<AstBenchmarks>();
// Console.WriteLine("hi");