using NotesApiWithUI.Services;
var builder = WebApplication.CreateBuilder(args);
// ��������� NoteService
builder.Services.AddSingleton<NoteService>();
builder.Services.AddControllersWithViews();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Notes API",
        Version = "v1",
        Description = "API ��� ������ � ���������",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "������",
            Email = "chaikin@sfedu.ru"
        }
    });
    c.EnableAnnotations(); // ��������� ��������� ���������
});
builder.Services.AddSingleton<TelegramService>();
var app = builder.Build();
app.UseHttpsRedirection();
app.UseStaticFiles();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Notes API v1");
        c.RoutePrefix = "swagger"; // ���������� ���� ��� Swagger UI
    });
}
app.UseRouting();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();
