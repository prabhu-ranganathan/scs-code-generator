namespace SCSCodeGen.Services
{
    public interface IServiceFactory
    {
        ICodeGenerationService CreateCodeGenerationService();
    }
}
