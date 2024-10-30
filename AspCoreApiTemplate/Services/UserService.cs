using AspCoreApiTemplate.Data;
using AspCoreApiTemplate.Data.Entities;
using AspCoreApiTemplate.Data.Entities.Authority;
using AspCoreApiTemplate.Services.Interfaces;
using AspCoreApiTemplate.ViewModels;
using AutoMapper;
using Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AspCoreApiTemplate.Services
{
    [Authorize]
    public class UserService : IUserService
    {
        private readonly ILogger<UserService> logger;
        private readonly IExampleDbRepository exampleDbRepository;
        private readonly IMapper mapper;
        private readonly UserManager<AuthorityUser> authorityUser;
        private readonly UserManager<AuthorityUser> userManager;
        private readonly IAtsHttpService _atsHttpService;

        public UserService(UserManager<AuthorityUser> AuthorityUser, IMapper mapper, ILogger<UserService> logger, IExampleDbRepository exampleDbRepository, UserManager<AuthorityUser> userManager, IAtsHttpService atsHttpService)
        {
            this.authorityUser = AuthorityUser;
            this.exampleDbRepository = exampleDbRepository;
            this.mapper = mapper;
            this.logger = logger;
            this.userManager = userManager;
            _atsHttpService = atsHttpService;
        }

        public async Task<OwnerViewModel> FindUserByUserName(string userName)
        {
            var uri = $"https://localhost:44347/api/" + userName;
            using HttpRequestMessage message = new(HttpMethod.Post, uri);
            message.Content = new StringContent(JsonConvert.SerializeObject(userName), Encoding.UTF8, "application/json");

            var response = await _atsHttpService.MakeHttpRequestAsync(message, "FindUserByUserName");

            var content = await response.Content.ReadAsStringAsync();
            var ownerViewModel = JsonConvert.DeserializeObject<OwnerViewModel>(content);
            return ownerViewModel;
        }
    }
}
