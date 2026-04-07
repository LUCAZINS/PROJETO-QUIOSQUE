using Microsoft.EntityFrameworkCore;
// Se o nome do seu projeto for diferente de 'QuiosqueApi', mude aqui embaixo:
using QuiosqueApi.Data; 

var builder = WebApplication.CreateBuilder(args);

// 1. Configura o Banco de Dados PostgreSQL com Pool
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContextPool<QuiosqueContext>(options =>
    options.UseNpgsql(connectionString));

// 2. Libera o acesso para o JavaScript
builder.Services.AddCors(options =>
{
    options.AddPolicy("LiberarFrontend", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

// 3. Adiciona suporte para Controllers (onde ficarão os endpoints de pedido/cardápio) e para o OpenAPI (Swagger)
builder.Services.AddControllers();

// Mantém o suporte para o OpenAPI (Swagger) mesmo em produção, mas só ativa a interface visual no desenvolvimento
builder.Services.AddOpenApi();

var app = builder.Build();

// Configura o Pipeline de requisições
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    // Dica: Se quiser ver a interface visual do Swagger, adicione app.UseSwaggerUI() aqui se instalado
}

app.UseHttpsRedirection();

// ATIVA O CORS (Deve vir antes do Authorization)
app.UseCors("LiberarFrontend");

app.UseAuthorization();

// Mapeia os seus Controllers (Onde ficarão os códigos de pedido/cardápio)
app.MapControllers();

app.Run();
