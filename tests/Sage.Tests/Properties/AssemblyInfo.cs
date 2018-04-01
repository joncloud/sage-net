using Xunit;

// Run all tests in serial to ensure that swapping of console 
// streams do not have unintended impacts against other tests.
[assembly: CollectionBehavior(DisableTestParallelization = true)]
