using System;
using Caravel.Entities;

namespace CaravelTemplate.Infrastructure.Authentication
{
    public class RefreshToken : Entity
    {
        public string Token { get; private set; }
        public DateTime Expires { get; private set; }
        public Guid UserId { get; private set; }
        public bool Active => DateTime.UtcNow <= Expires;

        public RefreshToken(string token, DateTime expires, Guid userId)
        {
            Token = token;
            Expires = expires;
            UserId = userId;
        }
    }
}