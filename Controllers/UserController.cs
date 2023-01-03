﻿using ApiSqlAsp.DataContext;
using ApiSqlAsp.Models;
using ApiSqlAsp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureIdentity.Password;

namespace ApiSqlAsp.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("v1")]
    public class UserController : ControllerBase
    {
        private User user;

        [HttpPost("accounts")]
        public async Task<IActionResult> Post(
            [FromBody] User model,
            [FromServices] ApiDataContext context)
        {
            

            await context.Users.AddAsync(model);
            await context.SaveChangesAsync();

            return Ok(model);
        }

        //[AllowAnonymous] // Libera a autorização
        [HttpPost("accounts/login")]
        public async Task<IActionResult> LoginAsync(
            [FromServices] TokenService tokenService,
            [FromBody] Login model,
            [FromServices] ApiDataContext context)
        {
            if (model == null)
                return BadRequest();

            var user = await context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.UserNames == model.UserNames);

            if (user == null)
                return StatusCode(401, "Usuário ou senha Inválida");

            if (!(model.Password == user.Password))
                return StatusCode(401, "Usuário ou Senha inválida");

            try
            {
                var token = tokenService.GenerateToken(user);
                return Ok($"Login Efetuado com Sucesso");
            }
            catch
            {
                return StatusCode(500, "Falha Interna");
            }
        }
    }
}
 