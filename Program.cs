using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using TodoApi;

var builder = WebApplication.CreateBuilder(args);

// הוספת השירותים לפני Build
//builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Todo API",
        Version = "v1"
    });
});
// builder.Services.AddCors(options =>
// {
//     options.AddPolicy("AllowAll", policy =>
//     {
//         policy.AllowAnyOrigin()
//               .AllowAnyMethod()
//               .AllowAnyHeader();
//     });
// });
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
// הוספת DbContext
var connectionString = builder.Configuration.GetConnectionString("todolist");
if (string.IsNullOrEmpty(connectionString))
{
    Console.WriteLine("Connection string is missing!");
    return;
}

builder.Services.AddDbContext<ToDoDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

//הגדרת Authentication ו-Authorization
string secretKey = builder.Configuration["JWT:SecretKey"] ?? throw new ArgumentNullException("JWT:SecretKey is missing");
var key = Encoding.ASCII.GetBytes(secretKey); // Secret Key
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

builder.Services.AddAuthorization();


var app = builder.Build();
app.UseCors("AllowAll");
app.UseAuthentication(); 
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
// app.MapControllers();
// app.UseRouting();


app.MapGet("/", async (ToDoDbContext dbContext) =>
{
    if (dbContext == null)
    {
        return Results.Problem("Database context is not available.");
    }

    var items = await dbContext.Items.ToListAsync();
    return Results.Json(items);
}).WithName("GetItems")
.Produces<List<Item>>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status500InternalServerError);

// הגדרת שאר הפעולות (POST, PUT, DELETE)
app.MapPost("/", [Microsoft.AspNetCore.Authorization.Authorize] async (ToDoDbContext dbContext, Item newItem) =>
{
    if (dbContext == null)
    {
        return Results.Problem("Database context is not available.");
    }
    dbContext.Items.Add(newItem);
    await dbContext.SaveChangesAsync();
    return Results.Created($"/{newItem.Id}", newItem);
});

app.MapPut("/{id}", [Microsoft.AspNetCore.Authorization.Authorize] async (int id, ToDoDbContext dbContext, Item updatedItem) =>
{
    if (dbContext == null)
    {
        return Results.Problem("Database context is not available.");
    }

    var item = await dbContext.Items.FindAsync(id);
    if (item == null)
    {
        return Results.NotFound(new { Message = "Todo item not found." });
    }
    if (!string.IsNullOrEmpty(updatedItem.Name)) item.Name = updatedItem.Name;
    item.IsComplete = updatedItem.IsComplete;
    await dbContext.SaveChangesAsync();
    return Results.Json(item);
});

app.MapDelete("/{id}", [Microsoft.AspNetCore.Authorization.Authorize] async (int id, ToDoDbContext dbContext) =>
{
    if (dbContext == null)
    {
        return Results.Problem("Database context is not available.");
    }
    var item = await dbContext.Items.FindAsync(id);
    if (item == null)
    {
        return Results.NotFound(new { Message = "Todo item not found." });
    }

    dbContext.Items.Remove(item);
    await dbContext.SaveChangesAsync();
    return Results.Ok(new { Message = "Todo item deleted." });
});

app.MapPost("/login", async (User loginRequest, ToDoDbContext dbContext) =>
{
    var user = await dbContext.Users.SingleOrDefaultAsync(u => u.Username == loginRequest.Username);
    if (user == null)
    {
        return Results.NotFound(new { Message = "User not found. Please register." });
    }
    
    if (!BCrypt.Net.BCrypt.Verify(loginRequest.Password, user.Password))
    {
        return Results.Unauthorized();
    }

    var claims = new[]
    {
        new Claim(ClaimTypes.Name, user.Username),
        new Claim("UserId", user.Id.ToString())
    };

    var tokenDescriptor = new SecurityTokenDescriptor
    {
        Subject = new ClaimsIdentity(claims),
        Expires = DateTime.UtcNow.AddHours(1),
        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
    };

    var tokenHandler = new JwtSecurityTokenHandler();
    var token = tokenHandler.CreateToken(tokenDescriptor);

    return Results.Json(new { Token = tokenHandler.WriteToken(token) });
});

 // Register Endpoint
app.MapPost("/register", async (User registerRequest, ToDoDbContext dbContext) =>
{
   
    if (string.IsNullOrEmpty(registerRequest.Username) || string.IsNullOrEmpty(registerRequest.Password))
    {
        return Results.BadRequest("Username and password are required.");
    }

    var existingUser = await dbContext.Users.AnyAsync(u => u.Username == registerRequest.Username);
    if (existingUser)
    {
        return Results.Conflict("Username already exists.");
    }

    var hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerRequest.Password);

    var newUser = new User
    {
        Username = registerRequest.Username,
        Password = hashedPassword
    };

    dbContext.Users.Add(newUser);
    await dbContext.SaveChangesAsync();

    return Results.Ok("User registered successfully.");
});

// הרצת האפליקציה
app.Run();
