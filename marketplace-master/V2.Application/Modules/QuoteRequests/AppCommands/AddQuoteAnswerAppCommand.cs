using Domain.DTO;
using Domain.Modules.Quotes;
using Domain.QuoteRequests.Commands;
using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using V2.Application.Helpers;

namespace V2.Application.Modules.QuoteRequests.AppCommands
{
    public class AddQuoteAnswerAppCommand
    {
        public Guid QuoteId { get; set; }
        public Guid AnswerId { get; set; }
        public string BuyerId { get; set; }
        public string SellerId { get; set; }
        public string AuthorId { get; set; }
        public string Text { get; set; }
        public decimal? Price { get; set; }
        public DateTime? Deadline { get; set; }
        public IEnumerable<FileUpload> Attachments { get; set; } = new List<FileUpload>();
        public bool IsModerator { get; set; }
    }

    public class AddQuoteAnswerAppCommandHandler
    {
        private readonly FileUploadHelper _fileUploadHelper;
        private readonly QuoteService _quoteService;

        public AddQuoteAnswerAppCommandHandler(
            FileUploadHelper fileUploadHelper,
            QuoteService quoteService)
        {
            _fileUploadHelper = fileUploadHelper;
            _quoteService = quoteService;
        }

        internal OperationResult Handle(AddQuoteAnswerAppCommand command)
        {
            var attachmentsResult = UploadAttachments(command.Attachments);
            if (!attachmentsResult.OK) return attachmentsResult;

            var domainCommand = new AddQuoteAnswerCommand(
                command.QuoteId,
                command.AnswerId,
                command.BuyerId,
                command.SellerId,
                command.AuthorId,
                command.Text,
                command.Price,
                command.Deadline,
                attachmentsResult.Result,
                command.IsModerator);

            var addResult = _quoteService.AddAnswer(domainCommand);
            return addResult;
        }

        private OperationResult<IEnumerable<string>> UploadAttachments(IEnumerable<FileUpload> filesToUpload)
        {
            if (filesToUpload == null || !filesToUpload.Any())
                return new List<string>();

            var uploadResult = _fileUploadHelper.Upload(CloudFileType.QuoteAttachment, filesToUpload);
            if (!uploadResult.OK) return uploadResult.Errors;

            return uploadResult.Result.Select(x => x.ToString()).ToList();
        }
    }
}