using Framework;

namespace V2.Application.Messages
{
    public interface ITemplateEngine
    {
        OperationResult<string> BuildTitle<TModel>(MessageSpecification<TModel> spec, TModel model, string lang)
            where TModel : MessageModel;

        OperationResult<string> BuildBody<TModel>(MessageSpecification<TModel> spec, TModel model, string lang)
            where TModel : MessageModel;
    }
}