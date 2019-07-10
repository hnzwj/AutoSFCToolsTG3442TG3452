
namespace AutoSFCTools
{
    public class NewLogParameter
    {
        public string Path = string.Empty;
        public string SN = string.Empty;
        public string Result = string.Empty;
        public NewLogParameter(string Serial, string FolderPath, string TestRes)
        {
            Path = FolderPath;
            SN = Serial;
            Result = TestRes;
        }


    }
}
