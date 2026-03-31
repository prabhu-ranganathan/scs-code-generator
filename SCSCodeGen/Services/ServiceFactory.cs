namespace SCSCodeGen.Services
{
    public class ServiceFactory : IServiceFactory
    {
        public ICodeGenerationService CreateCodeGenerationService()
        {
            return new CodeGenerationService(
                CreateYamlDataService(),
                CreateMappingService(),
                CreateGenerationService()
            );
        }

        private IYamlDataService CreateYamlDataService() => new YamlDataService();
        private ISitecoreMappingService CreateMappingService() => new SitecoreMappingService();
        private ITemplateGenerationService CreateGenerationService() => new TemplateGenerationService();
    }
}
