namespace Student_Att.Business
{
    // A simple "did it work + message" answer that services give back to pages
    public class ServiceResult
    {
        public bool Success { get; private set; }
        public string Message { get; private set; }

        public static ServiceResult Ok(string message)
        {
            ServiceResult result = new ServiceResult();
            result.Success = true;
            result.Message = message;
            return result;
        }

        public static ServiceResult Fail(string message)
        {
            ServiceResult result = new ServiceResult();
            result.Success = false;
            result.Message = message;
            return result;
        }
    }
}
