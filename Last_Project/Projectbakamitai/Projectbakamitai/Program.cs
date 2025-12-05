using Microsoft.EntityFrameworkCore;
using Projectbakamitai.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ProjectBakamitaiContext>(option =>
    option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnect")));

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("Twin Turbo V8", new() { Title = "Project_Bakamitai", Version = "Twin Turbo V8" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/Twin Turbo V8/swagger.json", "Project_Bakamitai"));

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
