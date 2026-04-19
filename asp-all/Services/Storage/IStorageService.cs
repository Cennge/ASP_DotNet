namespace asp_all.Services.Storage
{
    public interface IStorageService
    {
        public String Save(IFormFile formFile);
        public byte[] Load(String filename);
        public String GetPathPrefix();
    }
}
