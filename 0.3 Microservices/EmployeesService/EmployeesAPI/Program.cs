

var builder = WebApplication.CreateSlimBuilder(args);

builder.AddApplicationServices();


var app = builder.Build();


app.UseHttpsRedirection();
app.MapDefaultEndpoints();
app.MapControllers();
app.UseExceptionHandler("/Error", createScopeForErrors: true);
// The default HSTS value is 30 days.
// You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
app.UseHsts();


//var sampleTodos = new Todo[] {
//    new(1, "Walk the dog"),
//    new(2, "Do the dishes", DateOnly.FromDateTime(DateTime.Now)),
//    new(3, "Do the laundry", DateOnly.FromDateTime(DateTime.Now.AddDays(1))),
//    new(4, "Clean the bathroom"),
//    new(5, "Clean the car", DateOnly.FromDateTime(DateTime.Now.AddDays(2)))
//};

//var todosApi = app.MapGroup("/todos");
//todosApi.MapGet("/", () => sampleTodos);
//todosApi.MapGet("/{id}", (int id) =>
//    sampleTodos.FirstOrDefault(a => a.Id == id) is { } todo
//        ? Results.Ok(todo)
//        : Results.NotFound());

app.Run();

//public record Todo(int Id, string? Title, DateOnly? DueBy = null, bool IsComplete = false);

//[JsonSerializable(typeof(Todo[]))]
//internal partial class AppJsonSerializerContext : JsonSerializerContext
//{

//}
