namespace Infrastructure.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class WelcomeMessageToSeller : DbMigration
    {
        public override void Up()
        {
            /* Este migração não modifica a estrutura do banco de dados.
             * 
             * Apenas copia dados de template de email e labels de tradução de uma mensagem
             * que era enviada para comprador e vendedor e agora está sendo separada
             * em mensagens distintas para cada tipo de perfil.
             * 
             * A cópia é feita para que os marketplaces que já customizaram essas mensagens
             * não recebam agora uma mensagem padrão e precisem customizar novamente.
             */

            Sql(@"
                INSERT INTO dbo.MessageTemplate ([Key], Lang, SubjectTemplate, BodyTemplate, [Enabled], CreatedAt)
                SELECT 'WelcomeMessageToSeller', Lang, SubjectTemplate, BodyTemplate, [Enabled], GETUTCDATE()
                FROM dbo.MessageTemplate
                WHERE [Key] = 'WelcomeMessage'");

            Sql(@"
                INSERT INTO dbo.[Str] ([Label], Lang, [Value], CreatedAt)
                SELECT 'WELCOME_MESSAGE_TO_SELLER_TITLE', Lang, [Value], GETUTCDATE() FROM dbo.[Str] WHERE [Label] = 'WELCOME_MESSAGE_TITLE'");
        }

        public override void Down()
        {
            Sql("DELETE FROM dbo.MessageTemplate WHERE [Key] = 'WelcomeMessageToSeller'");

            Sql("DELETE FROM dbo.[Str] WHERE [Label] = 'WELCOME_MESSAGE_TO_SELLER_TITLE'");
        }
    }
}
