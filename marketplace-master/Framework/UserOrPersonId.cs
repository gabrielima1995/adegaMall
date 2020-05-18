using System;

namespace Framework
{
    public static class UserOrPersonId
    {
        public static string Generate() => Guid.NewGuid().ToString();
    }
}
