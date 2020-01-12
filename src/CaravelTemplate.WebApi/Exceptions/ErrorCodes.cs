using Caravel.Exceptions;

namespace CaravelTemplate.WebApi.Exceptions
{
    public static class ErrorCodes
    {
        // Not Found Errors
        public static readonly Error DeviceNotFound = new Error(30001, "Device does not exist");
    }
}