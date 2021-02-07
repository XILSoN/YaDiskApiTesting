namespace YaDiskApiTesting.Apis
{
    using System.Threading.Tasks;
    using Refit;
    using YaDiskApiTesting.Dto;

    public interface IYandexDiskApi
    {
        [Get("/v1/disk/resources/upload?path={path}&overwrite={overwrite}")]
        Task<ApiResponse<Link>> UploadAsync(string path, [Header("Authorization")]string authorizationToken, bool overwrite = false);
    }
}
