using Microsoft.Extensions.Http;
using Polly;

namespace XkcdComicFinder.Tests
{
    internal class TestPolicyHttpMessageHandler: PolicyHttpMessageHandler
    {
        //Test provides this Func.
        public Func<HttpRequestMessage, Context,CancellationToken, Task<HttpResponseMessage>>OnSendAsync{ get; set; } = null!;

        //Polly policy
        public TestPolicyHttpMessageHandler(IAsyncPolicy<HttpResponseMessage> policy): base(policy) { }

        //Another constructor
        public TestPolicyHttpMessageHandler(Func<HttpRequestMessage,IAsyncPolicy<HttpResponseMessage>> policySelector): base(policySelector) { }

        //Calls Func
        protected override Task<HttpResponseMessage>SendCoreAsync(
          HttpRequestMessage request,
          Context context,
          CancellationToken cancellationToken)
          => OnSendAsync(request, context, cancellationToken);
    }
}