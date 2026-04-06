namespace PilotIceService.Application.Exceptions
{
    public static class ExExtension
    {
        public static Exception WithData(this Exception ex, string key, object? data)
        {
            if (ex.Data.Contains(key) == false)
            {
                ex.Data.Add(key, data);
            }

            return ex;
        }
    }
}