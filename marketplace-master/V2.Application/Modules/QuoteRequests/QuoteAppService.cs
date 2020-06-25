using Framework;
using V2.Application.Modules.QuoteRequests.AppCommands;

namespace V2.Application.Modules.QuoteRequests
{
    public class QuoteAppService
    {
        private readonly AddQuoteAnswerAppCommandHandler _addQuoteAnswerAppCommandHandler;
        private readonly RequestQuoteAppCommandHandler _requestQuoteAppCommandHandler;

        public QuoteAppService(
            AddQuoteAnswerAppCommandHandler addQuoteAnswerAppCommandHandler,
            RequestQuoteAppCommandHandler requestQuoteAppCommandHandler)
        {
            _addQuoteAnswerAppCommandHandler = addQuoteAnswerAppCommandHandler;
            _requestQuoteAppCommandHandler = requestQuoteAppCommandHandler;
        }

        public OperationResult AddAnswer(AddQuoteAnswerAppCommand command)
        {
            return _addQuoteAnswerAppCommandHandler.Handle(command);
        }

        public OperationResult RegisterQuoteRequest(RequestQuoteAppCommand command)
        {
            return _requestQuoteAppCommandHandler.Handle(command);
        }
    }
}
