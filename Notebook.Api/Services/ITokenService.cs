using Notebook.Api.Models;
using System.IdentityModel.Tokens.Jwt;

namespace Notebook.Api.Services
{
    public interface ITokenService
    {
        public string CreateForUser(NotebookUser user);
    }
}
