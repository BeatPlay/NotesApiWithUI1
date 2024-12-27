
using Microsoft.AspNetCore.Mvc;
using NotesApiWithUI.Models;
using NotesApiWithUI.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace NotesApiWithUI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotesController : ControllerBase
    {
        private readonly NoteService _noteService;
        private readonly TelegramService _telegramService;

        public NotesController(NoteService noteService, TelegramService telegramService)
        {
            _noteService = noteService;
            _telegramService = telegramService;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Получение всех заметок", Description = "Возвращает список всех заметок")]
        [SwaggerResponse(200, "Список заметок успешно получен")]
        public async Task<IActionResult> GetAll()
        {
            var notes = _noteService.GetAll();

            // Отправляем список заметок в Telegram
            var message = "Список всех заметок:\n";
            foreach (var note in notes)
            {
                message += $"- ID: {note.Id}, Title: {note.Title}, CreatedAt: {note.CreatedAt:yyyy-MM-dd HH:mm}\n";
            }

            // Отправляем сообщение в Telegram
            await _telegramService.SendMessageAsync(message);
            return Ok(notes);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var note = _noteService.GetById(id);

            // Формируем строку для Telegram с информацией о заметке по ID
            if (note != null)
            {
                var message = $"Заметка:\nID: {note.Id}\nTitle: {note.Title}\nContent: {note.Content}\nCreatedAt: {note.CreatedAt:yyyy-MM-dd HH:mm}";
                await _telegramService.SendMessageAsync(message);
            }
            else
            {
                await _telegramService.SendMessageAsync("Заметка не найдена.");
            }
            return note == null ? NotFound() : Ok(note);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Добавление новой заметки", Description = "Добавляет новую заметку в список")]
        [SwaggerResponse(201, "Заметка успешно добавлена")]
        [SwaggerResponse(400, "Некорректные данные")]
        public async Task<IActionResult> Add(Note newNote)
        {
            if (newNote == null || string.IsNullOrWhiteSpace(newNote.Title))
            {
                // Отправляем сообщение о некорректных данных
                await _telegramService.SendMessageAsync("Ошибка: данные заметки некорректны.");
                return BadRequest("Данные заметки некорректны.");
            }

            var addedNote = _noteService.Add(newNote);

            // Отправляем сообщение о добавлении новой заметки
            var message = $"Заметка добавлена:\nID: {addedNote.Id}\nTitle: {addedNote.Title}\nContent: {addedNote.Content}\nCreatedAt: {addedNote.CreatedAt:yyyy-MM-dd HH:mm}";
            await _telegramService.SendMessageAsync(message);

            return CreatedAtAction(nameof(GetById), new { id = addedNote.Id }, addedNote);
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Удаление заметки", Description = "Удаляет заметку по идентификатору")]
        [SwaggerResponse(204, "Заметка успешно удалена")]
        [SwaggerResponse(404, "Заметка не найдена")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = _noteService.Delete(id);

            if (!deleted)
            {
                // Отправляем сообщение о том, что заметка не найдена
                await _telegramService.SendMessageAsync($"Ошибка: Заметка с ID {id} не найдена.");
                return NotFound();
            }

            // Отправляем сообщение о том, что заметка удалена
            await _telegramService.SendMessageAsync($"Заметка с ID {id} успешно удалена.");
            return NoContent();
        }

        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Обновление заметки", Description = "Обновляет заметку по идентификатору")]
        [SwaggerResponse(200, "Заметка успешно обновлена")]
        [SwaggerResponse(404, "Заметка не найдена")]
        public async Task<IActionResult> Update(int id, Note updatedNote)
        {
            if (updatedNote == null)
            {
                // Отправляем сообщение о некорректных данных
                await _telegramService.SendMessageAsync("Ошибка: Данные для обновления заметки некорректны.");
                return BadRequest("Данные для обновления заметки некорректны.");
            }

            var note = _noteService.Update(id, updatedNote);

            if (note == null)
            {
                // Отправляем сообщение о том, что заметка не найдена
                await _telegramService.SendMessageAsync($"Ошибка: Заметка с ID {id} не найдена.");
                return NotFound();
            }

            // Отправляем сообщение о том, что заметка обновлена
            var message = $"Заметка обновлена:\nID: {note.Id}\nTitle: {note.Title}\nContent: {note.Content}\nUpdatedAt: {note.CreatedAt:yyyy-MM-dd HH:mm}";
            await _telegramService.SendMessageAsync(message);

            return Ok(note);
        }
    }
}