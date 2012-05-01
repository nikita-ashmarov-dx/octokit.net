﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Xunit.Extensions;
using Moq;
using Burr.Http;
using Burr.Tests.TestHelpers;

namespace Burr.Tests
{
    public class UsersEndpointTests
    {
        static Func<Task<IResponse<User>>> fakeUserResponse =
            new Func<Task<IResponse<User>>>(() => Task.FromResult<IResponse<User>>(new Response<User> { BodyAsObject = new User() }));

        public class TheConstructor
        {
            [Fact]
            public void ThrowsForBadArgs()
            {
                Assert.Throws<ArgumentNullException>(() => new UsersEndpoint(null));
            }
        }

        public class TheGetAsyncMethod
        {
            [Fact]
            public async Task GetsAuthenticatedUserWithBasic()
            {
                var endpoint = "/user";
                var c = new Mock<IConnection>();
                c.Setup(x => x.GetAsync<User>(endpoint)).Returns(fakeUserResponse);
                var client = new GitHubClient
                {
                    Login = "tclem",
                    Password = "pwd",
                    Connection = c.Object
                };

                var user = await client.Users.GetAsync();

                user.Should().NotBeNull();
                c.Verify(x => x.GetAsync<User>(endpoint));
            }

            [Fact]
            public async Task GetsAuthenticatedUserWithToken()
            {
                var endpoint = "/user";
                var c = new Mock<IConnection>();
                c.Setup(x => x.GetAsync<User>(endpoint)).Returns(fakeUserResponse);
                var client = new GitHubClient
                {
                    Token = "xyz",
                    Connection = c.Object
                };

                var user = await client.Users.GetAsync();

                user.Should().NotBeNull();
                c.Verify(x => x.GetAsync<User>(endpoint));
            }

            [Fact]
            public async Task ThrowsIfGivenNullUser()
            {
                try
                {
                    var user = await (new GitHubClient { Token = "axy" }).Users.UpdateAsync(null);

                    Assert.True(false, "ArgumentNullException was not thrown");
                }
                catch (ArgumentNullException)
                {
                }
            }

            [Fact]
            public async Task ThrowsIfNotAuthenticated()
            {
                try
                {
                    var user = await new GitHubClient().Users.GetAsync();

                    Assert.True(false, "AuthenticationException was not thrown");
                }
                catch (AuthenticationException)
                {
                }
            }
        }

        public class TheUpdateAsyncMethod
        {
            [Fact]
            public async Task UpdatesAuthenticatedUserWithBasic()
            {
                var endpoint = "/user";
                var c = new Mock<IConnection>();
                c.Setup(x => x.PatchAsync<User>(endpoint, It.IsAny<UserUpdate>())).Returns(fakeUserResponse);
                var client = new GitHubClient
                {
                    Login = "tclem",
                    Password = "pwd",
                    Connection = c.Object
                };

                var user = await client.Users.UpdateAsync(new UserUpdate { Name = "Tim" });

                user.Should().NotBeNull();
                c.Verify(x => x.PatchAsync<User>(endpoint, It.IsAny<UserUpdate>()));
            }

            [Fact]
            public async Task UpdatesAuthenticatedUserWithToken()
            {
                var endpoint = "/user";
                var c = new Mock<IConnection>();
                c.Setup(x => x.PatchAsync<User>(endpoint, It.IsAny<UserUpdate>())).Returns(fakeUserResponse);
                var client = new GitHubClient
                {
                    Token = "xyz",
                    Connection = c.Object
                };

                var user = await client.Users.UpdateAsync(new UserUpdate { Name = "Tim" });

                user.Should().NotBeNull();
                c.Verify(x => x.PatchAsync<User>(endpoint, It.IsAny<UserUpdate>()));
            }

            [Fact]
            public async Task ThrowsIfNotAuthenticated()
            {
                try
                {
                    var user = await new GitHubClient().Users.UpdateAsync(new UserUpdate());

                    Assert.True(false, "AuthenticationException was not thrown");
                }
                catch (AuthenticationException)
                {
                }
            }
        }
    }
}